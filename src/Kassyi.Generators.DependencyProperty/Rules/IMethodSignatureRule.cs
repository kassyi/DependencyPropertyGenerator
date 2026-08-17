using Kassyi.Generators.DependencyProperty.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules;

internal interface IMethodSignatureRule
{
    void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match);
}
