using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

public abstract class AttributeGeneratorBase<TData> : IIncrementalGenerator
    where TData : struct
{
    protected abstract string Id { get; }
    protected abstract IReadOnlyList<string> AttributeNames { get; }

    protected abstract void PostInitialize(IncrementalGeneratorPostInitializationContext context);
    protected abstract TData? PrepareData(((ClassWithAttributesContext context, Framework framework) left, string version) tuple);
    protected abstract void GenerateCode(ref SourceWriter writer, TData data);
    protected abstract string GetHintName(TData data);

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
        var writer = new SourceWriter();
        try
        {
            GenerateCode(ref writer, data);
            return new FileWithName(
                Name: GetHintName(data),
                Text: writer.ToString());
        }
        finally
        {
            writer.Dispose();
        }
    }
}
