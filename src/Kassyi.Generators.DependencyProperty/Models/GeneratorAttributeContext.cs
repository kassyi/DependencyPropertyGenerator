using System.Collections.Immutable;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Encapsulates generator execution context for a single attribute.</summary>
public readonly record struct GeneratorAttributeContext(
    SemanticModel SemanticModel,
    AttributeData Attribute,
    TypeDeclarationSyntax ClassSyntax,
    INamedTypeSymbol ClassSymbol,
    Framework Framework,
    string Version,
    ClassData ClassData);

/// <summary>Encapsulates generator execution context for multiple attributes on a class.</summary>
public readonly record struct GeneratorMultiAttributeContext(
    SemanticModel SemanticModel,
    ImmutableArray<AttributeData> Attributes,
    TypeDeclarationSyntax ClassSyntax,
    INamedTypeSymbol ClassSymbol,
    Framework Framework,
    string Version,
    ClassData ClassData);
