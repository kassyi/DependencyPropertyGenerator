using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    internal static string GenerateType(DependencyPropertyData property, bool canBeNull = false)
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

    internal static string GenerateType(EventData @event, bool nullable = true)
    {
        var value = @event.Type;
        if (nullable && !@event.IsValueType)
        {
            value += "?";
        }

        return value;
    }

    internal static string GenerateDependencyPropertyName(DependencyPropertyData property)
    {
        if (property is { IsReadOnly: true, Framework: Framework.Wpf or Framework.Maui })
        {
            return $"{property.Name}PropertyKey";
        }

        return $"{property.Name}Property";
    }

    internal static string GenerateTypeByPlatform(Framework framework, string name)
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

    internal static string GenerateDependencyObjectType(Framework framework)
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

    internal static string GenerateDefaultValue(DependencyPropertyData property)
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

    internal static string GenerateBrowsableForType(DependencyPropertyData property)
    {
        return property.BrowsableForType ?? GenerateDependencyObjectType(property.Framework);
    }

    private static string GenerateBrowsableForTypeParameterName(DependencyPropertyData property)
    {
        var typeName = property.BrowsableForType ?? GenerateDependencyObjectType(property.Framework);
        int lastDot = typeName.LastIndexOf('.');
        int startIndex = lastDot >= 0 ? lastDot + 1 : 0;
        int length = typeName.Length - startIndex;
        
        if (length <= 0)
        {
            return string.Empty;
        }

        Span<char> span = stackalloc char[length];
        typeName.AsSpan(startIndex).CopyTo(span);
        span[0] = char.ToLowerInvariant(span[0]);
        return span.ToString();
    }

    private const string OptionsPrefix = "global::System.Windows.FrameworkPropertyMetadataOptions.";
    internal static string GenerateOptions(DependencyPropertyData property)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateOptions(ref writer, property);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static void GenerateOptions(ref SourceWriter writer, DependencyPropertyData property)
    {
        bool hasOption = false;
        static void AppendOption(ref SourceWriter w, ref bool ho, bool condition, string name)
        {
            if (condition)
            {
                if (ho)
                {
                    w.Append(" | ");
                }
                w.Append(OptionsPrefix);
                w.Append(name);
                ho = true;
            }
        }

        AppendOption(ref writer, ref hasOption, property.AffectsMeasure, nameof(DependencyPropertyData.AffectsMeasure));
        AppendOption(ref writer, ref hasOption, property.AffectsArrange, nameof(DependencyPropertyData.AffectsArrange));
        AppendOption(ref writer, ref hasOption, property.AffectsParentMeasure, nameof(DependencyPropertyData.AffectsParentMeasure));
        AppendOption(ref writer, ref hasOption, property.AffectsParentArrange, nameof(DependencyPropertyData.AffectsParentArrange));
        AppendOption(ref writer, ref hasOption, property.AffectsRender, nameof(DependencyPropertyData.AffectsRender));
        AppendOption(ref writer, ref hasOption, property.Inherits, nameof(DependencyPropertyData.Inherits));
        AppendOption(ref writer, ref hasOption, property.OverridesInheritanceBehavior, nameof(DependencyPropertyData.OverridesInheritanceBehavior));
        AppendOption(ref writer, ref hasOption, property.NotDataBindable, nameof(DependencyPropertyData.NotDataBindable));
        AppendOption(ref writer, ref hasOption, property.DefaultBindingMode == "TwoWay", "BindsTwoWayByDefault");
        AppendOption(ref writer, ref hasOption, property.Journal, nameof(DependencyPropertyData.Journal));
        AppendOption(ref writer, ref hasOption, property.SubPropertiesDoNotAffectRender, nameof(DependencyPropertyData.SubPropertiesDoNotAffectRender));

        if (!hasOption)
        {
            writer.Append("global::System.Windows.FrameworkPropertyMetadataOptions.None");
        }
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

    private static void GenerateValidatePartialMethod(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        if (!property.Validate)
        {
            writer.Append(" ");
            return;
        }

        if (property.Framework == Framework.Maui)
        {
            var senderType = property.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type;

            writer.Append($"""
                private static partial bool Is{property.Name}Valid(
                    {senderType} sender,
                    {GenerateType(property, canBeNull: true)} value);
        """);
            return;
        }

        writer.Append($"""
                private static partial bool Is{property.Name}Valid({GenerateType(property, canBeNull: true)} value);
        """);
    }

    private static void GenerateCreateDefaultValueCallbackPartialMethod(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (!property.CreateDefaultValueCallback)
        {
            writer.Append(" ");
            return;
        }

        writer.Append($"""
                private static partial {GenerateType(property)} Get{property.Name}DefaultValue();
        """);
    }

    private static void GenerateOnChangedMethodDeclaration(ref SourceWriter writer, string name, DependencyPropertyData property)
    {
        var modifiers = property.IsAttached ? "static " : string.Empty;
        var targetParameter = property.IsAttached
            ? $"\n            {GenerateBrowsableForType(property)} {GenerateBrowsableForTypeParameterName(property)},"
            : string.Empty;

        writer.Append($"""
                {modifiers}partial void {name}({targetParameter}
                    {GenerateType(property)} oldValue,
                    {GenerateType(property)} newValue)
        """);
    }

    private static void GenerateOnChangedMethodCall(ref SourceWriter writer, string name, DependencyPropertyData property)
    {
        var targetArgument = property.IsAttached
            ? $"\n                {GenerateBrowsableForTypeParameterName(property)},"
            : string.Empty;

        writer.Append($"""
                    {name}({targetArgument}
                        oldValue,
                        newValue);
        """);
    }

    private static void GenerateBindEventMethod(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property.BindEvents.IsEmpty)
        {
            writer.Append(" ");
            return;
        }

        var type = property.Type;
        var sender = property.IsAttached ? GenerateBrowsableForTypeParameterName(property) : "this";

        writer.AppendLine();
        GenerateOnChangedMethodDeclaration(ref writer, $"On{property.Name}Changed_BeforeBind", property);
        writer.AppendLine(";");
        GenerateOnChangedMethodDeclaration(ref writer, $"On{property.Name}Changed_AfterBind", property);
        writer.AppendLine(";");
        writer.AppendLine();
        GenerateOnChangedMethodDeclaration(ref writer, $"On{property.Name}Changed", property);
        writer.AppendLine();
        writer.AppendLine("        {");
        GenerateOnChangedMethodCall(ref writer, $"On{property.Name}Changed_BeforeBind", property);
        writer.AppendLine();
        writer.AppendLine();
        writer.AppendLine($"            if (oldValue is not default({type}))");
        writer.AppendLine("            {");
        foreach (var @event in property.BindEvents)
        {
            writer.AppendLine($"                {sender}.{@event} -= On{property.Name}Changed_{@event};");
        }
        writer.AppendLine("            }");
        writer.AppendLine($"            if (newValue is not default({type}))");
        writer.AppendLine("            {");
        foreach (var @event in property.BindEvents)
        {
            writer.AppendLine($"                {sender}.{@event} += On{property.Name}Changed_{@event};");
        }
        writer.AppendLine("            }");
        writer.AppendLine();
        GenerateOnChangedMethodCall(ref writer, $"On{property.Name}Changed_AfterBind", property);
        writer.AppendLine();
        writer.Append("        }");
    }

    private static string GeneratePropertyType(ClassData @class, DependencyPropertyData property)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(property.Framework);
        return generator.GeneratePropertyType(@class, property);
    }
    
    internal static string GenerateEventArgsType(EventData @event)
    {
        return string.IsNullOrWhiteSpace(@event.Type) ? "global::System.EventArgs" : GenerateType(@event);
    }
}

