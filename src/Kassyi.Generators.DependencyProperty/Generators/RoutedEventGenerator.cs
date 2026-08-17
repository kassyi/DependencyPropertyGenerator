using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for WPF and WinUI routed events.</summary>
[Generator]
public class RoutedEventGenerator : AttributeGeneratorBase<(ClassData Class, EventData Event)>
{

    protected override IReadOnlyList<string> AttributeNames { get; } =
    [
        KnownAttributes.RoutedEvent,
        $"{KnownAttributes.RoutedEvent}`1"
    ];

    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "RoutedEventAttribute.g.cs",
            source: Resources.RoutedEventAttribute_cs.AsString());
        context.AddSource(
            hintName: "RoutedEventStrategy.g.cs",
            source: Resources.RoutedEventStrategy_cs.AsString());
    }

    protected override IReadOnlyList<Framework> SupportedFrameworks => [
        Framework.Wpf,
        Framework.Uwp,
        Framework.WinUi,
        Framework.Uno,
        Framework.UnoWinUi,
        Framework.Avalonia
    ];

    protected override (ClassData Class, EventData Event)? PrepareData(
        GeneratorAttributeContext context)
    {
        var eventData = context.Attribute.GetEventData(isStaticClass: false);
        if (context.Framework is not Framework.Wpf && eventData.IsAttached)
        {
            return null;
        }

        return (context.ClassData, eventData);
    }

    protected override string GenerateSource((ClassData Class, EventData Event) data) =>
        SourceGenerationHelper.GenerateRoutedEventSource(data.Class, data.Event);

    protected override string GetHintName((ClassData Class, EventData Event) data)
    {
        var category = data.Event.IsAttached
            ? "AttachedEvents"
            : "Events";
        return $"{data.Class.FullName}.{category}.{data.Event.Name.SanitizeFileName()}.g.cs";
    }
}
