using Microsoft.CodeAnalysis;
using Kassyi.Generators.DependencyProperty.Models;

namespace Kassyi.Generators.DependencyProperty.Rules.Signatures;

internal sealed class DoubleParameterRule : IMethodSignatureRule
{
    public void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match)
    {
        if (method.Parameters.Length == 2)
        {
            var p0 = method.Parameters[0];
            var p1 = method.Parameters[1];
            
            var type0 = SignatureRuleHelper.GetNormalizedTypeName(p0.Type);
            var type1 = SignatureRuleHelper.GetNormalizedTypeName(p1.Type);
            
            if ((type0 == targetType || type0 == senderType) && type1 == targetType)
            {
                match.Signatures |= CallbackSignature.OldAndNewValue;
            }
            
            if (SignatureRuleHelper.IsEventArgsType(p1.Type))
            {
                match.Signatures |= CallbackSignature.SenderAndEventArgs;
            }
        }
    }
}
