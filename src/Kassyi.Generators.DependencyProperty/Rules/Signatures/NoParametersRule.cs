using Kassyi.Generators.DependencyProperty.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules.Signatures;

internal sealed class NoParametersRule : IMethodSignatureRule
{
    public void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match)
    {
        if (method.Parameters.Length == 0)
        {
            match.Signatures |= CallbackSignature.NoParameters;
        }
    }
}
