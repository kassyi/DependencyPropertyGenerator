using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Kassyi.Generators.DependencyProperty.Suppressors;

/// <summary>Suppresses CS0436 warnings caused by duplicate internal attribute helper types across friend assemblies.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class Cs0436Suppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor s_descriptor = new(
        id: "DPG0436",
        suppressedDiagnosticId: "CS0436",
        justification: "DependencyPropertyGenerator emits internal attribute helper types into each compilation; duplicate friend-assembly copies are expected.");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = [s_descriptor];

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics.Where(ShouldSuppress))
        {
            context.ReportSuppression(Suppression.Create(s_descriptor, diagnostic));
        }
    }

    private static bool ShouldSuppress(Diagnostic diagnostic)
    {
        if (diagnostic.Id != "CS0436")
        {
            return false;
        }

        var message = diagnostic.GetMessage(System.Globalization.CultureInfo.InvariantCulture);

        return message.Contains(@"Kassyi.Generators.DependencyProperty\Kassyi.Generators.DependencyProperty.", StringComparison.Ordinal) ||
               message.Contains("Kassyi.Generators.DependencyProperty/Kassyi.Generators.DependencyProperty.", StringComparison.Ordinal);
    }
}
