using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for overriding dependency property metadata.</summary>
[Generator]
public class OverrideMetadataGenerator : MultiAttributeGeneratorBase<(ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetadata)>
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

    protected override (ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetadata)? PrepareData(
        GeneratorMultiAttributeContext context)
    {
        var builder = ImmutableArray.CreateBuilder<DependencyPropertyData>(context.Attributes.Length);
        foreach (var attribute in context.Attributes)
        {
            builder.Add(context.GetDependencyPropertyData(attribute));
        }

        return (Class: context.ClassData, OverrideMetadata: builder.MoveToImmutable().AsEquatableArray());
    }

    private static readonly IGenerationStrategy s_wpfStrategy = new WpfGenerationStrategy();
    private static readonly IGenerationStrategy s_nonWpfStrategy = new NonWpfGenerationStrategy();

    protected override string GenerateSource((ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetadata) data)
    {
        var strategy = data.Class.Framework is Framework.Wpf
            ? s_wpfStrategy
            : s_nonWpfStrategy;

        var writer = new SourceWriter();
        try
        {
            strategy.Generate(ref writer, data.Class, data.OverrideMetadata);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    protected override string GetHintName((ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetadata) data)
    {
        var strategy = data.Class.Framework is Framework.Wpf
            ? s_wpfStrategy
            : s_nonWpfStrategy;

        return strategy.GetFileName(data.Class);
    }
}
