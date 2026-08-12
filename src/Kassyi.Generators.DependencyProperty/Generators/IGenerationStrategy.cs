using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Generators;

public interface IGenerationStrategy
{
    string GetFileName(ClassData classData);
    void Generate(ref SourceWriter writer, ClassData classData, EquatableArray<DependencyPropertyData> overrideMetadata);
}

public class WpfGenerationStrategy : IGenerationStrategy
{
    public string GetFileName(ClassData classData) => $"{classData.FullName}.StaticConstructor.g.cs";

    public void Generate(ref SourceWriter writer, ClassData classData, EquatableArray<DependencyPropertyData> overrideMetadata)
    {
        SourceGenerationHelper.GenerateStaticConstructor(ref writer, classData, overrideMetadata.AsImmutableArray());
    }
}

public class NonWpfGenerationStrategy : IGenerationStrategy
{
    public string GetFileName(ClassData classData) => $"{classData.FullName}.Methods.RegisterPropertyChangedCallbacks.g.cs";

    public void Generate(ref SourceWriter writer, ClassData classData, EquatableArray<DependencyPropertyData> overrideMetadata)
    {
        SourceGenerationHelper.GenerateRegisterPropertyChangedCallbacksMethod(ref writer, classData, overrideMetadata.AsImmutableArray());
    }
}
