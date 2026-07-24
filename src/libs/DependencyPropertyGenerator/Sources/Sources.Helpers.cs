using H.Generators.Extensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace H.Generators;

internal static partial class Sources
{
    private static string GenerateType(DependencyPropertyData property, bool canBeNull = false)
    {
        var value = property.Type;
        if ((canBeNull ||
             property is { IsValueType: false, DefaultValue: null }) &&
            !value.EndsWith("?", StringComparison.Ordinal))
        {
            value += "?";
        }

        return value;
    }

    private static string GenerateType(EventData @event, bool nullable = true)
    {
        var value = @event.Type;
        if (nullable && !@event.IsValueType)
        {
            value += "?";
        }

        return value;
    }

    private static string GenerateDependencyPropertyName(DependencyPropertyData property)
    {
        if (property is { IsReadOnly: true, Framework: Framework.Wpf or Framework.Maui })
        {
            return $"{property.Name}PropertyKey";
        }

        return $"{property.Name}Property";
    }

    private static string GenerateTypeByPlatform(Framework framework, string name)
    {
        return (framework switch
        {
            Framework.Wpf => $"System.Windows.{name}",
            Framework.Uwp or Framework.Uno => $"Windows.UI.Xaml.{name}",
            Framework.WinUi or Framework.UnoWinUi => $"Microsoft.UI.Xaml.{name}",
            Framework.Avalonia => $"Avalonia.{name}",
            Framework.Maui => $"Microsoft.Maui.Controls.{name}",
            _ => throw new InvalidOperationException("Platform is not supported."),
        }).WithGlobalPrefix();
    }

    private static string GenerateDependencyObjectType(Framework framework)
    {
        if (framework == Framework.Maui)
        {
            return GenerateTypeByPlatform(framework, "BindableObject");
        }

        if (framework == Framework.Avalonia)
        {
            return GenerateTypeByPlatform(framework, "AvaloniaObject");
        }

        return GenerateTypeByPlatform(framework, "DependencyObject");
    }

    private static string GenerateDefaultValue(DependencyPropertyData property)
    {
        var type = property.Type;
        if (property is { IsSpecialType: true, DefaultValueDocumentation: { } })
        {
            return $"({type}){property.DefaultValueDocumentation}";
        }

        return property.DefaultValue != null
            ? $"({type}){property.DefaultValue}"
            : $"default({type})";
    }

    private static string GenerateBrowsableForType(DependencyPropertyData property)
    {
        return property.BrowsableForType ?? GenerateDependencyObjectType(property.Framework);
    }

    private static string GenerateBrowsableForTypeParameterName(DependencyPropertyData property)
    {
        return (property.BrowsableForType ?? GenerateDependencyObjectType(property.Framework))
            .ExtractSimpleName()
            .ToParameterName();
    }

    private static string GenerateAdditionalSetterModifier(DependencyPropertyData property)
    {
        return property is { IsDirect: true, Framework: Framework.Avalonia }
            ? "private "
            : property.IsReadOnly
                ? "protected "
                : string.Empty;
    }

    private static string GeneratePropertyModifier(DependencyPropertyData property)
    {
        if (property is { IsReadOnly: true, Framework: Framework.Wpf })
        {
            return "internal";
        }

        return "public";
    }

    private static string GenerateValidatePartialMethod(ClassData @class, DependencyPropertyData property)
    {
        if (!property.Validate)
        {
            return " ";
        }

        if (property.Framework == Framework.Maui)
        {
            var senderType = property.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type;

            return $"""
                private static partial bool Is{property.Name}Valid(
                    {senderType} sender,
                    {GenerateType(property, canBeNull: true)} value);
        """.RemoveBlankLinesWhereOnlyWhitespaces();
        }

        return $"""
                private static partial bool Is{property.Name}Valid({GenerateType(property, canBeNull: true)} value);
        """.RemoveBlankLinesWhereOnlyWhitespaces();
    }

    private static string GenerateCreateDefaultValueCallbackPartialMethod(DependencyPropertyData property)
    {
        if (!property.CreateDefaultValueCallback)
        {
            return " ";
        }

        return $"""
                private static partial {GenerateType(property)} Get{property.Name}DefaultValue();
        """.RemoveBlankLinesWhereOnlyWhitespaces();
    }

    private static string GenerateOnChangedMethodDeclaration(string name, DependencyPropertyData property)
    {
        var modifiers = property.IsAttached ? "static " : string.Empty;
        var targetParameter = property.IsAttached
            ? $"\n            {GenerateBrowsableForType(property)} {GenerateBrowsableForTypeParameterName(property)},"
            : string.Empty;

        return $"""
                {modifiers}partial void {name}({targetParameter}
                    {GenerateType(property)} oldValue,
                    {GenerateType(property)} newValue)
        """.RemoveBlankLinesWhereOnlyWhitespaces();
    }

    private static string GenerateOnChangedMethodCall(string name, DependencyPropertyData property)
    {
        var targetArgument = property.IsAttached
            ? $"\n                {GenerateBrowsableForTypeParameterName(property)},"
            : string.Empty;

        return $"""
                    {name}({targetArgument}
                        oldValue,
                        newValue);
        """.RemoveBlankLinesWhereOnlyWhitespaces();
    }

    private static string GenerateBindEventMethod(DependencyPropertyData property)
    {
        if (property.BindEvents.IsEmpty)
        {
            return " ";
        }

        var type = property.Type;
        var sender = property.IsAttached ? GenerateBrowsableForTypeParameterName(property) : "this";

        var unbindEvents = property.BindEvents
            .Select(@event => $"                {sender}.{@event} -= On{property.Name}Changed_{@event};\n")
            .Inject();
        var bindEvents = property.BindEvents
            .Select(@event => $"                {sender}.{@event} += On{property.Name}Changed_{@event};\n")
            .Inject();

        var beforeBindDecl = GenerateOnChangedMethodDeclaration($"On{property.Name}Changed_BeforeBind", property);
        var afterBindDecl = GenerateOnChangedMethodDeclaration($"On{property.Name}Changed_AfterBind", property);
        var onChangedDecl = GenerateOnChangedMethodDeclaration($"On{property.Name}Changed", property);
        var beforeBindCall = GenerateOnChangedMethodCall($"On{property.Name}Changed_BeforeBind", property);
        var afterBindCall = GenerateOnChangedMethodCall($"On{property.Name}Changed_AfterBind", property);

        return $$"""

        {{beforeBindDecl}};
        {{afterBindDecl}};

        {{onChangedDecl}}
                {
        {{beforeBindCall}}

                    if (oldValue is not default({{type}}))
                    {
        {{unbindEvents}}
                    }
                    if (newValue is not default({{type}}))
                    {
        {{bindEvents}}
                    }

        {{afterBindCall}}
                }
        """.RemoveBlankLinesWhereOnlyWhitespaces();
    }

    private const string OptionsPrefix = "global::System.Windows.FrameworkPropertyMetadataOptions.";

    private static readonly (Func<DependencyPropertyData, bool> Condition, string Name)[] OptionMappings =
    [
        (static p => p.AffectsMeasure, OptionsPrefix + nameof(DependencyPropertyData.AffectsMeasure)),
        (static p => p.AffectsArrange, OptionsPrefix + nameof(DependencyPropertyData.AffectsArrange)),
        (static p => p.AffectsParentMeasure, OptionsPrefix + nameof(DependencyPropertyData.AffectsParentMeasure)),
        (static p => p.AffectsParentArrange, OptionsPrefix + nameof(DependencyPropertyData.AffectsParentArrange)),
        (static p => p.AffectsRender, OptionsPrefix + nameof(DependencyPropertyData.AffectsRender)),
        (static p => p.Inherits, OptionsPrefix + nameof(DependencyPropertyData.Inherits)),
        (static p => p.OverridesInheritanceBehavior, OptionsPrefix + nameof(DependencyPropertyData.OverridesInheritanceBehavior)),
        (static p => p.NotDataBindable, OptionsPrefix + nameof(DependencyPropertyData.NotDataBindable)),
        (static p => p.DefaultBindingMode == "TwoWay", OptionsPrefix + "BindsTwoWayByDefault"),
        (static p => p.Journal, OptionsPrefix + nameof(DependencyPropertyData.Journal)),
        (static p => p.SubPropertiesDoNotAffectRender, OptionsPrefix + nameof(DependencyPropertyData.SubPropertiesDoNotAffectRender)),
    ];

    private static string GenerateOptions(DependencyPropertyData property)
    {
        var values = new List<string>(capacity: OptionMappings.Length);
        foreach (var (condition, name) in OptionMappings)
        {
            if (condition(property))
            {
                values.Add(name);
            }
        }

        return values.Count == 0
            ? "global::System.Windows.FrameworkPropertyMetadataOptions.None"
            : string.Join(" | ", values);
    }

    private static string GeneratePropertyType(ClassData @class, DependencyPropertyData property)
    {
        if (property.Framework == Framework.Maui)
        {
            return GenerateTypeByPlatform(
                property.Framework,
                property.IsReadOnly
                    ? "BindablePropertyKey"
                    : "BindableProperty");
        }

        if (property.Framework == Framework.Avalonia)
        {
            return property.IsDirect
                ? GenerateTypeByPlatform(
                    property.Framework,
                    $"DirectProperty<{@class.Type}, {GenerateType(property)}>")
                : property.IsAttached
                    ? GenerateTypeByPlatform(
                        property.Framework,
                        $"AttachedProperty<{GenerateType(property)}>")
                    : GenerateTypeByPlatform(
                        property.Framework,
                        $"StyledProperty<{GenerateType(property)}>");
        }

        return GenerateTypeByPlatform(property.Framework, property is { IsReadOnly: true, Framework: Framework.Wpf } ? "DependencyPropertyKey" : "DependencyProperty");
    }
    
    private static string GenerateEventArgsType(EventData @event)
    {
        return string.IsNullOrWhiteSpace(@event.Type) ? "global::System.EventArgs" : GenerateType(@event);
    }
}
