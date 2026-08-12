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

        const string ns = "Kassyi.Generators.DependencyProperty.";
        const string adp1 = $"{ns}AttachedDependencyPropertyAttribute";
        const string dp1 = $"{ns}DependencyPropertyAttribute";

        var attributes = new (string Name, bool IsAttached)[]
        {
            (dp1, false),
            ($"{dp1}`1", false),
            (adp1, true),
            ($"{adp1}`1", true),
            ($"{adp1}`2", true)
        };

        var combined = GetClassData(context, attributes[0].Name, framework, version, attributes[0].IsAttached);
        for (var i = 1; i < attributes.Length; i++)
        {
            combined = combined
                .Combine(GetClassData(context, attributes[i].Name, framework, version, attributes[i].IsAttached))
                .Select(static (x, _) => x.Left.AsImmutableArray().AddRange(x.Right.AsImmutableArray()).AsEquatableArray());
        }

        combined
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
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

    private static EquatableArray<FileWithName> GetSourceCode(
        EquatableArray<(ClassData Class, DependencyPropertyData DependencyProperty)> values)
    {
        if (values.AsImmutableArray().IsDefaultOrEmpty)
        {
            return ImmutableArray<FileWithName>.Empty.AsEquatableArray();
        }

        return values
            .Where(static x => x.Class.Framework is Framework.Avalonia)
            .GroupBy(static x => x.Class, static x => x.DependencyProperty)
            .Select(static g =>
            {
                var writer = new SourceWriter();
                try
                {
                    SourceGenerationHelper.GenerateStaticConstructor(
                        ref writer,
                        g.Key,
                        g.Where(static property => !property.IsDirect).ToArray());
                    var text = writer.ToString();
                    return string.IsNullOrWhiteSpace(text) switch
                    {
                        false => new FileWithName(Name: $"{g.Key.FullName}.StaticConstructor.g.cs", Text: text),
                        _ => FileWithName.Empty
                    };
                }
                finally
                {
                    writer.Dispose();
                }
            })
            .Where(static f => !string.IsNullOrWhiteSpace(f.Text))
            .ToImmutableArray()
            .AsEquatableArray();
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
