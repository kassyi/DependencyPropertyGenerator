using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for adding dependency property owners.</summary>
[Generator]
public class AddOwnerGenerator : AttributeGeneratorBase<(ClassData Class, DependencyPropertyData DependencyProperty)>
{

    protected override IReadOnlyList<string> AttributeNames { get; } =
    [
        KnownAttributes.AddOwner,
        $"{KnownAttributes.AddOwner}`2"
    ];

    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "AddOwnerAttribute.g.cs",
            source: Resources.AddOwnerAttribute_cs.AsString());
    }

    protected override IReadOnlyList<Framework> SupportedFrameworks => [Framework.Avalonia, Framework.Wpf];

    protected override (ClassData Class, DependencyPropertyData DependencyProperty)? PrepareData(
        GeneratorAttributeContext context) =>
        (context.ClassData, context.GetDependencyPropertyData(isAddOwner: true));

    protected override string GenerateSource((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        SourceGenerationHelper.GenerateDependencyPropertySource(data.Class, data.DependencyProperty);

    protected override string GetHintName((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        $"{data.Class.FullName}.AddOwner.{data.DependencyProperty.Name.SanitizeFileName()}.g.cs";
}
