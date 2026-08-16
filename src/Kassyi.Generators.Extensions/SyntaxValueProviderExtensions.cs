using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides Roslyn syntax provider extension methods for finding annotated types.</summary>
public static class SyntaxValueProviderExtensions
{
    /// <summary>Creates a provider for classes and records decorated with the specified attribute metadata name.</summary>
    public static IncrementalValuesProvider<GeneratorAttributeSyntaxContext>
        ForAttributeWithMetadataNameOfClassesAndRecords(
            this SyntaxValueProvider source,
            string fullyQualifiedMetadataName)
    {
        return source
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: fullyQualifiedMetadataName,
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax
                    {
                        AttributeLists.Count: > 0,
                    } or RecordDeclarationSyntax
                    {
                        AttributeLists.Count: > 0,
                    },
                transform: static (context, _) => context);
    }
    
    /// <summary>Transforms syntax contexts into <see cref="ClassWithAttributesContext"/> instances.</summary>
    public static IncrementalValuesProvider<ClassWithAttributesContext>
        SelectAllAttributes(
            this IncrementalValuesProvider<GeneratorAttributeSyntaxContext> source)
    {
        return source
            .Select(static (context, _) => new ClassWithAttributesContext(
                SemanticModel: context.SemanticModel,
                Attributes: context.Attributes,
                ClassSyntax: (TypeDeclarationSyntax)context.TargetNode,
                ClassSymbol: (INamedTypeSymbol)context.TargetSymbol));
    }

    /// <summary>Extracts the string name from an attribute argument syntax without allocating strings.</summary>
    private static string? ExtractArgumentName(AttributeArgumentSyntax arg)
    {
        var expr = arg.Expression;

        // [DependencyProperty("MyProperty")]
        if (expr is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return literal.Token.ValueText;
        }

        // [DependencyProperty(nameof(MyProperty))]
        if (expr is InvocationExpressionSyntax invocation &&
            invocation.Expression is IdentifierNameSyntax idName &&
            idName.Identifier.ValueText == "nameof" &&
            invocation.ArgumentList.Arguments.Count == 1)
        {
            var innerExpr = invocation.ArgumentList.Arguments[0].Expression;

            // nameof(MyProperty)
            if (innerExpr is IdentifierNameSyntax innerId)
            {
                return innerId.Identifier.ValueText;
            }
            
            // nameof(MyClass.MyProperty)
            if (innerExpr is MemberAccessExpressionSyntax memberAccess)
            {
                return memberAccess.Name.Identifier.ValueText;
            }
        }

        return null;
    }
    
    /// <summary>Finds the attribute syntax corresponding to the specified attribute data on a class declaration.</summary>
    public static AttributeSyntax? TryFindAttributeSyntax(
        this TypeDeclarationSyntax classSyntax,
        AttributeData attribute)
    {
        classSyntax = classSyntax ?? throw new ArgumentNullException(nameof(classSyntax));
        attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        // [WHY] Avoid LINQ ElementAtOrDefault to eliminate allocations in the hot syntax provider path.
        var name = attribute.ConstructorArguments is { Length: > 0 } ctorArgs
            ? ctorArgs[0].Value?.ToString()
            : null;

        if (name == null)
        {
            return null;
        }

        // [WHY] Use nested foreach instead of SelectMany/FirstOrDefault LINQ chains to eliminate allocations in the generator pipeline.
        foreach (var attributeList in classSyntax.AttributeLists)
        {
            foreach (var attr in attributeList.Attributes)
            {
                if (attr.ArgumentList is not { Arguments.Count: > 0 } argList)
                {
                    continue;
                }

                var firstArg = argList.Arguments[0];
                var argName = ExtractArgumentName(firstArg);
                if (argName == name)
                {
                    return attr;
                }
            }
        }

        return null;
    }
    
    /// <summary>Transforms syntax contexts into individual <see cref="ClassWithAttributesContext"/> entries per matched attribute.</summary>
    public static IncrementalValuesProvider<ClassWithAttributesContext>
        SelectManyAllAttributesOfCurrentClassSyntax(
            this IncrementalValuesProvider<GeneratorAttributeSyntaxContext> source)
    {
        return source
            .SelectMany(static (context, _) =>
            {
                var classSyntax = (TypeDeclarationSyntax)context.TargetNode;
                var classSymbol = (INamedTypeSymbol)context.TargetSymbol;
                
                // [WHY] Use foreach and ImmutableArray.Builder instead of LINQ Where/Select to minimize GC pressure during IDE typing.
                var builder = ImmutableArray.CreateBuilder<ClassWithAttributesContext>(context.Attributes.Length);

                foreach (var attribute in context.Attributes)
                {
                    if (classSyntax.TryFindAttributeSyntax(attribute) != null)
                    {
                        builder.Add(new ClassWithAttributesContext(
                            SemanticModel: context.SemanticModel,
                            Attributes: [attribute],
                            ClassSyntax: classSyntax,
                            ClassSymbol: classSymbol));
                    }
                }

                return builder.ToImmutable();
            });
    }
    
    /// <summary>Resolves the assembly or generator version from MSBuild properties.</summary>
    public static IncrementalValueProvider<string> DetectVersion(
        this IncrementalGeneratorInitializationContext context)
    {
        var defaultVersion = $"{Assembly.GetCallingAssembly().GetName().Version}";
        
        return context.AnalyzerConfigOptionsProvider
            .Select<AnalyzerConfigOptionsProvider, string>((options, _) =>
                options.GetGlobalOption("Version", prefix: "RecognizeFramework") ?? defaultVersion);
    }
}
