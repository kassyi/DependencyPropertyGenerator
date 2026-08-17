using Kassyi.Generators.DependencyProperty.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules;

/// <summary>Defines a contract for evaluating callback method signatures against expected patterns.</summary>
internal interface IMethodSignatureRule
{
    /// <summary>Evaluates the provided method symbol to determine if it matches a specific signature rule.</summary>
    void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match);
}
