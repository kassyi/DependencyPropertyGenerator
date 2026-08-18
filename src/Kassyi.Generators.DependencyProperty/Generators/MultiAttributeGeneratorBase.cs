using Kassyi.Generators.DependencyProperty.Diagnostics;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Abstract base class for attribute-driven incremental source generators that process multiple attributes per class.</summary>
public abstract class MultiAttributeGeneratorBase<TData> : IIncrementalGenerator
    where TData : struct
{
    /// <summary>Gets the metadata names of the attributes that trigger this generator.</summary>
    protected abstract IReadOnlyList<string> AttributeNames { get; }
    
    /// <summary>Gets the UI frameworks supported by this generator.</summary>
    protected virtual IReadOnlyList<Framework> SupportedFrameworks => [];

    /// <summary>Gets a value indicating whether to process multiple attributes on the same class individually.</summary>
    protected virtual bool SelectMany => false;

    /// <summary>Registers static source files during post-initialization.</summary>
    protected abstract void PostInitialize(IncrementalGeneratorPostInitializationContext context);

    /// <summary>Prepares the data model required for source generation from the matched attribute contexts.</summary>
    protected abstract TData? PrepareData(GeneratorMultiAttributeContext context);
    
    /// <summary>Generates the C# source code text based on the prepared data.</summary>
    protected abstract string GenerateSource(TData data);

    /// <summary>Gets the hint name for the generated source file.</summary>
    protected abstract string GetHintName(TData data);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitialize);

        var framework = context.DetectFramework(DiagnosticDescriptors.FrameworkNotRecognized);
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
                return PrepareData(multiCtx);
            },
            GetSourceCode,
            selectMany: SelectMany);
    }

    private FileWithName GetSourceCode(TData data)
    {
        var text = GenerateSource(data);
        return string.IsNullOrWhiteSpace(text) ? FileWithName.Empty 
            : new FileWithName(Name: GetHintName(data), Text: text);
    }
}
