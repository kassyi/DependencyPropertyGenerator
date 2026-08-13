using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

[Generator]
public class StaticConstructorGenerator : IIncrementalGenerator
{
    #region Constants

    private const string Id = "SCG";

    #endregion

    #region Methods

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                hintName: "Localizability.g.cs",
                source: Resources.Localizability_cs.AsString());
            context.AddSource(
                hintName: "DefaultBindingMode.g.cs",
                source: Resources.DefaultBindingMode_cs.AsString());
            context.AddSource(
                hintName: "SourceTrigger.g.cs",
                source: Resources.SourceTrigger_cs.AsString());
        });

        var framework = context.DetectFramework();
        var version = context.DetectVersion();



        (string Name, bool IsAttached)[] attributes =
        [
            (KnownAttributes.DependencyProperty, false),
            ($"{KnownAttributes.DependencyProperty}`1", false),
            (KnownAttributes.AttachedDependencyProperty, true),
            ($"{KnownAttributes.AttachedDependencyProperty}`1", true),
            ($"{KnownAttributes.AttachedDependencyProperty}`2", true)
        ];

        var providers = attributes
            .Select(attr => GetClassData(context, attr.Name, framework, version, attr.IsAttached));

        providers.CombineAll()
            .SelectMany(TransformToStaticConstructorData)
            .WithComparer(EqualityComparer<StaticConstructorData>.Default)
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
    }

    internal static IEnumerable<StaticConstructorData> TransformToStaticConstructorData(
        EquatableArray<(ClassData Class, DependencyPropertyData DependencyProperty)> array,
        CancellationToken _)
    {
        return array.Where(static x => x.Class.Framework is Framework.Avalonia)
                    .GroupBy(static x => x.Class, static x => x.DependencyProperty)
                    .Select(static g => new StaticConstructorData(
                        Class: g.Key,
                        Properties: g.ToImmutableArray().AsEquatableArray()));
    }

    private static (ClassData Class, DependencyPropertyData DependencyProperty)? PrepareData(
        ((ClassWithAttributesContext context,
            Framework framework) left,
            string version) tuple,
        bool isAttached)
    {
        var (((_, attributes, _, classSymbol), framework), version) = tuple;
        if (attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var dependencyPropertyData = attribute.GetDependencyPropertyData(framework, version, classSymbol, isAttached: isAttached);

        return (classData, dependencyPropertyData);
    }

    private static FileWithName GetSourceCode(StaticConstructorData data)
    {
        var writer = new SourceWriter();
        try
        {
            SourceGenerationHelper.GenerateStaticConstructor(
                ref writer,
                data.Class,
                [.. data.Properties.Where(static property => !property.IsDirect)]);
            var text = writer.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                return FileWithName.Empty;
            }

            return new FileWithName(Name: $"{data.Class.FullName}.StaticConstructor.g.cs", Text: text);
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static IncrementalValueProvider<EquatableArray<(ClassData Class, DependencyPropertyData DependencyProperty)>> GetClassData(
        IncrementalGeneratorInitializationContext context,
        string attributeName,
        IncrementalValueProvider<Framework> framework,
        IncrementalValueProvider<string> version,
        bool isAttached)
    {
        return context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords(attributeName)
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions((x, _) => PrepareData(x, isAttached: isAttached), context, Id)
            .WhereNotNull()
            .CollectAsEquatableArray();
    }
    #endregion
}
