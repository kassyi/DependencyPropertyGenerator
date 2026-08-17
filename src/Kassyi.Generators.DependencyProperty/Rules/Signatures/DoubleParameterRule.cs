using Kassyi.Generators.DependencyProperty.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules.Signatures;

/// <summary>Evaluates callback methods that take exactly two parameters (e.g., OldValue and NewValue).</summary>
internal sealed class DoubleParameterRule : IMethodSignatureRule
{
    public void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match)
    {
        if (method.Parameters.Length != 2)
        {
            return;
        }

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
