using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

/// <summary>Provides generic and framework-agnostic helper methods for building source code strings.</summary>
internal static partial class SourceGenerationHelper
{
    internal static string GenerateType(DependencyPropertyData property, bool canBeNull = false)
    {
        var value = property.Type;
        if ((canBeNull ||
             property is { Modifiers.IsValueType: false, DefaultValue: null }) &&
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

    internal static string GenerateDependencyPropertyName(DependencyPropertyData property) => property is { Modifiers.IsReadOnly: true, Framework: Framework.Wpf or Framework.Maui } ? $"{property.Name}PropertyKey" : $"{property.Name}Property";

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

    internal static string GenerateDependencyObjectType(Framework framework) => 
        framework == Framework.Maui ? GenerateTypeByPlatform(framework, "BindableObject") : GenerateTypeByPlatform(framework, framework == Framework.Avalonia ? "AvaloniaObject" : "DependencyObject");

    internal static string GenerateDefaultValue(DependencyPropertyData property)
    {
        var type = property.Type;
        if (property is { Modifiers.IsSpecialType: true, DefaultValueDocumentation: { } })
        {
            return $"({type}){property.DefaultValueDocumentation}";
        }

        return property.DefaultValue != null
            ? $"({type}){property.DefaultValue}"
            : $"default({type})";
    }

    private static readonly string[] s_cSharpKeywords =
    [
        "class", "struct", "record", "enum", "interface", "object", "string", "int", "long", "bool", "double", "float",
        "decimal", "byte", "sbyte", "short", "ushort", "uint", "ulong", "char", "void", "dynamic"
    ];

    internal static string GenerateBrowsableForType(DependencyPropertyData property) =>
        property.ComponentModel.BrowsableForType ?? GenerateDependencyObjectType(property.Framework);

    /// <summary>Generates a valid C# parameter name from a type name, escaping keywords if necessary.</summary>
    private static string GenerateBrowsableForTypeParameterName(DependencyPropertyData property)
    {
        var typeName = property.ComponentModel.BrowsableForType ?? GenerateDependencyObjectType(property.Framework);
        
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return "sender";
        }
        
        // Remove generic type arguments (e.g. "List<string>" -> "List")
        var genericIndex = typeName.IndexOf('<');
        var nameToProcess = genericIndex >= 0 ? typeName.Substring(0, genericIndex) : typeName;
        
        // Remove namespace (e.g. "System.Windows.Controls.Control" -> "Control")
        var lastDot = nameToProcess.LastIndexOf('.');
        var startIndex = lastDot >= 0 ? lastDot + 1 : 0;
        var length = nameToProcess.Length - startIndex;
        
        if (length <= 0)
        {
            return "sender";
        }

        // Convert the first character to lowercase (e.g. "Control" -> "control")
        Span<char> span = stackalloc char[length];
        nameToProcess.AsSpan(startIndex).CopyTo(span);
        
        if (char.IsLetter(span[0]))
        {
            span[0] = char.ToLowerInvariant(span[0]);
        }
        
        var name = span.ToString();
        
        // Escape C# keywords if necessary (e.g. "class" -> "@class")
        if (Array.IndexOf(s_cSharpKeywords, name) >= 0)
        {
            return "@" + name;
        }

        return name;
    }

    private const string OptionsPrefix = "global::System.Windows.FrameworkPropertyMetadataOptions.";
    internal static string GenerateOptions(DependencyPropertyData property)
    {
        var builder = new System.Text.StringBuilder();
        var hasOption = false;

        addOption(property.FrameworkMetadata.AffectsMeasure, nameof(FrameworkMetadataData.AffectsMeasure));
        addOption(property.FrameworkMetadata.AffectsArrange, nameof(FrameworkMetadataData.AffectsArrange));
        addOption(property.FrameworkMetadata.AffectsParentMeasure, nameof(FrameworkMetadataData.AffectsParentMeasure));
        addOption(property.FrameworkMetadata.AffectsParentArrange, nameof(FrameworkMetadataData.AffectsParentArrange));
        addOption(property.FrameworkMetadata.AffectsRender, nameof(FrameworkMetadataData.AffectsRender));
        addOption(property.FrameworkMetadata.Inherits, nameof(FrameworkMetadataData.Inherits));
        addOption(property.FrameworkMetadata.OverridesInheritanceBehavior, nameof(FrameworkMetadataData.OverridesInheritanceBehavior));
        addOption(property.FrameworkMetadata.NotDataBindable, nameof(FrameworkMetadataData.NotDataBindable));
        addOption(property.FrameworkMetadata.DefaultBindingMode == "TwoWay", "BindsTwoWayByDefault");
        addOption(property.FrameworkMetadata.Journal, nameof(FrameworkMetadataData.Journal));
        addOption(property.FrameworkMetadata.SubPropertiesDoNotAffectRender, nameof(FrameworkMetadataData.SubPropertiesDoNotAffectRender));

        if (!hasOption)
        {
            return "global::System.Windows.FrameworkPropertyMetadataOptions.None";
        }

        return builder.ToString();

        void addOption(bool condition, string name)
        {
            if (!condition)
            {
                return;
            }

            if (hasOption)
            {
                builder.Append(" | ");
            }

            builder.Append(OptionsPrefix).Append(name);
            hasOption = true;
        }
    }

    private static string GenerateAdditionalSetterModifier(DependencyPropertyData property)
    {
        return property is { Modifiers.IsDirect: true, Framework: Framework.Avalonia }
            ? "private "
            : property.Modifiers.IsReadOnly
                ? "protected "
                : string.Empty;
    }

    private static string GeneratePropertyModifier(DependencyPropertyData property)
    {
        if (property is { Modifiers.IsReadOnly: true, Framework: Framework.Wpf })
        {
            return "internal";
        }

        return "public";
    }

    private static void GenerateValidatePartialMethod(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        if (!property.ValidationAndCallbacks.Validate)
        {
            return;
        }

        if (property.Framework == Framework.Maui)
        {
            var senderType = property.Modifiers.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type;

            writer.AppendLine($"private static partial bool Is{property.Name}Valid({senderType} sender, {GenerateType(property, canBeNull: true)} value);");
            return;
        }

        writer.AppendLine($"private static partial bool Is{property.Name}Valid({GenerateType(property, canBeNull: true)} value);");
    }

    private static void GenerateCreateDefaultValueCallbackPartialMethod(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (!property.ValidationAndCallbacks.CreateDefaultValueCallback)
        {
            return;
        }

        writer.AppendLine($"private static partial {GenerateType(property)} Get{property.Name}DefaultValue();");
    }

    private static void GenerateOnChangedMethodDeclaration(ref SourceWriter writer, string name, DependencyPropertyData property)
    {
        var modifiers = property.Modifiers.IsAttached ? "static " : string.Empty;
        var targetParameter = property.Modifiers.IsAttached
            ? $"{GenerateBrowsableForType(property)} {GenerateBrowsableForTypeParameterName(property)}, "
            : string.Empty;
        var propertyType = GenerateType(property);

        writer.Append($"{modifiers}partial void {name}({targetParameter}{propertyType} oldValue, {propertyType} newValue)");
    }

    private static void GenerateOnChangedMethodCall(ref SourceWriter writer, string name, DependencyPropertyData property)
    {
        var targetArgument = property.Modifiers.IsAttached
            ? $"{GenerateBrowsableForTypeParameterName(property)}, "
            : string.Empty;

        writer.Append($"{name}({targetArgument}oldValue, newValue);");
    }

    private static void GenerateBindEventMethod(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property.ValidationAndCallbacks.BindEvents.IsEmpty)
        {
            return;
        }

        var type = property.Type;
        var sender = property.Modifiers.IsAttached ? GenerateBrowsableForTypeParameterName(property) : "this";

        writer.AppendLine();
        GenerateOnChangedMethodDeclaration(ref writer, $"On{property.Name}Changed_BeforeBind", property);
        writer.AppendLine(";");
        GenerateOnChangedMethodDeclaration(ref writer, $"On{property.Name}Changed_AfterBind", property);
        writer.AppendLine(";");
        writer.AppendLine();
        GenerateOnChangedMethodDeclaration(ref writer, $"On{property.Name}Changed", property);
        writer.AppendLine();
        using (writer.Scope())
        {
            GenerateOnChangedMethodCall(ref writer, $"On{property.Name}Changed_BeforeBind", property);
            writer.AppendLine();
            writer.AppendLine();
            using (writer.Scope($"if (oldValue is not default({type}))"))
            {
                foreach (var @event in property.ValidationAndCallbacks.BindEvents)
                {
                    writer.AppendLine($"{sender}.{@event} -= On{property.Name}Changed_{@event};");
                }
            }
            using (writer.Scope($"if (newValue is not default({type}))"))
            {
                foreach (var @event in property.ValidationAndCallbacks.BindEvents)
                {
                    writer.AppendLine($"{sender}.{@event} += On{property.Name}Changed_{@event};");
                }
            }
            writer.AppendLine();
            GenerateOnChangedMethodCall(ref writer, $"On{property.Name}Changed_AfterBind", property);
            writer.AppendLine();
        }
    }

    internal static Strategies.IDependencyPropertyGeneratorStrategy GeneratePropertyHeader(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.XmlDocumentation, property, isProperty: false);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);

        return Strategies.FrameworkGeneratorFactory.CreateDependencyPropertyStrategy(property.Framework);
    }

    internal static void GeneratePropertyFooter(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        GenerateOnChangedMethods(ref writer, @class, property);
        GenerateOnChangingMethods(ref writer, @class, property);
        GenerateCoercePartialMethod(ref writer, property);
        GenerateValidatePartialMethod(ref writer, @class, property);
        GenerateCreateDefaultValueCallbackPartialMethod(ref writer, property);
        GenerateBindEventMethod(ref writer, property);
    }

    /// <summary>Opens the outer envelope (namespace and class declarations) and returns a zero-allocation disposable scope to close them.</summary>
    internal static SourceWriterClassScope ClassScope(ref this SourceWriter writer, ClassData @class)
    {
        writer.AppendLine();
        writer.AppendLine("#nullable enable");
        writer.AppendLine();
        
        var hasNamespace = !string.IsNullOrWhiteSpace(@class.Namespace) && @class.Namespace != "<global namespace>";
        if (hasNamespace)
        {
            writer.AppendLine($"namespace {@class.Namespace}");
            writer.AppendLine("{");
        }

        var parentCount = 0;
        var parentArray = @class.ParentClasses.AsImmutableArray();
        if (!parentArray.IsEmpty)
        {
            for (var i = parentArray.Length - 1; i >= 0; i--)
            {
                var parent = parentArray[i];
                var parentModifiers = string.IsNullOrWhiteSpace(parent.Modifiers) ? string.Empty : parent.Modifiers;
                writer.AppendLine($"{parentModifiers}partial {parent.Keyword} {parent.NameWithTypeParameters}");
                writer.AppendLine("{");
                parentCount++;
            }
        }

        writer.AppendLine($"{GenerateModifiers(@class)}partial {@class.Keyword} {@class.NameWithTypeParameters}");
        writer.AppendLine("{");
        return new SourceWriterClassScope(writer, hasNamespace, parentCount);
    }

    internal static string GenerateEventArgsType(EventData @event) =>
        string.IsNullOrWhiteSpace(@event.Type) ? "global::System.EventArgs" : GenerateType(@event);
}

/// <summary>Provides a zero-allocation disposable scope for closing class and namespace brackets.</summary>
internal readonly ref struct SourceWriterClassScope(SourceWriter writer, bool hasNamespace, int parentCount) : IDisposable
{
    public void Dispose()
    {
        writer.AppendLine("}");
        for (var i = 0; i < parentCount; i++)
        {
            writer.AppendLine("}");
        }
        if (hasNamespace)
        {
            writer.AppendLine("}");
        }
    }
}
