using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Abstract base class for attribute-driven incremental source generators.</summary>
public abstract class AttributeGeneratorBase<TData> : IIncrementalGenerator
    where TData : struct
{
    protected abstract string Id { get; }
    protected abstract IReadOnlyList<string> AttributeNames { get; }

    protected abstract void PostInitialize(IncrementalGeneratorPostInitializationContext context);
    protected abstract TData? PrepareData(((ClassWithAttributesContext context, Framework framework) left, string version) tuple);
    
    // [WHY] AST-based generator pipeline replaces legacy text templates for zero allocations.
    protected abstract string GenerateSource(TData data);

    protected abstract string GetHintName(TData data);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitialize);

        var framework = context.DetectFramework();
        var version = context.DetectVersion();

        context.RegisterAttributeGenerator(
            framework,
            version,
            AttributeNames,
            PrepareData,
            GetSourceCode,
            Id);
    }

    private FileWithName GetSourceCode(TData data)
    {
        var text = GenerateSource(data);
        return new FileWithName(
            Name: GetHintName(data),
            Text: text);
    }
}
