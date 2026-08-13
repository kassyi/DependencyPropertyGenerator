using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

[Generator]
public class WeakEventGenerator : AttributeGeneratorBase<(ClassData Class, EventData Event)>
{
    protected override string Id => "WEG";

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

    protected override (ClassData Class, EventData Event)? PrepareData(
        ((ClassWithAttributesContext context, Framework framework) left, string version) tuple)
    {
        var (((_, attributes, _, classSymbol), framework), version) = tuple;
        if (framework is not (Framework.Maui or Framework.Wpf) ||
            attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var eventData = attribute.GetEventData(isStaticClass: classData.IsStatic);

        return (classData, eventData);
    }

    protected override void GenerateCode(ref SourceWriter writer, (ClassData Class, EventData Event) data) =>
        SourceGenerationHelper.GenerateWeakEvent(ref writer, data.Class, data.Event);

    protected override string GetHintName((ClassData Class, EventData Event) data) =>
        $"{data.Class.FullName}.WeakEvents.{data.Event.Name}.g.cs";
}
