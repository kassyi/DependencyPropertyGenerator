using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for attached dependency properties.</summary>
[Generator]
public class AttachedDependencyPropertyGenerator : AttributeGeneratorBase<(ClassData Class, DependencyPropertyData DependencyProperty)>
{
    protected override string Id => "ADPG";

    protected override IReadOnlyList<string> AttributeNames { get; } =
    [
        KnownAttributes.AttachedDependencyProperty,
        $"{KnownAttributes.AttachedDependencyProperty}`1",
        $"{KnownAttributes.AttachedDependencyProperty}`2"
    ];

    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "AttachedDependencyPropertyAttribute.g.cs",
            source: Resources.AttachedDependencyPropertyAttribute_cs.AsString());
    }

    protected override (ClassData Class, DependencyPropertyData DependencyProperty)? PrepareData(
        in GeneratorAttributeContext context)
    {
        var dependencyPropertyData = context.Attribute.GetDependencyPropertyData(
            context.Framework,
            context.Version,
            context.ClassSymbol,
            context.ClassSyntax.TryFindAttributeSyntax(context.Attribute),
            isAttached: true,
            semanticModel: context.SemanticModel);

        return (context.ClassData, dependencyPropertyData);
    }

    protected override string GenerateSource((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        SourceGenerationHelper.GenerateAttachedDependencyPropertySource(data.Class, data.DependencyProperty);

    protected override string GetHintName((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        $"{data.Class.FullName}.AttachedProperties.{data.DependencyProperty.Name.SanitizeFileName()}.g.cs";
}
