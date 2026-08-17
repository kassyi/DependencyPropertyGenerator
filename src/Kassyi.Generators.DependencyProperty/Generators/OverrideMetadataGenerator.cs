using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for overriding dependency property metadata.</summary>
[Generator]
public class OverrideMetadataGenerator : MultiAttributeGeneratorBase<(ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetada)>
{
    protected override string Id => "OMG";

    protected override IReadOnlyList<string> AttributeNames { get; } =
    [
        KnownAttributes.OverrideMetadata,
        $"{KnownAttributes.OverrideMetadata}`1"
    ];

    protected override IReadOnlyList<Framework> SupportedFrameworks => [
        Framework.Wpf,
        Framework.Uwp,
        Framework.WinUi,
        Framework.Uno,
        Framework.UnoWinUi
    ];

    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "OverrideMetadataAttribute.g.cs",
            source: Resources.OverrideMetadataAttribute_cs.AsString());
    }

    protected override (ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetada)? PrepareData(
        GeneratorMultiAttributeContext context)
    {
        var overrideMetadata = context.Attributes
            .Select(attribute => context.GetDependencyPropertyData(attribute))
            .ToImmutableArray()
            .AsEquatableArray();

        return (Class: context.ClassData, OverrideMetada: overrideMetadata);
    }

    protected override string GenerateSource((ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetada) data)
    {
        IGenerationStrategy strategy = data.Class.Framework is Framework.Wpf
            ? new WpfGenerationStrategy()
            : new NonWpfGenerationStrategy();

        var writer = new SourceWriter();
        try
        {
            strategy.Generate(ref writer, data.Class, data.OverrideMetada);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    protected override string GetHintName((ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetada) data)
    {
        IGenerationStrategy strategy = data.Class.Framework is Framework.Wpf
            ? new WpfGenerationStrategy()
            : new NonWpfGenerationStrategy();
        return strategy.GetFileName(data.Class);
    }
}
