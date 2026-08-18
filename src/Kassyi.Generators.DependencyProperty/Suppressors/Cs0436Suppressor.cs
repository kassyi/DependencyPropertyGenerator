using System.Collections.Immutable;
using System.Linq;
using Kassyi.Generators.DependencyProperty.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Kassyi.Generators.DependencyProperty.Suppressors;

/// <summary>Suppresses CS0436 warnings caused by duplicate internal attribute helper types across friend assemblies.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class Cs0436Suppressor : DiagnosticSuppressor
{
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } =
        [DiagnosticDescriptors.Cs0436DuplicateInternalAttributeHelper];

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            if (ShouldSuppress(diagnostic))
            {
                context.ReportSuppression(Suppression.Create(DiagnosticDescriptors.Cs0436DuplicateInternalAttributeHelper, diagnostic));
            }
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
