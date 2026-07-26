using System.Collections.Immutable;
using H.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace H.Generators;

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

        var dp1 = context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords("DependencyPropertyGenerator.DependencyPropertyAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(static (x, _) => PrepareData(x, isAttached: false), context, Id)
            .WhereNotNull()
            .CollectAsEquatableArray();

        var dp2 = context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords("DependencyPropertyGenerator.DependencyPropertyAttribute`1")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(static (x, _) => PrepareData(x, isAttached: false), context, Id)
            .WhereNotNull()
            .CollectAsEquatableArray();

        var adp1 = context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords("DependencyPropertyGenerator.AttachedDependencyPropertyAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(static (x, _) => PrepareData(x, isAttached: true), context, Id)
            .WhereNotNull()
            .CollectAsEquatableArray();

        var adp2 = context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords("DependencyPropertyGenerator.AttachedDependencyPropertyAttribute`1")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(static (x, _) => PrepareData(x, isAttached: true), context, Id)
            .WhereNotNull()
            .CollectAsEquatableArray();

        var adp3 = context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords("DependencyPropertyGenerator.AttachedDependencyPropertyAttribute`2")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(static (x, _) => PrepareData(x, isAttached: true), context, Id)
            .WhereNotNull()
            .CollectAsEquatableArray();

        dp1
            .Combine(dp2)
            .Select(static (x, _) => x.Left.AsImmutableArray().AddRange(x.Right.AsImmutableArray()).AsEquatableArray())
            .Combine(adp1)
            .Select(static (x, _) => x.Left.AsImmutableArray().AddRange(x.Right.AsImmutableArray()).AsEquatableArray())
            .Combine(adp2)
            .Select(static (x, _) => x.Left.AsImmutableArray().AddRange(x.Right.AsImmutableArray()).AsEquatableArray())
            .Combine(adp3)
            .Select(static (x, _) => x.Left.AsImmutableArray().AddRange(x.Right.AsImmutableArray()).AsEquatableArray())
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
                var text = Sources.GenerateStaticConstructor(
                    g.Key,
                    g.Where(static property => !property.IsDirect).ToArray());
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return new FileWithName(
                        Name: $"{g.Key.FullName}.StaticConstructor.g.cs",
                        Text: text);
                }
                return FileWithName.Empty;
            })
            .Where(static f => !string.IsNullOrWhiteSpace(f.Text))
            .ToImmutableArray()
            .AsEquatableArray();
    }

    #endregion
}