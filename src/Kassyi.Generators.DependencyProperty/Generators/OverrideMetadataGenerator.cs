using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;


namespace Kassyi.Generators.DependencyProperty.Generators;

[Generator]
public class OverrideMetadataGenerator : IIncrementalGenerator
{
    #region Constants

    private const string Id = "OMG";

    #endregion

    #region Methods

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

        const string ns = "Kassyi.Generators.DependencyProperty.";
        const string attributeName = $"{ns}OverrideMetadataAttribute";
        var attributes = new[]
        {
            attributeName,
            $"{attributeName}`1"
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
        ((ClassWithAttributesContext context,
            Framework framework) left,
            string version) tuple)
    {
        var (((_, attributes, _, classSymbol), framework), version) = tuple;
        if (framework is not (Framework.Wpf or Framework.Uwp or Framework.WinUi or Framework.Uno or Framework.UnoWinUi))
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var overrideMetadata = attributes
            .Select(attribute => attribute.GetDependencyPropertyData(framework, version, classSymbol))
            .ToImmutableArray()
            .AsEquatableArray();

        return (classData, overrideMetadata);
    }

    private static FileWithName GetSourceCode(
        (ClassData Class, EquatableArray<DependencyPropertyData> OverrideMetada) data)
    {
        var name = data.Class.Framework is Framework.Wpf
            ? $"{data.Class.FullName}.StaticConstructor.g.cs"
            : $"{data.Class.FullName}.Methods.RegisterPropertyChangedCallbacks.g.cs";

        var writer = new SourceWriter();
        try
        {
            if (data.Class.Framework is Framework.Wpf)
            {
                SourceGenerationHelper.GenerateStaticConstructor(ref writer, data.Class, data.OverrideMetada.AsImmutableArray());
            }
            else
            {
                SourceGenerationHelper.GenerateRegisterPropertyChangedCallbacksMethod(ref writer, data.Class, data.OverrideMetada.AsImmutableArray());
            }

            return new FileWithName(
                Name: name,
                Text: writer.ToString());
        }
        finally
        {
            writer.Dispose();
        }
    }

    #endregion
}


