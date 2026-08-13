using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Strategy interface for generating platform-specific dependency property code.</summary>
public interface IGenerationStrategy
{
    /// <summary>Gets the generated output file name for the class.</summary>
    string GetFileName(ClassData classData);

    /// <summary>Generates source code using the provided writer and metadata.</summary>
    void Generate(ref SourceWriter writer, ClassData classData, EquatableArray<DependencyPropertyData> overrideMetadata);
}

/// <summary>WPF-specific source generation strategy for static constructor metadata.</summary>
public class WpfGenerationStrategy : IGenerationStrategy
{
    /// <inheritdoc />
    public string GetFileName(ClassData classData) => $"{classData.FullName}.StaticConstructor.g.cs";

    /// <inheritdoc />
    public void Generate(ref SourceWriter writer, ClassData classData, EquatableArray<DependencyPropertyData> overrideMetadata) =>
        SourceGenerationHelper.GenerateStaticConstructor(ref writer, classData, overrideMetadata.AsImmutableArray());
}

/// <summary>Non-WPF source generation strategy for property changed callback registrations.</summary>
public class NonWpfGenerationStrategy : IGenerationStrategy
{
    /// <inheritdoc />
    public string GetFileName(ClassData classData) => $"{classData.FullName}.Methods.RegisterPropertyChangedCallbacks.g.cs";

    /// <inheritdoc />
    public void Generate(ref SourceWriter writer, ClassData classData, EquatableArray<DependencyPropertyData> overrideMetadata) =>
        SourceGenerationHelper.GenerateRegisterPropertyChangedCallbacksMethod(ref writer, classData, overrideMetadata.AsImmutableArray());
}
