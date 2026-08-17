using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Rules.Signatures;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Kassyi.Generators.DependencyProperty;

/// <summary>Provides data extraction and transformation helpers for Roslyn generator pipeline stages.</summary>
public static class PrepareData
{
    /// <summary>Extracts dependency property model data directly from generator attribute context.</summary>
    public static DependencyPropertyData GetDependencyPropertyData(
        this GeneratorAttributeContext context,
        bool isAddOwner = false,
        bool isAttached = false)
    {
        var attribute = context.Attribute ?? throw new ArgumentNullException(nameof(context));

        return new DependencyPropertyDataBuilder()
            .WithCoreProperties(attribute, context.Framework, context.Version, isAddOwner, isAttached, context.ClassSymbol)
            .WithMetadata(attribute)
            .WithDefaultValues(attribute, context.ClassSyntax.TryFindAttributeSyntax(attribute), context.SemanticModel)
            .WithXmlDocumentation(attribute)
            .WithCallbacks(attribute, context.ClassSymbol)
            .Build();
    }

    /// <summary>Extracts dependency property model data directly from multi-attribute generator context.</summary>
    public static DependencyPropertyData GetDependencyPropertyData(
        this GeneratorMultiAttributeContext context,
        AttributeData attribute) =>
        context.ForAttribute(attribute).GetDependencyPropertyData();

    private static readonly ImmutableArray<Rules.IMethodSignatureRule> s_signatureRules =
    [
        new NoParametersRule(),
        new SingleParameterRule(),
        new DoubleParameterRule(),
        new TripleParameterRule()
    ];

    internal static MethodSignatureMatch CheckMethodsDirectly(
        INamedTypeSymbol classSymbol, string methodName, string targetType, string senderType)
    {
        var match = new MethodSignatureMatch();
        foreach (var member in classSymbol.GetMembers(methodName))
        {
            if (member is not IMethodSymbol method)
            {
                continue;
            }

            match.HasMethod = true;
            foreach (var rule in s_signatureRules)
            {
                rule.Evaluate(method, targetType, senderType, match);
            }
        }
        return match;
    }

    internal static string GenerateDependencyObjectType(Framework framework) =>
        framework == Framework.Maui ? "Microsoft.Maui.Controls.BindableObject" :
        framework == Framework.Avalonia ? "Avalonia.AvaloniaObject" :
        framework == Framework.Wpf ? "System.Windows.DependencyObject" :
        framework is Framework.Uwp or Framework.Uno ? "Windows.UI.Xaml.DependencyObject" :
        "Microsoft.UI.Xaml.DependencyObject";

    /// <summary>Extracts routed or weak event model data from attribute definitions.</summary>
    public static EventData GetEventData(this AttributeData attribute, bool isStaticClass)
    {
        attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        // [WHY] Avoid LINQ ElementAtOrDefault to prevent delegate allocations.
        var name = (attribute.ConstructorArguments is { Length: > 0 } ctorArgs0
            ? ctorArgs0[0].Value?.ToString()?.TrimStart('@')
            : null) ?? string.Empty;

        var arg1 = attribute.ConstructorArguments is { Length: > 1 } ctorArgs1
            ? ctorArgs1[1]
            : default;

        var strategy = arg1.ToEnum(defaultValue: RoutedEventStrategy.Direct).ToString("G");
        var isStatic = attribute.GetNamedArgument(nameof(WeakEventAttribute.IsStatic)).ToBoolean();
        var typeSymbol =
            attribute.GetGenericTypeArgument(0) ??
            attribute.GetNamedArgument(nameof(RoutedEventAttribute.Type)).Value as ITypeSymbol;
        var type = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
        var isValueType =
            typeSymbol?.IsValueType ??
            arg1.Type?.IsValueType ??
            true;
        var isAttached = attribute.GetNamedArgument(nameof(RoutedEventAttribute.IsAttached)).ToBoolean();
        var description = attribute.GetNamedArgument(nameof(RoutedEventAttribute.Description)).Value?.ToString();
        var category = attribute.GetNamedArgument(nameof(RoutedEventAttribute.Category)).Value?.ToString();

        var xmlDocumentation =
            attribute.GetNamedArgument(nameof(RoutedEventAttribute.XmlDocumentation)).Value?.ToString();
        var eventXmlDocumentation = attribute.GetNamedArgument(nameof(RoutedEventAttribute.EventXmlDocumentation)).Value
            ?.ToString();

        var winRtEvents = attribute.GetNamedArgument(nameof(RoutedEventAttribute.WinRtEvents)).ToBoolean();

        return new EventData(
            Name: name,
            Strategy: strategy,
            Type: type,
            IsValueType: isValueType,
            IsAttached: isAttached || isStatic || isStaticClass,
            Description: description,
            Category: category,
            XmlDocumentation: xmlDocumentation,
            EventXmlDocumentation: eventXmlDocumentation,
            WinRtEvents: winRtEvents);
    }

    /// <summary>Extracts target class metadata from a named type symbol.</summary>
    public static ClassData GetClassData(
        this INamedTypeSymbol classSymbol,
        Framework framework,
        string version)
    {
        classSymbol = classSymbol ?? throw new ArgumentNullException(nameof(classSymbol));

        // [WHY] Avoid LINQ Any() to eliminate delegate and enumerator allocations during syntax tree exploration on every keystroke.
        var isFileLocal = CheckIsFileLocal(classSymbol);

        if (isFileLocal)
        {
            var location = classSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLocation() ?? Location.None;
            throw new DiagnosticException(
                Diagnostic.Create(Diagnostics.DiagnosticDescriptors.FileScopedTypeNotSupported, location, classSymbol.Name));
        }

        // [WHY] Sanitize invalid filename characters (<, >, ,, spaces) in a single pass to ensure valid Roslyn hint names without heap allocations for non-generic types.
        var fullClassName = classSymbol.ToDisplayString().SanitizeFileName();
        var @namespace = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;

        var keyword = classSymbol.IsRecord 
            ? (classSymbol.IsValueType ? "record struct" : "record") 
            : (classSymbol.IsValueType ? "struct" : "class");
        var nameWithTypeParameters = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        var parentClasses = GetParentClasses(classSymbol);

        // [WHY] Avoid LINQ Where() and string.Join to eliminate array allocations and enumerator allocations on every keystroke.
        var modifiers = GetModifiers(classSymbol);

        return new ClassData(
            Namespace: @namespace,
            Name: className,
            FullName: fullClassName,
            Type: nameWithTypeParameters,
            Keyword: keyword,
            NameWithTypeParameters: nameWithTypeParameters,
            Modifiers: modifiers,
            Version: version,
            IsStatic: classSymbol.IsStatic,
            Framework: framework,
            ParentClasses: parentClasses);
    }

    private static EquatableArray<ParentClassData> GetParentClasses(INamedTypeSymbol classSymbol)
    {
        var parentClassesBuilder = ImmutableArray.CreateBuilder<ParentClassData>();
        var currentParent = classSymbol.ContainingType;
        while (currentParent != null)
        {
            var parentKeyword = currentParent.IsRecord 
                ? (currentParent.IsValueType ? "record struct" : "record") 
                : (currentParent.IsValueType ? "struct" : "class");
            var parentName = currentParent.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            var parentModifiers = GetModifiers(currentParent);

            parentClassesBuilder.Add(new ParentClassData(parentKeyword, parentName, parentModifiers));
            currentParent = currentParent.ContainingType;
        }

        return parentClassesBuilder.ToImmutable().AsEquatableArray();
    }

    internal static bool? IsSpecialType(this ITypeSymbol? symbol)
    {
        return symbol switch
        {
            null => null,
            _ => symbol is IArrayTypeSymbol || symbol.SpecialType != SpecialType.None ||
                 (symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && symbol.BaseType is
                 {
                     SpecialType: not SpecialType.None
                 })
        };
    }

    internal static ExpressionSyntax? GetNamedArgumentExpressionSyntax(this AttributeSyntax attributeSyntax, string name)
    {
        attributeSyntax = attributeSyntax ?? throw new ArgumentNullException(nameof(attributeSyntax));

        if (attributeSyntax.ArgumentList == null)
        {
            return null;
        }

        // [WHY] Avoid LINQ to eliminate array allocations and enumerator allocations on every keystroke.
        foreach (var argument in attributeSyntax.ArgumentList.Arguments)
        {
            if (argument.NameEquals?.Name.Identifier.ValueText == name)
            {
                return argument.Expression;
            }
        }

        return null;
    }

    internal static string? GetNamedArgumentExpression(this AttributeSyntax attributeSyntax, string name) => attributeSyntax.GetNamedArgumentExpressionSyntax(name)?.ToFullString();

    internal static string? ExpandDefaultValueExpression(string? defaultValue, ExpressionSyntax? expression, ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null || string.IsNullOrWhiteSpace(defaultValue))
        {
            return defaultValue;
        }

        var targetSymbol = typeSymbol is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType
            ? nullableType.TypeArguments[0]
            : typeSymbol;

        if (expression == null)
        {
            try
            {
                expression = SyntaxFactory.ParseExpression(defaultValue!);
            }
            catch
            {
                // Fallback to raw string if Roslyn syntax parsing fails.
            }
        }

        if (expression is not ImplicitObjectCreationExpressionSyntax implicitNew)
        {
            return defaultValue;
        }

        var typeString = targetSymbol.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return $"{implicitNew.GetLeadingTrivia().ToFullString()}new {typeString}{implicitNew.ArgumentList.ToFullString()}{implicitNew.Initializer?.ToFullString() ?? ""}{implicitNew.GetTrailingTrivia().ToFullString()}";

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CheckIsFileLocal(INamedTypeSymbol classSymbol)
    {
        var current = classSymbol;
        while (current != null)
        {
            foreach (var syntaxRef in current.DeclaringSyntaxReferences)
            {
                if (syntaxRef.GetSyntax() is not TypeDeclarationSyntax typeDecl)
                {
                    continue;
                }

                foreach (var modifier in typeDecl.Modifiers)
                {
                    if (modifier.IsKind(SyntaxKind.FileKeyword) || modifier.Text == "file")
                    {
                        return true;
                    }
                }
            }
            current = current.ContainingType;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetModifiers(INamedTypeSymbol classSymbol)
    {
        if (classSymbol.DeclaringSyntaxReferences.Length <= 0 ||
            classSymbol.DeclaringSyntaxReferences[0].GetSyntax() is not TypeDeclarationSyntax syntaxNode)
        {
            return classSymbol.IsStatic ? "public static " : "public ";
        }

        return ExtractModifiers(syntaxNode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ExtractModifiers(TypeDeclarationSyntax syntaxNode)
    {
        var modifiers = string.Empty;
        foreach (var m in syntaxNode.Modifiers)
        {
            if (!m.IsKind(SyntaxKind.PartialKeyword) && !m.IsKind(SyntaxKind.FileKeyword) && m.Text != "file")
            {
                modifiers += m.Text + " ";
            }
        }

        if (string.IsNullOrWhiteSpace(modifiers))
        {
            return string.Empty;
        }

        return modifiers.EndsWith(" ", StringComparison.Ordinal) ? modifiers : modifiers + " ";
    }
}
