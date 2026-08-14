using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.Extensions;

/// <summary>Contains Roslyn syntax and semantic metadata for a class declaration and its attributes.</summary>
public readonly record struct ClassWithAttributesContext(
    SemanticModel SemanticModel,
    ImmutableArray<AttributeData> Attributes,
    TypeDeclarationSyntax ClassSyntax,
    INamedTypeSymbol ClassSymbol);
