using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using System.Text;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
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
        var typeName = property.BrowsableForType ?? GenerateDependencyObjectType(property.Framework);
        int lastDot = typeName.LastIndexOf('.');
        int startIndex = lastDot >= 0 ? lastDot + 1 : 0;
        int length = typeName.Length - startIndex;
        
        if (length <= 0) return string.Empty;

        Span<char> span = stackalloc char[length];
        typeName.AsSpan(startIndex).CopyTo(span);
        span[0] = char.ToLowerInvariant(span[0]);
        return span.ToString();
    }

    private static string ToLowerFirstChar(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;
        Span<char> span = stackalloc char[name.Length];
        name.AsSpan().CopyTo(span);
        span[0] = char.ToLowerInvariant(span[0]);
        return span.ToString();
    }

    private const string OptionsPrefix = "global::System.Windows.FrameworkPropertyMetadataOptions.";
    private static string GenerateOptions(DependencyPropertyData property)
    {
        var sb = new StringBuilder();
        void AppendOption(bool condition, string name)
        {
            if (condition)
            {
                if (sb.Length > 0)
                {
                    _ = sb.Append(" | ");
                }
                _ = sb.Append(OptionsPrefix);
                _ = sb.Append(name);
            }
        }

        AppendOption(property.AffectsMeasure, nameof(DependencyPropertyData.AffectsMeasure));
        AppendOption(property.AffectsArrange, nameof(DependencyPropertyData.AffectsArrange));
        AppendOption(property.AffectsParentMeasure, nameof(DependencyPropertyData.AffectsParentMeasure));
        AppendOption(property.AffectsParentArrange, nameof(DependencyPropertyData.AffectsParentArrange));
        AppendOption(property.AffectsRender, nameof(DependencyPropertyData.AffectsRender));
        AppendOption(property.Inherits, nameof(DependencyPropertyData.Inherits));
        AppendOption(property.OverridesInheritanceBehavior, nameof(DependencyPropertyData.OverridesInheritanceBehavior));
        AppendOption(property.NotDataBindable, nameof(DependencyPropertyData.NotDataBindable));
        AppendOption(property.DefaultBindingMode == "TwoWay", "BindsTwoWayByDefault");
        AppendOption(property.Journal, nameof(DependencyPropertyData.Journal));
        AppendOption(property.SubPropertiesDoNotAffectRender, nameof(DependencyPropertyData.SubPropertiesDoNotAffectRender));

        return sb.Length == 0 ? "global::System.Windows.FrameworkPropertyMetadataOptions.None" : sb.ToString();
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

