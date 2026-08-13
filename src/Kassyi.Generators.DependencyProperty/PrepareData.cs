using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;



namespace Kassyi.Generators.DependencyProperty;

public static class PrepareData
{
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
            .WithCoreProperties(attribute, framework, version, isAddOwner, isAttached)
            .WithMetadata(attribute)
            .WithDefaultValues(attribute, attributeSyntax)
            .WithXmlDocumentation(attribute)
            .WithCallbacks(attribute, classSymbol)
            .Build();
    }

    internal static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    internal static (bool Has0, bool Has1, bool Has2, bool Has3, bool HasArgs1, bool HasArgs2) CheckMethodsDirectly(
        INamedTypeSymbol classSymbol, string methodName, string targetType, string senderType)
    {
        bool has0 = false, has1 = false, has2 = false, has3 = false;
        bool hasArgs1 = false, hasArgs2 = false;

        foreach (var member in classSymbol.GetMembers(methodName))
        {
            if (member is not IMethodSymbol method)
            {
                continue;
            }

            var p = method.Parameters;
            switch (p.Length)
            {
                case 0:
                    has0 = true;
                    break;
                case 1:
                    has1 |= CheckHas1(p, targetType, senderType);
                    hasArgs1 |= IsEventArgsType(p[0].Type.Name);
                    break;
                case 2:
                    has2 |= CheckHas2(p, targetType, senderType);
                    hasArgs2 |= IsEventArgsType(p[1].Type.Name);
                    break;
                case 3:
                    has3 |= CheckHas3(p, targetType, senderType);
                    break;
            }
        }

        return (has0, has1, has2, has3, hasArgs1, hasArgs2);
    }

    private static bool CheckHas1(ImmutableArray<IParameterSymbol> parameters, string targetType, string senderType)
    {
        var type0 = GetNormalizedTypeName(parameters[0].Type);
        return type0 == targetType || type0 == senderType;
    }

    private static bool CheckHas2(ImmutableArray<IParameterSymbol> parameters, string targetType, string senderType)
    {
        var type0 = GetNormalizedTypeName(parameters[0].Type);
        var type1 = GetNormalizedTypeName(parameters[1].Type);
        return (type0 == targetType || type0 == senderType) && type1 == targetType;
    }

    private static bool CheckHas3(ImmutableArray<IParameterSymbol> parameters, string targetType, string senderType)
    {
        var type0 = GetNormalizedTypeName(parameters[0].Type);
        var type1 = GetNormalizedTypeName(parameters[1].Type);
        var type2 = GetNormalizedTypeName(parameters[2].Type);
        return type0 == senderType && type1 == targetType && type2 == targetType;
    }

    private static string GetNormalizedTypeName(ITypeSymbol typeSymbol)
    {
        var str = typeSymbol.ToDisplayString(TypeFormat);
        return str.EndsWith("?", StringComparison.Ordinal) ? str.Substring(0, str.Length - 1) : str;
    }

    private static bool IsEventArgsType(string typeName) =>
        typeName.EndsWith("EventArgs", StringComparison.Ordinal) ||
        typeName.EndsWith("EventArgs>", StringComparison.Ordinal) ||
        typeName.EndsWith("DependencyPropertyChangedEventArgs", StringComparison.Ordinal) ||
        typeName.EndsWith("ValueChangedEventArgs", StringComparison.Ordinal);

    internal static string GenerateDependencyObjectType(Framework framework) =>
        framework == Framework.Maui ? "Microsoft.Maui.Controls.BindableObject" :
        framework == Framework.Avalonia ? "Avalonia.AvaloniaObject" :
        framework == Framework.Wpf ? "System.Windows.DependencyObject" :
        framework is Framework.Uwp or Framework.Uno ? "Windows.UI.Xaml.DependencyObject" :
        "Microsoft.UI.Xaml.DependencyObject";

    public static EventData GetEventData(this AttributeData attribute, bool isStaticClass)
    {
        attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        var name =
            attribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ??
            string.Empty;
        var strategy = attribute.ConstructorArguments.ElementAtOrDefault(1)
            .ToEnum(defaultValue: RoutedEventStrategy.Direct)
            .ToString("G");
        var isStatic = attribute.GetNamedArgument(nameof(WeakEventAttribute.IsStatic)).ToBoolean();
        var typeSymbol =
            attribute.GetGenericTypeArgument(0) ??
            attribute.GetNamedArgument(nameof(RoutedEventAttribute.Type)).Value as ITypeSymbol;
        var type = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
        var isValueType =
            typeSymbol?.IsValueType ??
            attribute.ConstructorArguments.ElementAtOrDefault(1).Type?.IsValueType ??
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

    public static ClassData GetClassData(
        this INamedTypeSymbol classSymbol,
        Framework framework,
        string version)
    {
        classSymbol = classSymbol ?? throw new ArgumentNullException(nameof(classSymbol));

        var fullClassName = classSymbol.ToString() ?? string.Empty;
        var type = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var @namespace = fullClassName.Substring(0, fullClassName.LastIndexOf('.'));
        var className = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);

        return new ClassData(
            Namespace: @namespace,
            Name: className,
            FullName: fullClassName,
            Type: type,
            Modifiers: classSymbol.IsStatic ? "public static " : string.Empty,
            Version: version,
            IsStatic: classSymbol.IsStatic,
            Framework: framework);
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

        return attributeSyntax.ArgumentList?.Arguments
            .FirstOrDefault(x =>
            {
                var nameEquals = x.NameEquals?.ToFullString()
                    .Trim('=', ' ', '\t', '\r', '\n');

                return nameEquals == name;
            })?
            .Expression
            .ToFullString();
    }

    private static string RemoveNameof(this string value)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));

        return value.Contains("nameof(")
            ? value
                .Substring(value.LastIndexOf('.') + 1)
                .TrimEnd(')', ' ')
            : value;
    }

    internal static AttributeSyntax? TryFindAttributeSyntax(this ClassDeclarationSyntax classSyntax,
        AttributeData attribute)
    {
        var name = attribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();

        return classSyntax.AttributeLists
            .SelectMany(static x => x.Attributes)
            .FirstOrDefault(
                x => x.ArgumentList?.Arguments.FirstOrDefault()?.ToString().Trim('"').RemoveNameof() == name);
    }

    public static ITypeSymbol? GetGenericTypeArgumentOrNamed(this AttributeData attribute, int position, string name)
    {
        attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        return attribute.GetGenericTypeArgument(position) ??
               attribute.GetNamedArgument(name).Value as ITypeSymbol;
    }

    internal static string? ExpandDefaultValueExpression(string? defaultValue, ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null ||
            defaultValue is not { Length: > 0 } ||
            !defaultValue.Trim().StartsWith("new", StringComparison.Ordinal))
        {
            return defaultValue;
        }

        var targetSymbol = typeSymbol is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType
            ? nullableType.TypeArguments[0]
            : typeSymbol;

        return SyntaxFactory.ParseExpression(defaultValue) switch
        {
            ImplicitObjectCreationExpressionSyntax implicitNew => SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.Token(SyntaxKind.NewKeyword).WithTrailingTrivia(SyntaxFactory.Space),
                SyntaxFactory.ParseTypeName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).TrimEnd('?')),
                implicitNew.ArgumentList,
                implicitNew.Initializer).ToFullString(),
            _ => defaultValue,
        };
    }
}
