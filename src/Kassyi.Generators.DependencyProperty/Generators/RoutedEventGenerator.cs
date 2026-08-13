using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for WPF and WinUI routed events.</summary>
[Generator]
public class RoutedEventGenerator : AttributeGeneratorBase<(ClassData Class, EventData Event)>
{
    protected override string Id => "REG";

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

    protected override (ClassData Class, EventData Event)? PrepareData(
        ((ClassWithAttributesContext context, Framework framework) left, string version) tuple)
    {
        var (((_, attributes, _, classSymbol), framework), version) = tuple;
        if (attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var eventData = attribute.GetEventData(isStaticClass: false);
        if (framework is Framework.Maui ||
            framework is not Framework.Wpf && eventData.IsAttached)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);

        return (classData, eventData);
    }

    protected override string GenerateSource((ClassData Class, EventData Event) data) =>
        SourceGenerationHelper.GenerateRoutedEventSource(data.Class, data.Event);

    protected override string GetHintName((ClassData Class, EventData Event) data)
    {
        var category = data.Event.IsAttached
            ? "AttachedEvents"
            : "Events";
        return $"{data.Class.FullName}.{category}.{data.Event.Name}.g.cs";
    }
}
