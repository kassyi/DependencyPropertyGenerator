using Microsoft.CodeAnalysis;
using Kassyi.Generators.DependencyProperty.Models;

namespace Kassyi.Generators.DependencyProperty.Rules;

internal interface IMethodSignatureRule
{
    void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match);
}
