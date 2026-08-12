using System.Net;
using System.Security;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    private static readonly char[] Separator = ['\r', '\n'];

    private static void GenerateXmlDocumentationFrom(ref SourceWriter writer, string value)
    {
        var lines = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            writer.AppendLine($"        /// {line}");
        }
    }

    private static void GenerateXmlDocumentationFrom(
        ref SourceWriter writer,
        string? value,
        DependencyPropertyData property,
        bool isProperty)
    {
        var name = property.IsAttached
            ? property.Name
            : $"<see cref=\"{property.Name}\"/>";
        var body = isProperty
            ? property.Description != null ? $"{SecurityElement.Escape(property.Description)}<br/>" : ""
            : $"Identifies the {name} dependency property.<br/>";
            
        if (value != null)
        {
            GenerateXmlDocumentationFrom(ref writer, value);
            return;
        }

        writer.AppendLine("        /// <summary>");
        writer.LineIf(body.Length > 0, $"        /// {body}");
        var defaultDoc = property.DefaultValueDocumentation?.ExtractSimpleName() ?? $"default({WebUtility.HtmlEncode(property.ShortType)})";
        writer.AppendLine($"        /// Default value: {defaultDoc}");
        writer.AppendLine("        /// </summary>");
    }

    private static void GenerateXmlDocumentationFrom(ref SourceWriter writer, string? value, EventData @event)
    {
        if (value != null)
        {
            GenerateXmlDocumentationFrom(ref writer, value);
            return;
        }

        writer.AppendLine("        /// <summary>");
        writer.LineIf(!string.IsNullOrWhiteSpace(@event.Description), $"        /// {@event.Description}");
        writer.AppendLine("        /// </summary>");
    }
}

