using System.ComponentModel;
using System.Security;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    private static void GenerateAttribute(ref SourceWriter writer, string name)
    {
        writer.AppendLine($"        [global::{name}]");
    }

    private static void GenerateAttribute(ref SourceWriter writer, string name, string? value)
    {
        if (value != null)
        {
            writer.AppendLine($"        [global::{name}({value})]");
        }
    }

    private static void GenerateComponentModelAttribute(ref SourceWriter writer, string name, string? value)
    {
        GenerateAttribute(ref writer, $"System.ComponentModel.{name}", value);
    }

    private static void GenerateCategoryAttribute(ref SourceWriter writer, string? value)
    {
        if (value != null)
        {
            GenerateComponentModelAttribute(ref writer, nameof(DependencyPropertyData.Category), $"\"{value}\"");
        }
    }

    private static void GenerateDescriptionAttribute(ref SourceWriter writer, string? value)
    {
        if (value == null) return;
        
        var isMultilineString =
            value.Contains('\r') ||
            value.Contains('\n');

        GenerateComponentModelAttribute(
            ref writer,
            nameof(DependencyPropertyData.Description),
            isMultilineString
                ? $"@\"{SecurityElement.Escape(value)}\""
                : $"\"{SecurityElement.Escape(value)}\"");
    }

    private static void GenerateTypeConverterAttribute(ref SourceWriter writer, string? value)
    {
        if (value != null)
        {
            GenerateComponentModelAttribute(ref writer, nameof(DependencyPropertyData.TypeConverter),
                $"typeof({value.WithGlobalPrefix()})");
        }
    }

    private static string ToBooleanKeyword(this bool value)
    {
        return value
            ? "true"
            : "false";
    }

    private static void GenerateBindableAttribute(ref SourceWriter writer, bool? value)
    {
        GenerateComponentModelAttribute(
            ref writer,
            nameof(DependencyPropertyData.Bindable),
            value?.ToBooleanKeyword());
    }

    private static void GenerateBrowsableAttribute(ref SourceWriter writer, bool? value)
    {
        GenerateComponentModelAttribute(
            ref writer,
            nameof(DependencyPropertyData.Browsable),
            value?.ToBooleanKeyword());
    }

    private static void GenerateDesignerSerializationVisibilityAttribute(ref SourceWriter writer, string? value)
    {
        if (value != null)
        {
            GenerateComponentModelAttribute(ref writer, nameof(DependencyPropertyData.DesignerSerializationVisibility),
                $"global::System.ComponentModel.{nameof(DesignerSerializationVisibility)}.{value}");
        }
    }

    private static void GenerateClsCompliantAttribute(ref SourceWriter writer, bool? value)
    {
        GenerateAttribute(ref writer, "System.CLSCompliant", value?.ToBooleanKeyword());
    }

    private static void GenerateGeneratedCodeAttribute(ref SourceWriter writer, string version)
    {
        GenerateAttribute(ref writer, "System.CodeDom.Compiler.GeneratedCode",
            $"\"DependencyPropertyGenerator\", \"{version}\"");
    }

    private static void GenerateExcludeFromCodeCoverageAttribute(ref SourceWriter writer)
    {
        GenerateAttribute(ref writer, "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage");
    }

    private static void GenerateLocalizabilityAttribute(ref SourceWriter writer, string? value, Framework framework)
    {
        if (value == null || framework != Framework.Wpf) return;

        GenerateAttribute(
            ref writer,
            "System.Windows.Localizability",
            $"global::System.Windows.LocalizationCategory.{value}");
    }

    private static void GenerateBrowsableForTypeAttribute(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property.Framework != Framework.Wpf) return;

        GenerateAttribute(
            ref writer,
            "System.Windows.AttachedPropertyBrowsableForType",
            $"typeof({GenerateBrowsableForType(property)})");
    }
}

