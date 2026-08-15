using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Rules.Signatures;
using System.Runtime.CompilerServices;


namespace Kassyi.Generators.DependencyProperty;

/// <summary>Provides data extraction and transformation helpers for Roslyn generator pipeline stages.</summary>
public static class PrepareData
{
    /// <summary>Extracts dependency property model data from attribute and syntax definitions.</summary>
    public static DependencyPropertyData GetDependencyPropertyData(
        this AttributeData attribute,
        Framework framework,
        string version,
        INamedTypeSymbol? classSymbol = null,
        AttributeSyntax? attributeSyntax = null,
        bool isAddOwner = false,
        bool isAttached = false)
    {
        attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        return new DependencyPropertyDataBuilder()
            .WithCoreProperties(attribute, framework, version, isAddOwner, isAttached, classSymbol)
            .WithMetadata(attribute)
            .WithDefaultValues(attribute, attributeSyntax)
            .WithXmlDocumentation(attribute)
            .WithCallbacks(attribute, classSymbol)
            .Build();
    }

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
            var descriptor = new DiagnosticDescriptor(
                id: "DPG0002",
                title: "Invalid Type Modifier",
                messageFormat: "File scoped types are not supported by Source Generators ('{0}')",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
            throw new DiagnosticException(
                Diagnostic.Create(descriptor, location, classSymbol.Name));
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

            var parentModifiers = string.Empty;
            if (currentParent.DeclaringSyntaxReferences.Length > 0 && currentParent.DeclaringSyntaxReferences[0].GetSyntax() is TypeDeclarationSyntax parentSyntaxNode)
            {
                foreach (var m in parentSyntaxNode.Modifiers)
                {
                    if (!m.IsKind(SyntaxKind.PartialKeyword) && !m.IsKind(SyntaxKind.FileKeyword) && m.Text != "file")
                    {
                        parentModifiers += m.Text + " ";
                    }
                }
            }
            else
            {
                parentModifiers = currentParent.IsStatic ? "public static " : "public ";
            }
            if (string.IsNullOrWhiteSpace(parentModifiers))
            {
                parentModifiers = string.Empty;
            }
            else if (!parentModifiers.EndsWith(" ", StringComparison.Ordinal))
            {
                parentModifiers += " ";
            }

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

    internal static string? GetNamedArgumentExpression(this AttributeSyntax attributeSyntax, string name)
    {
        attributeSyntax = attributeSyntax ?? throw new ArgumentNullException(nameof(attributeSyntax));

        if (attributeSyntax.ArgumentList == null)
        {
            return null;
        }

        // [WHY] Avoid LINQ FirstOrDefault(predicate) to eliminate delegate allocations during syntax tree analysis.
        foreach (var argument in attributeSyntax.ArgumentList.Arguments)
        {
            var nameEquals = argument.NameEquals?.ToFullString().Trim('=', ' ', '\t', '\r', '\n');
            if (nameEquals == name)
            {
                return argument.Expression.ToFullString();
            }
        }

        return null;
    }

    internal static string? ExpandDefaultValueExpression(string? defaultValue, ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null || string.IsNullOrWhiteSpace(defaultValue))
        {
            return defaultValue;
        }

        var targetSymbol = typeSymbol is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType
            ? nullableType.TypeArguments[0]
            : typeSymbol;

        try
        {
            var expression = SyntaxFactory.ParseExpression(defaultValue!);
            if (expression is ImplicitObjectCreationExpressionSyntax implicitNew)
            {
                return SyntaxFactory.ObjectCreationExpression(
                    SyntaxFactory.Token(SyntaxKind.NewKeyword).WithTrailingTrivia(SyntaxFactory.Space),
                    SyntaxFactory.ParseTypeName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).TrimEnd('?')),
                    implicitNew.ArgumentList,
                    implicitNew.Initializer).ToFullString();
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // [WHY] Fallback to raw string if Roslyn syntax parsing fails.
        }

        return defaultValue;
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

        var modifiers = string.Empty;
        foreach (var m in syntaxNode.Modifiers)
        {
            if (!m.IsKind(SyntaxKind.PartialKeyword))
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
