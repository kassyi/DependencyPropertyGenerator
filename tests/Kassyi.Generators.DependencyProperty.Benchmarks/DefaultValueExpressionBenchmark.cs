using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Benchmarks;

[MemoryDiagnoser]
public class DefaultValueExpressionBenchmark
{
    private ImplicitObjectCreationExpressionSyntax _implicitNew = null!;
    private string _typeString = "global::System.Collections.Generic.List<string>";

    [GlobalSetup]
    public void Setup()
    {
        var expr = SyntaxFactory.ParseExpression("new(1, 2, 3)");
        _implicitNew = (ImplicitObjectCreationExpressionSyntax)expr;
    }

    [Benchmark(Baseline = true)]
    public string RoslynAstMutation()
    {
        var typeName = SyntaxFactory.ParseTypeName(_typeString);
        var explicitNew = SyntaxFactory.ObjectCreationExpression(
            _implicitNew.NewKeyword,
            typeName,
            _implicitNew.ArgumentList,
            _implicitNew.Initializer);
            
        return explicitNew.NormalizeWhitespace().ToFullString();
    }

    [Benchmark]
    public string StringInterpolation()
    {
        return $"{_implicitNew.GetLeadingTrivia().ToFullString()}new {_typeString}{_implicitNew.ArgumentList.ToFullString()}{_implicitNew.Initializer?.ToFullString() ?? ""}{_implicitNew.GetTrailingTrivia().ToFullString()}";
    }
}
