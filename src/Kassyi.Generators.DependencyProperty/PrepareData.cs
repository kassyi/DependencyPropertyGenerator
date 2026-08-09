using System.Collections.Immutable;
using System.ComponentModel;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



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

        var name =
            attribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ??
            string.Empty;
        var typeSymbol =
            attribute.GetGenericTypeArgument(0) ??
            attribute.ConstructorArguments.ElementAtOrDefault(1).Value as ITypeSymbol;
        var type = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
        var shortType = typeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? string.Empty;
        var isValueType = typeSymbol?.IsValueType ?? true;
        var isSpecialType = typeSymbol.IsSpecialType() ?? false;
        var defaultValue =
            attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString() ??
            attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValue)).Value?.ToString();
        var defaultValueDocumentation =
            attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString() ??
            attributeSyntax?.GetNamedArgumentExpression(nameof(DependencyPropertyAttribute.DefaultValue));

        defaultValue = ExpandDefaultValueExpression(defaultValue, typeSymbol);
        defaultValueDocumentation = ExpandDefaultValueExpression(defaultValueDocumentation, typeSymbol);
        var browsableForType =
            attribute.GetGenericTypeArgumentOrNamed(position: 1, nameof(AttachedDependencyPropertyAttribute.BrowsableForType))?
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var fromType =
            attribute.GetGenericTypeArgumentOrNamed(position: 1, nameof(AddOwnerAttribute.FromType))?
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var isReadOnly = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.IsReadOnly)).ToBoolean();
        var isDirect = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.IsDirect)).ToBoolean();

        var description = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Description)).Value?.ToString();
        var category = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Category)).Value?.ToString();
        var typeConverter = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.TypeConverter)).Value
            ?.ToString();
        var bindable = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Bindable)).ToNullableBoolean();
        var browsable = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Browsable)).ToNullableBoolean();
        var designerSerializationVisibility = attribute
            .GetNamedArgument(nameof(DependencyPropertyAttribute.DesignerSerializationVisibility))
            .ToEnum<DesignerSerializationVisibility>()?
            .ToString("G");
        var clsCompliant = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.ClsCompliant))
            .ToNullableBoolean();
        var localizability = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Localizability))
            .ToEnum<Localizability>()?
            .ToString("G");

        var xmlDocumentation = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.XmlDocumentation)).Value
            ?.ToString();
        var propertyXmlDocumentation = attribute
            .GetNamedArgument(nameof(DependencyPropertyAttribute.PropertyXmlDocumentation)).Value?.ToString();
        var getterXmlDocumentation = attribute
            .GetNamedArgument(nameof(AttachedDependencyPropertyAttribute.GetterXmlDocumentation)).Value?.ToString();
        var setterXmlDocumentation = attribute
            .GetNamedArgument(nameof(AttachedDependencyPropertyAttribute.SetterXmlDocumentation)).Value?.ToString();
        var bindEvent = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.BindEvent)).Value?.ToString();
        var bindEvents = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.BindEvents));
        var onChanged = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.OnChanged)).Value?.ToString();

        var affectsMeasure = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsMeasure)).ToBoolean();
        var affectsArrange = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsArrange)).ToBoolean();
        var affectsParentMeasure = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsParentMeasure))
            .ToBoolean();
        var affectsParentArrange = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsParentArrange))
            .ToBoolean();
        var affectsRender = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsRender)).ToBoolean();
        var inherits = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Inherits)).ToBoolean();
        var overridesInheritanceBehavior = attribute
            .GetNamedArgument(nameof(DependencyPropertyAttribute.OverridesInheritanceBehavior)).ToBoolean();
        var notDataBindable =
            attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.NotDataBindable)).ToBoolean();
        var journal = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Journal)).ToBoolean();
        var subPropertiesDoNotAffectRender = attribute
            .GetNamedArgument(nameof(DependencyPropertyAttribute.SubPropertiesDoNotAffectRender)).ToBoolean();
        var isAnimationProhibited =
            attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.IsAnimationProhibited)).ToBoolean();
        var defaultUpdateSourceTrigger = attribute
            .GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultUpdateSourceTrigger))
            .ToEnum<SourceTrigger>()?
            .ToString("G");
        var defaultBindingMode = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultBindingMode))
            .ToEnum<DefaultBindingMode>()?
            .ToString("G");
        var enableDataValidation = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.EnableDataValidation))
            .ToBoolean();
        var coerce = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Coerce)).ToBoolean();
        var validate = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Validate)).ToBoolean();
        var createDefaultValueCallback = attribute
            .GetNamedArgument(nameof(DependencyPropertyAttribute.CreateDefaultValueCallback)).ToBoolean();

        var isCustomOnChanged = !string.IsNullOrWhiteSpace(onChanged);
        var onChangedName = isCustomOnChanged ? onChanged! : $"On{name}Changed";
        var onChangingName = $"On{name}Changing";

        var targetType = type.Replace("global::", string.Empty).Replace("?", string.Empty);
        var targetSenderType = classSymbol != null
            ? (isAttached
                ? (browsableForType ?? GenerateDependencyObjectType(framework))
                : classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                .Replace("global::", string.Empty).Replace("?", string.Empty)
            : string.Empty;

        var (c0, c1, c2, c3, ca1, ca2) = classSymbol != null
            ? CheckMethodsDirectly(classSymbol, onChangedName, targetType, targetSenderType)
            : (false, false, false, false, false, false);

        var (ch0, ch1, ch2, ch3, _, _) = classSymbol != null
            ? CheckMethodsDirectly(classSymbol, onChangingName, targetType, targetSenderType)
            : (false, false, false, false, false, false);

        var bindEventsArray = (bindEvent != null
            ? [bindEvent]
            : bindEvents.Kind == TypedConstantKind.Array
                ? bindEvents.Values
                    .Select(static value => value.Value?.ToString() ?? string.Empty)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .ToArray()
                : Array.Empty<string>()).ToImmutableArray().AsEquatableArray();

        c2 |= !isCustomOnChanged && !isAttached && !bindEventsArray.IsEmpty;
        c3 |= !isCustomOnChanged && isAttached && !bindEventsArray.IsEmpty;

        return new DependencyPropertyData(
            Name: name,
            Type: type,
            Version: version,
            ShortType: shortType,
            IsValueType: isValueType,
            IsSpecialType: isSpecialType,
            DefaultValue: defaultValue,
            DefaultValueDocumentation: defaultValueDocumentation,
            IsReadOnly: isReadOnly,
            IsDirect: isDirect,
            IsAttached: isAttached,
            IsAddOwner: isAddOwner,
            Framework: framework,
            Description: description,
            Category: category,
            TypeConverter: typeConverter,
            Bindable: bindable,
            Browsable: browsable,
            DesignerSerializationVisibility: designerSerializationVisibility,
            ClsCompliant: clsCompliant,
            Localizability: localizability,
            BrowsableForType: browsableForType,
            FromType: fromType,
            XmlDocumentation: xmlDocumentation,
            GetterXmlDocumentation: getterXmlDocumentation ?? propertyXmlDocumentation,
            SetterXmlDocumentation: setterXmlDocumentation,
            BindEvents: bindEventsArray,
            OnChanged: onChanged ?? string.Empty,
            AffectsMeasure: affectsMeasure,
            AffectsArrange: affectsArrange,
            AffectsParentMeasure: affectsParentMeasure,
            AffectsParentArrange: affectsParentArrange,
            AffectsRender: affectsRender,
            Inherits: inherits,
            OverridesInheritanceBehavior: overridesInheritanceBehavior,
            NotDataBindable: notDataBindable,
            Journal: journal,
            SubPropertiesDoNotAffectRender: subPropertiesDoNotAffectRender,
            IsAnimationProhibited: isAnimationProhibited,
            DefaultUpdateSourceTrigger: defaultUpdateSourceTrigger,
            DefaultBindingMode: defaultBindingMode,
            EnableDataValidation: enableDataValidation,
            Coerce: coerce,
            Validate: validate,
            CreateDefaultValueCallback: createDefaultValueCallback,
            IsChanged0: c0,
            IsChanged1: c1,
            IsChanged2: c2,
            IsChanged3: c3,
            IsChangedArgs1: ca1,
            IsChangedArgs2: ca2,
            IsChanging0: ch0,
            IsChanging1: ch1,
            IsChanging2: ch2,
            IsChanging3: ch3);
    }

    private static (bool Has0, bool Has1, bool Has2, bool Has3, bool HasArgs1, bool HasArgs2) CheckMethodsDirectly(
        INamedTypeSymbol classSymbol, string methodName, string targetType, string senderType)
    {
        bool has0 = false, has1 = false, has2 = false, has3 = false, hasArgs1 = false, hasArgs2 = false;

        foreach (var member in classSymbol.GetMembers(methodName))
        {
            if (member is not IMethodSymbol method) continue;

            var p = method.Parameters;
            switch (p.Length)
            {
                case 0:
                    has0 = true;
                    break;

                case 1:
                    var type0 = GetNormalizedTypeName(p[0].Type);
                    if (type0 == targetType || type0 == senderType) has1 = true;
                    if (IsEventArgsType(p[0].Type.Name)) hasArgs1 = true;
                    break;

                case 2:
                    var type02 = GetNormalizedTypeName(p[0].Type);
                    var type12 = GetNormalizedTypeName(p[1].Type);
                    if ((type02 == targetType && type12 == targetType) || (type02 == senderType && type12 == targetType)) has2 = true;
                    if (IsEventArgsType(p[1].Type.Name)) hasArgs2 = true;
                    break;

                case 3:
                    var type03 = GetNormalizedTypeName(p[0].Type);
                    var type13 = GetNormalizedTypeName(p[1].Type);
                    var type23 = GetNormalizedTypeName(p[2].Type);
                    if (type03 == senderType && type13 == targetType && type23 == targetType) has3 = true;
                    break;
            }
        }

        return (has0, has1, has2, has3, hasArgs1, hasArgs2);
    }

    private static string GetNormalizedTypeName(ITypeSymbol typeSymbol) =>
        typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", string.Empty)
            .TrimEnd('?');

    private static bool IsEventArgsType(string typeName) =>
        typeName.EndsWith("EventArgs", StringComparison.Ordinal) ||
        typeName.EndsWith("EventArgs>", StringComparison.Ordinal) ||
        typeName.Contains("DependencyPropertyChangedEventArgs") ||
        typeName.Contains("ValueChangedEventArgs");

    private static string GenerateDependencyObjectType(Framework framework) =>
        framework == Framework.Maui ? "Microsoft.Maui.Controls.BindableObject" :
        framework == Framework.Avalonia ? "Avalonia.AvaloniaObject" :
        framework == Framework.Wpf ? "System.Windows.DependencyObject" :
        framework == Framework.Uwp || framework == Framework.Uno ? "Windows.UI.Xaml.DependencyObject" :
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

    private static bool? IsSpecialType(this ITypeSymbol? symbol)
    {
        return symbol switch
        {
            null => null,
            _ => symbol is IArrayTypeSymbol || symbol.SpecialType != SpecialType.None ||
                 (symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && symbol.BaseType != null &&
                  symbol.BaseType.SpecialType != SpecialType.None)
        };
    }

    private static string? GetNamedArgumentExpression(this AttributeSyntax attributeSyntax, string name)
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

    private static string? ExpandDefaultValueExpression(string? defaultValue, ITypeSymbol? typeSymbol)
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
