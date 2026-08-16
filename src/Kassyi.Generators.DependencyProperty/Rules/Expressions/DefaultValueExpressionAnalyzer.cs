using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Rules.Expressions;

/// <summary>
/// Analyzes default value expressions to detect reference type instantiations or shared mutable reference members.
/// </summary>
internal static class DefaultValueExpressionAnalyzer
{
    /// <summary>
    /// Evaluates whether the given AST expression or any of its sub-expressions is a reference type instantiation or shared reference member.
    /// </summary>
    public static bool IsReferenceTypeExpression(
        ExpressionSyntax expression,
        ITypeSymbol? propertyTypeSymbol,
        INamedTypeSymbol? containingClassSymbol,
        SemanticModel? semanticModel,
        int? position)
    {
        if (expression.ContainsDiagnostics)
        {
            return IsConservativeReferenceTypeFallback(expression.ToString(), propertyTypeSymbol);
        }

        // Check top-level node first
        if (IsReferenceTypeNode(expression, propertyTypeSymbol, containingClassSymbol, semanticModel, position, isTopLevel: true))
        {
            return true;
        }

        // [OPTIMIZATION] If top-level node is an immutable leaf (literal, typeof, default), skip descendant traversal completely
        if (expression is LiteralExpressionSyntax or TypeOfExpressionSyntax or DefaultExpressionSyntax)
        {
            return false;
        }

        // Traverse descendant expressions with early pruning for irrelevant/leaf subtrees
        foreach (var node in expression.DescendantNodes(descendIntoChildren: ShouldDescendIntoNode))
        {
            if (node is ExpressionSyntax descExpr &&
                IsReferenceTypeNode(descExpr, null, containingClassSymbol, semanticModel, position, isTopLevel: false))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ShouldDescendIntoNode(SyntaxNode node)
    {
        // [OPTIMIZATION] Prune subtrees that cannot contain reference-type instantiations or member expressions
        return node switch
        {
            LiteralExpressionSyntax or
            TypeOfExpressionSyntax or
            DefaultExpressionSyntax or
            TypeSyntax or
            TypeArgumentListSyntax => false,

            _ => true
        };
    }


    private static bool IsConservativeReferenceTypeFallback(string? rawExpression, ITypeSymbol? propertyTypeSymbol)
    {
        // [WHY] Value types and strings are safe regardless of expression syntax errors
        if (propertyTypeSymbol != null && (propertyTypeSymbol.IsValueType || IsSpecialSafeType(propertyTypeSymbol)))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(rawExpression))
        {
            return false;
        }

        var trimmed = rawExpression!.Trim();
        // Literals, typeof, default, null are safe even when parsing fails
        if (trimmed is "null" || trimmed.StartsWith("typeof(", StringComparison.Ordinal) || trimmed.StartsWith("default", StringComparison.Ordinal))
        {
            return false;
        }

        // [WHY] Conservative fallback: if target property is a reference type and default value is not null/default, treat as potentially dangerous reference instantiation
        return propertyTypeSymbol is { IsValueType: false };
    }

    private static bool IsReferenceTypeNode(
        ExpressionSyntax expression,
        ITypeSymbol? propertyTypeSymbol,
        INamedTypeSymbol? containingClassSymbol,
        SemanticModel? semanticModel,
        int? position,
        bool isTopLevel)
    {
        return expression switch
        {
            // [WHY] Literals, typeof, and default expressions are always value types or immutable constants.
            LiteralExpressionSyntax or
            TypeOfExpressionSyntax or
            DefaultExpressionSyntax => false,

            // [WHY] Collection expressions, arrays, and anonymous objects are always allocated reference instances.
            CollectionExpressionSyntax or
            ArrayCreationExpressionSyntax or
            ImplicitArrayCreationExpressionSyntax or
            AnonymousObjectCreationExpressionSyntax => true,

            // [WHY] Object creations (new T()): reference type if T is a reference type.
            ObjectCreationExpressionSyntax objectCreation =>
                !IsKnownValueType(objectCreation, objectCreation.Type, propertyTypeSymbol, semanticModel, position, isTopLevel),

            // [WHY] Target-typed new(): reference type if target type is a reference type.
            ImplicitObjectCreationExpressionSyntax implicitCreation =>
                !IsKnownValueType(implicitCreation, null, propertyTypeSymbol, semanticModel, position, isTopLevel),

            // [WHY] Method calls: reference type if the return type is a reference type.
            InvocationExpressionSyntax invocation =>
                !IsKnownValueType(invocation, null, propertyTypeSymbol, semanticModel, position, isTopLevel),

            // [WHY] Record with-expression: reference type if target record is a reference type.
            WithExpressionSyntax withExpr =>
                !IsKnownValueType(withExpr, null, propertyTypeSymbol, semanticModel, position, isTopLevel),

            // [WHY] Member identifier or qualified access: reference type if referencing a mutable reference field/property.
            IdentifierNameSyntax or MemberAccessExpressionSyntax =>
                IsReferenceTypeMemberAccess(expression, containingClassSymbol, semanticModel, position),

            _ => false
        };
    }

    private static bool IsKnownValueType(
        ExpressionSyntax expression,
        TypeSyntax? typeSyntax,
        ITypeSymbol? propertyTypeSymbol,
        SemanticModel? semanticModel,
        int? position,
        bool isTopLevel)
    {
        if (isTopLevel && propertyTypeSymbol != null)
        {
            return propertyTypeSymbol.IsValueType || IsSpecialSafeType(propertyTypeSymbol);
        }

        // [WHY] Speculative binding requires a valid syntax position context; skip if position is unavailable
        if (semanticModel != null && position.HasValue)
        {
            var typeInfo = semanticModel.GetSpeculativeTypeInfo(position.Value, expression, SpeculativeBindingOption.BindAsExpression);
            var type = typeInfo.Type ?? typeInfo.ConvertedType;
            if (type != null && type.TypeKind != TypeKind.Error)
            {
                return type.IsValueType || IsSpecialSafeType(type);
            }
        }

        return typeSyntax switch
        {
            PredefinedTypeSyntax predefined =>
                predefined.Keyword.Kind() is not (SyntaxKind.StringKeyword or SyntaxKind.ObjectKeyword),

            NullableTypeSyntax or TupleTypeSyntax => true,

            _ => false
        };
    }

    private static bool IsReferenceTypeMemberAccess(
        ExpressionSyntax expression,
        INamedTypeSymbol? containingClassSymbol,
        SemanticModel? semanticModel,
        int? position)
    {
        // 1. [Best Practice] Resolve referenced symbol via SemanticModel
        if (semanticModel != null && position.HasValue)
        {
            var symbolInfo = semanticModel.GetSpeculativeSymbolInfo(position.Value, expression, SpeculativeBindingOption.BindAsExpression);
            var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
            if (symbol != null)
            {
                return IsReferenceTypeSymbol(symbol);
            }
        }

        // 2. [Fallback] Check member name against containingClassSymbol
        var memberName = expression switch
        {
            IdentifierNameSyntax id => id.Identifier.ValueText,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            _ => null
        };

        if (memberName != null && containingClassSymbol != null)
        {
            foreach (var member in containingClassSymbol.GetMembers(memberName))
            {
                if (IsReferenceTypeSymbol(member))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsReferenceTypeSymbol(ISymbol symbol)
    {
        ITypeSymbol? memberType = symbol switch
        {
            IFieldSymbol field when !field.HasConstantValue => field.Type,
            IPropertySymbol prop => prop.Type,
            _ => null
        };

        return memberType is { IsValueType: false } && !IsSpecialSafeType(memberType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSpecialSafeType(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null)
        {
            return false;
        }

        // [WHY] Only BCL System.String is special safe (immutable reference type). Avoid loose Name == "String" heuristic.
        return typeSymbol.SpecialType == SpecialType.System_String;
    }
}
