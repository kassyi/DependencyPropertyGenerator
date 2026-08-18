using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Abstract base class for attribute-driven incremental source generators.</summary>
public abstract class AttributeGeneratorBase<TData> : IIncrementalGenerator
    where TData : struct
{
    /// <summary>Gets the metadata names of the attributes that trigger this generator.</summary>
    protected abstract IReadOnlyList<string> AttributeNames { get; }

    /// <summary>Registers static source files during post-initialization.</summary>
    protected abstract void PostInitialize(IncrementalGeneratorPostInitializationContext context);
    
    /// <summary>Prepares the data model required for source generation from the matched attribute contexts.</summary>
    protected abstract TData? PrepareData(GeneratorAttributeContext context);
    
    /// <summary>Generates the C# source code text based on the prepared data.</summary>
    // [WHY] AST-based generator pipeline replaces legacy text templates for zero allocations.
    protected abstract string GenerateSource(TData data);

    /// <summary>Gets the hint name for the generated source file.</summary>
    protected abstract string GetHintName(TData data);

    /// <summary>Gets the UI frameworks supported by this generator.</summary>
    protected virtual IReadOnlyList<Framework> SupportedFrameworks => [];

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitialize);

        var framework = context.DetectFramework(Diagnostics.DiagnosticDescriptors.FrameworkNotRecognized);
        var version = context.DetectVersion();
        var supported = SupportedFrameworks;

        context.RegisterAttributeGenerator(
            framework,
            version,
            AttributeNames,
            multiCtx =>
            {
                if (supported.Count > 0 && !supported.Contains(multiCtx.Framework))
                {
                    return null;
                }
                return PrepareData(multiCtx.ForFirstAttribute());
            },
            GetSourceCode);
    }

    private FileWithName GetSourceCode(TData data)
    {
        var text = GenerateSource(data);
        return new FileWithName(Name: GetHintName(data), Text: text);
    }
}
