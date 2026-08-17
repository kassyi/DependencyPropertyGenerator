using Kassyi.Generators.DependencyProperty.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules.Signatures;

/// <summary>Evaluates callback methods that take exactly one parameter (e.g., NewValue).</summary>
internal sealed class SingleParameterRule : IMethodSignatureRule
{
    public void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match)
    {
        if (method.Parameters.Length != 1)
        {
            return;
        }

        var p = method.Parameters[0];
        var type0 = SignatureRuleHelper.GetNormalizedTypeName(p.Type);
            
        if (type0 == targetType || type0 == senderType)
        {
            match.Signatures |= CallbackSignature.NewValue;
        }
            
        if (SignatureRuleHelper.IsEventArgsType(p.Type))
        {
            match.Signatures |= CallbackSignature.EventArgs;
        }
    }
}
