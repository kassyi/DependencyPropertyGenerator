using Microsoft.CodeAnalysis;
using Kassyi.Generators.DependencyProperty.Models;

namespace Kassyi.Generators.DependencyProperty.Rules.Signatures;

internal sealed class TripleParameterRule : IMethodSignatureRule
{
    public void Evaluate(IMethodSymbol method, string targetType, string senderType, MethodSignatureMatch match)
    {
        if (method.Parameters.Length == 3)
        {
            var p0 = method.Parameters[0];
            var p1 = method.Parameters[1];
            var p2 = method.Parameters[2];
            
            var type0 = SignatureRuleHelper.GetNormalizedTypeName(p0.Type);
            var type1 = SignatureRuleHelper.GetNormalizedTypeName(p1.Type);
            var type2 = SignatureRuleHelper.GetNormalizedTypeName(p2.Type);
            
            if (type0 == senderType && type1 == targetType && type2 == targetType)
            {
                match.Has3 = true;
            }
        }
    }
}
