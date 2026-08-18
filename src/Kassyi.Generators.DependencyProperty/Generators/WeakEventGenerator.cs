using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for weak event pattern subscriptions.</summary>
[Generator]
public class WeakEventGenerator : AttributeGeneratorBase<(ClassData Class, EventData Event)>
{

    protected override IReadOnlyList<string> AttributeNames { get; } =
    [
        KnownAttributes.WeakEvent,
        $"{KnownAttributes.WeakEvent}`1"
    ];

    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "WeakEventAttribute.g.cs",
            source: Resources.WeakEventAttribute_cs.AsString());
    }

    protected override IReadOnlyList<Framework> SupportedFrameworks => [Framework.Maui, Framework.Wpf];

    protected override (ClassData Class, EventData Event)? PrepareData(GeneratorAttributeContext context)
    {
        var eventData = context.Attribute.GetEventData(isStaticClass: context.ClassData.IsStatic);

        return (context.ClassData, eventData);
    }

    protected override string GenerateSource((ClassData Class, EventData Event) data) =>
        SourceGenerationHelper.GenerateWeakEventSource(data.Class, data.Event);

    protected override string GetHintName((ClassData Class, EventData Event) data) =>
        $"{data.Class.FullName}.WeakEvents.{data.Event.Name.SanitizeFileName()}.g.cs";
}
