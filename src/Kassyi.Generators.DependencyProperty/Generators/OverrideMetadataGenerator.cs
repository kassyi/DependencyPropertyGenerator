using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for overriding dependency property metadata.</summary>
[Generator]
public class OverrideMetadataGenerator : IIncrementalGenerator
{
    #region Constants

    private const string Id = "OMG";

    #endregion

    #region Methods

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                hintName: "OverrideMetadataAttribute.g.cs",
                source: Resources.OverrideMetadataAttribute_cs.AsString());
        });

        var framework = context.DetectFramework();
        var version = context.DetectVersion();

        const string AttributeName = KnownAttributes.OverrideMetadata;
        var attributes = new[]
        {
            AttributeName,
            $"{AttributeName}`1"
        };

        context.RegisterAttributeGenerator(
            framework,
            version,
            attributes,
            PrepareData,
            GetSourceCode,
            Id,
            selectMany: false);
    }

    private static (ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetada)? PrepareData(
        in GeneratorMultiAttributeContext context)
    {
        if (context.Framework is not (Framework.Wpf or Framework.Uwp or Framework.WinUi or Framework.Uno or Framework.UnoWinUi))
        {
            return null;
        }

        var overrideMetadata = context.Attributes
            .Select(attribute => attribute.GetDependencyPropertyData(
                context.Framework,
                context.Version,
                context.ClassSymbol,
                context.ClassSyntax.TryFindAttributeSyntax(attribute),
                semanticModel: context.SemanticModel))
            .ToImmutableArray()
            .AsEquatableArray();

        return (context.ClassData, overrideMetadata);
    }

    private static FileWithName GetSourceCode(
        (ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetada) data)
    {
        IGenerationStrategy strategy = data.Class.Framework is Framework.Wpf
            ? new WpfGenerationStrategy()
            : new NonWpfGenerationStrategy();

        var writer = new SourceWriter();
        try
        {
            strategy.Generate(ref writer, data.Class, data.OverrideMetada);
            
            var text = writer.ToString();

            return new FileWithName(
                Name: strategy.GetFileName(data.Class),
                Text: text);
        }
        finally
        {
            writer.Dispose();
        }
    }

    #endregion
}
