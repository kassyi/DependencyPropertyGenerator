using System.Net;
using System.Security;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    private static readonly char[] s_separator = ['\r', '\n'];

    private static void GenerateXmlDocumentationFrom(ref SourceWriter writer, string value)
    {
        var lines = value.Split(s_separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            writer.AppendLine($"/// {line}");
        }
    }

    internal static void GenerateXmlDocumentationFrom(
        ref SourceWriter writer,
        string? value,
        DependencyPropertyData property,
        bool isProperty)
    {
        var name = property.Modifiers.IsAttached
            ? property.Name
            : $"<see cref=\"{property.Name}\"/>";
        var body = isProperty
            ? property.ComponentModel.Description != null ? $"{SecurityElement.Escape(property.ComponentModel.Description)}<br/>" : ""
            : $"Identifies the {name} dependency property.<br/>";
            
        if (value != null)
        {
            GenerateXmlDocumentationFrom(ref writer, value);
            return;
        }

        writer.AppendLine("/// <summary>");
        writer.LineIf(body.Length > 0, $"/// {body}");
        var defaultDoc = property.DefaultValueDocumentation?.ExtractSimpleName() ?? $"default({WebUtility.HtmlEncode(property.ShortType)})";
        writer.AppendLine($"/// Default value: {defaultDoc}");
        writer.AppendLine("/// </summary>");
    }

    internal static void GenerateXmlDocumentationFrom(ref SourceWriter writer, string? value, EventData @event)
    {
        if (value != null)
        {
            GenerateXmlDocumentationFrom(ref writer, value);
            return;
        }

        writer.AppendLine("/// <summary>");
        writer.LineIf(!string.IsNullOrWhiteSpace(@event.Description), $"/// {@event.Description}");
        writer.AppendLine("/// </summary>");
    }
}

