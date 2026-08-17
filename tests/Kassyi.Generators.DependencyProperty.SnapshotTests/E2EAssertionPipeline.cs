#nullable enable

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Kassyi.Generators.DependencyProperty.Generators;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public static class E2EAssertionPipeline
{
    public static void Verify(
        string inputSource,
        string[] generatedSources,
        Framework framework,
        Compilation compilation,
        ImmutableArray<Diagnostic> diagnostics,
        string callerName,
        bool skipE2EValidation = false)
    {
        // 意図的なエラーや特殊なエッジケースを検証するテストはアサーションパイプラインをスキップ
        if (skipE2EValidation)
        {
            return;
        }

        var inputRoot = CSharpSyntaxTree.ParseText(inputSource).GetCompilationUnitRoot();
        var outputRoots = generatedSources.Select(s => CSharpSyntaxTree.ParseText(s).GetCompilationUnitRoot()).ToArray();

        // Level 1: Ensure generator produced expected elements
        VerifyCountMatching(inputRoot, outputRoots, framework);

        // Level 2: Signature and syntax validation
        VerifySignatureMatching(outputRoots, framework);
        
        // Level 3: Verify get/set accessors
        VerifyClrWrappers(outputRoots, framework);

        // Level 4: Diagnostic Validation
        VerifyDiagnostics(diagnostics, callerName);
    }

    internal static void VerifyCountMatching(SyntaxNode inputRoot, SyntaxNode[] outputRoots, Framework framework)
    {
        // 1. Count target attributes in input
        var targetAttributes = inputRoot.DescendantNodes()
            .OfType<AttributeSyntax>()
            .Where(a => GetSimpleName(a.Name) is KnownAttributeShortNames.DependencyProperty 
                                              or KnownAttributes.DependencyPropertyAttribute
                                              or KnownAttributeShortNames.AttachedDependencyProperty 
                                              or KnownAttributes.AttachedDependencyPropertyAttribute)
            .ToList();

        // 2. Count generated fields in outputs
        // Exclude DependencyPropertyKey since it generates alongside DependencyProperty for ReadOnly properties
        var dpFieldsCount = outputRoots.SelectMany(r => r.DescendantNodes().OfType<FieldDeclarationSyntax>())
            .Count(f => {
                var typeName = GetSimpleName(f.Declaration.Type);
                return (typeName is KnownPropertyTypes.DependencyProperty 
                                 or KnownPropertyTypes.StyledProperty 
                                 or KnownPropertyTypes.DirectProperty 
                                 or KnownPropertyTypes.AttachedProperty 
                                 or KnownPropertyTypes.BindableProperty ||
                        typeName.EndsWith("Property", StringComparison.Ordinal)) &&
                       !typeName.EndsWith("PropertyKey", StringComparison.Ordinal);
            });

        int dpAttributesCount = targetAttributes.Count;

        // Verify count matches (only if there are attributes)
        if (dpAttributesCount > 0 && dpFieldsCount != dpAttributesCount)
        {
            throw new Exception($"Level 1 Assertion Failed: Expected {dpAttributesCount} generated property fields, but found {dpFieldsCount}.");
        }
    }

    private static void VerifySignatureMatching(SyntaxNode[] outputRoots, Framework framework)
    {
        // Find InvocationExpression for Register / RegisterAttached / Create / CreateAttached
        var invocations = outputRoots.SelectMany(r => r.DescendantNodes().OfType<InvocationExpressionSyntax>())
            .Where(i => i.Expression is MemberAccessExpressionSyntax mae && 
                        mae.Name.Identifier.ValueText is KnownMethodNames.Register 
                                                     or KnownMethodNames.RegisterAttached 
                                                     or KnownMethodNames.RegisterReadOnly 
                                                     or KnownMethodNames.RegisterAttachedReadOnly 
                                                     or KnownMethodNames.RegisterDirect 
                                                     or KnownMethodNames.Create 
                                                     or KnownMethodNames.CreateAttached 
                                                     or KnownMethodNames.CreateReadOnly 
                                                     or KnownMethodNames.CreateAttachedReadOnly)
            .ToList();

        foreach (var inv in invocations)
        {
            var args = inv.ArgumentList.Arguments;
            if (args.Count >= 3)
            {
                // Name argument
                var nameArg = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "name") ?? args[0];
                if (nameArg.Expression is not InvocationExpressionSyntax && nameArg.Expression is not LiteralExpressionSyntax)
                {
                    throw new Exception("Level 2 Assertion Failed: Name argument is not a nameof expression or literal.");
                }

                // Avalonia handles things slightly differently (e.g. AvaloniaProperty.Register<TOwner, TValue>)
                if (framework == Framework.Avalonia)
                {
                    // Avalonia has type arguments in the method call for Register<TOwner, TValue>
                    if (inv.Expression is MemberAccessExpressionSyntax mae && mae.Name is GenericNameSyntax)
                    {
                        continue;
                    }
                }

                // PropertyType argument
                var propTypeArg = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "propertyType" || a.NameColon?.Name.Identifier.Text == "returnType");
                if (propTypeArg == null && args.Count > 1) propTypeArg = args[1];

                if (propTypeArg != null && !IsTypeOfExpression(propTypeArg.Expression))
                {
                    throw new Exception($"Level 2 Assertion Failed: PropertyType argument is not typeof. Found: {propTypeArg.Expression}");
                }

                // OwnerType argument
                var ownerTypeArg = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "ownerType" || a.NameColon?.Name.Identifier.Text == "declaringType");
                if (ownerTypeArg == null && args.Count > 2) ownerTypeArg = args[2];

                if (ownerTypeArg != null && !IsTypeOfExpression(ownerTypeArg.Expression))
                {
                    throw new Exception($"Level 2 Assertion Failed: OwnerType argument is not typeof. Found: {ownerTypeArg.Expression}");
                }
            }
        }
    }

    private static void VerifyClrWrappers(SyntaxNode[] outputRoots, Framework framework)
    {
        var properties = outputRoots
            .SelectMany(r => r.DescendantNodes().OfType<PropertyDeclarationSyntax>())
            .Where(p => p.Identifier.ValueText != "CurrentManager")
            .ToList();
        foreach (var prop in properties)
        {
            var accessors = prop.AccessorList?.Accessors;
            if (accessors != null)
            {
                var get = accessors.Value.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword));
                var set = accessors.Value.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword));

                if (get != null)
                {
                    ExpressionSyntax? getExpr = get.ExpressionBody?.Expression;
                    if (getExpr == null && get.Body != null)
                    {
                        var returnStatement = get.Body.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault();
                        getExpr = returnStatement?.Expression;
                    }

                    if (getExpr != null && !IsValidGetterExpression(getExpr))
                        throw new Exception("Level 3 Assertion Failed: Getter does not call GetValue or return a backing field.");
                }

                if (set != null)
                {
                    ExpressionSyntax? setExpr = set.ExpressionBody?.Expression;
                    if (setExpr == null && set.Body != null)
                    {
                        var exprStatement = set.Body.Statements.OfType<ExpressionStatementSyntax>().FirstOrDefault();
                        setExpr = exprStatement?.Expression;
                    }

                    if (setExpr != null && !IsValidSetterExpression(setExpr))
                        throw new Exception("Level 3 Assertion Failed: Setter does not call SetValue or SetAndRaise.");
                }
            }
        }
    }

    private static bool IsTypeOfExpression(ExpressionSyntax expression)
    {
        var unwrapped = UnwrapExpression(expression);
        return unwrapped is TypeOfExpressionSyntax;
    }

    private static bool IsValidGetterExpression(ExpressionSyntax expression)
    {
        var expr = UnwrapExpression(expression);

        // Check for GetValue invocation
        if (expr is InvocationExpressionSyntax invocation)
        {
            var methodName = GetMethodName(invocation);
            if (methodName == KnownMethodNames.GetValue)
            {
                return true;
            }
        }

        // Backing field access: e.g. _isSpinning or this._isSpinning
        if (expr is IdentifierNameSyntax id && id.Identifier.ValueText.StartsWith("_", StringComparison.Ordinal))
        {
            return true;
        }

        if (expr is MemberAccessExpressionSyntax memberAccess && memberAccess.Name.Identifier.ValueText.StartsWith("_", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static bool IsValidSetterExpression(ExpressionSyntax expression)
    {
        var expr = UnwrapExpression(expression);

        if (expr is InvocationExpressionSyntax invocation)
        {
            var methodName = GetMethodName(invocation);
            if (methodName is KnownMethodNames.SetValue or KnownMethodNames.SetAndRaise)
            {
                return true;
            }
        }

        return false;
    }

    private static ExpressionSyntax UnwrapExpression(ExpressionSyntax expression)
    {
        while (true)
        {
            if (expression is ParenthesizedExpressionSyntax parenthesized)
            {
                expression = parenthesized.Expression;
            }
            else if (expression is CastExpressionSyntax cast)
            {
                expression = cast.Expression;
            }
            else
            {
                return expression;
            }
        }
    }

    private static string? GetMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            IdentifierNameSyntax id => id.Identifier.ValueText,
            MemberAccessExpressionSyntax mae => mae.Name.Identifier.ValueText,
            GenericNameSyntax gn => gn.Identifier.ValueText,
            _ => null
        };
    }

    private static void VerifyDiagnostics(ImmutableArray<Diagnostic> diagnostics, string callerName)
    {
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        
        if (errors.Any())
        {
            throw new Exception($"Level 4 Assertion Failed: Found {errors.Count} compilation errors. First error: {errors.First().GetMessage()}");
        }

        var cs0108 = diagnostics.Where(d => d.Id == "CS0108").ToList(); // Hiding member
        if (cs0108.Any())
        {
            throw new Exception("Level 4 Assertion Failed: Found CS0108 (Member hides inherited member) warning.");
        }
    }

    private static string GetSimpleName(TypeSyntax type)
    {
        return type switch
        {
            IdentifierNameSyntax id => id.Identifier.ValueText,
            QualifiedNameSyntax qn => qn.Right.Identifier.ValueText,
            GenericNameSyntax gn => gn.Identifier.ValueText,
            AliasQualifiedNameSyntax alias => alias.Name.Identifier.ValueText,
            PredefinedTypeSyntax pre => pre.Keyword.ValueText,
            NullableTypeSyntax nts => GetSimpleName(nts.ElementType) + "?",
            ArrayTypeSyntax ats => GetSimpleName(ats.ElementType) + "[]",
            _ => type.ToString()
        };
    }
}
