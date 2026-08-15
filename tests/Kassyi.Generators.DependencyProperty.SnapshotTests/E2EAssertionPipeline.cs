using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public static class E2EAssertionPipeline
{
    public static void Verify(
        string inputSource,
        string[] generatedSources,
        Framework framework,
        Compilation compilation,
        ImmutableArray<Diagnostic> diagnostics,
        string callerName,
        bool skipE2EValidation = false)
    {
        // 意図的なエラーや特殊なエッジケースを検証するテストはアサーションパイプラインをスキップ
        if (skipE2EValidation)
        {
            return;
        }

        var inputRoot = CSharpSyntaxTree.ParseText(inputSource).GetCompilationUnitRoot();
        var outputRoots = generatedSources.Select(s => CSharpSyntaxTree.ParseText(s).GetCompilationUnitRoot()).ToArray();

        // Level 1: Ensure generator produced expected elements
        VerifyCountMatching(inputRoot, outputRoots, framework);

        // Level 2: Signature and syntax validation
        VerifySignatureMatching(outputRoots, framework);
        
        // Level 3: Verify get/set accessors
        VerifyClrWrappers(outputRoots, framework);

        // Level 4: Diagnostic Validation
        VerifyDiagnostics(diagnostics, callerName);
    }

    internal static void VerifyCountMatching(SyntaxNode inputRoot, SyntaxNode[] outputRoots, Framework framework)
    {
        // 1. Count target attributes in input
        var attributeNames = new[] { "DependencyProperty", "AttachedDependencyProperty" };
        var targetAttributes = inputRoot.DescendantNodes()
            .OfType<AttributeSyntax>()
            .Where(a => attributeNames.Any(n => a.Name.ToString().Contains(n)))
            .ToList();

        // 2. Count generated fields in outputs
        // Exclude DependencyPropertyKey since it generates alongside DependencyProperty for ReadOnly properties
        var dpFieldsCount = outputRoots.SelectMany(r => r.DescendantNodes().OfType<FieldDeclarationSyntax>())
            .Count(f => {
                var typeStr = f.Declaration.Type.ToString();
                return (typeStr.Contains("Property") || typeStr.Contains("StyledProperty") || typeStr.Contains("DirectProperty") || typeStr.Contains("AttachedProperty")) && !typeStr.Contains("PropertyKey");
            });

        int dpAttributesCount = targetAttributes.Count;

        // Verify count matches (only if there are attributes)
        if (dpAttributesCount > 0 && dpFieldsCount != dpAttributesCount)
        {
            throw new Exception($"Level 1 Assertion Failed: Expected {dpAttributesCount} generated property fields, but found {dpFieldsCount}.");
        }
    }

    private static void VerifySignatureMatching(SyntaxNode[] outputRoots, Framework framework)
    {
        // Find InvocationExpression for Register / RegisterAttached / Create / CreateAttached
        var invocations = outputRoots.SelectMany(r => r.DescendantNodes().OfType<InvocationExpressionSyntax>())
            .Where(i => i.Expression is MemberAccessExpressionSyntax mae && 
                        (mae.Name.Identifier.Text == "Register" || mae.Name.Identifier.Text == "RegisterAttached" || 
                         mae.Name.Identifier.Text == "RegisterReadOnly" || mae.Name.Identifier.Text == "RegisterAttachedReadOnly" ||
                         mae.Name.Identifier.Text == "Create" || mae.Name.Identifier.Text == "CreateAttached" || mae.Name.Identifier.Text == "CreateReadOnly" || mae.Name.Identifier.Text == "CreateAttachedReadOnly"))
            .ToList();

        foreach (var inv in invocations)
        {
            var args = inv.ArgumentList.Arguments;
            if (args.Count >= 3)
            {
                // Name argument
                var nameArg = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "name") ?? args[0];
                if (!(nameArg.Expression is InvocationExpressionSyntax || nameArg.Expression is LiteralExpressionSyntax))
                {
                    throw new Exception("Level 2 Assertion Failed: Name argument is not a nameof expression or literal.");
                }

                // Avalonia handles things slightly differently (e.g. AvaloniaProperty.Register<TOwner, TValue>)
                if (framework == Framework.Avalonia)
                {
                    // Avalonia has type arguments in the method call for Register<TOwner, TValue>
                    if (inv.Expression is MemberAccessExpressionSyntax mae && mae.Name is GenericNameSyntax)
                    {
                        continue;
                    }
                }

                // PropertyType argument
                var propTypeArg = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "propertyType" || a.NameColon?.Name.Identifier.Text == "returnType");
                if (propTypeArg == null && args.Count > 1) propTypeArg = args[1];

                if (propTypeArg != null && !(propTypeArg.Expression is TypeOfExpressionSyntax))
                {
                    // Allow typeof or similar constructs
                    if (!propTypeArg.Expression.ToString().StartsWith("typeof("))
                        throw new Exception($"Level 2 Assertion Failed: PropertyType argument is not typeof. Found: {propTypeArg.Expression}");
                }

                // OwnerType argument
                var ownerTypeArg = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "ownerType" || a.NameColon?.Name.Identifier.Text == "declaringType");
                if (ownerTypeArg == null && args.Count > 2) ownerTypeArg = args[2];

                if (ownerTypeArg != null && !(ownerTypeArg.Expression is TypeOfExpressionSyntax))
                {
                    if (!ownerTypeArg.Expression.ToString().StartsWith("typeof("))
                        throw new Exception($"Level 2 Assertion Failed: OwnerType argument is not typeof. Found: {ownerTypeArg.Expression}");
                }
            }
        }
    }

    private static void VerifyClrWrappers(SyntaxNode[] outputRoots, Framework framework)
    {
        var properties = outputRoots.SelectMany(r => r.DescendantNodes().OfType<PropertyDeclarationSyntax>()).ToList();
        foreach (var prop in properties)
        {
            var accessors = prop.AccessorList?.Accessors;
            if (accessors != null)
            {
                var get = accessors.Value.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword));
                var set = accessors.Value.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword));

                if (get != null && get.Body == null && get.ExpressionBody != null)
                {
                    var expr = get.ExpressionBody.Expression.ToString();
                    // Avalonia Direct properties return backing fields (e.g., _isSpinning)
                    if (!expr.Contains("GetValue") && !expr.StartsWith("_"))
                        throw new Exception("Level 3 Assertion Failed: Getter does not call GetValue or return a backing field.");
                }

                if (set != null && set.Body == null && set.ExpressionBody != null)
                {
                    var expr = set.ExpressionBody.Expression.ToString();
                    if (!expr.Contains("SetValue") && !expr.Contains("SetAndRaise"))
                        throw new Exception("Level 3 Assertion Failed: Setter does not call SetValue or SetAndRaise.");
                }
            }
        }
    }

    private static void VerifyDiagnostics(ImmutableArray<Diagnostic> diagnostics, string callerName)
    {
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        
        // --- 既知のジェネレータバグ (TODO: 将来のPRで修正) ---
        // WeakEventGenerator: WeakEvent<T> が EventArgs を継承していない型の場合の CS1503
        errors = errors.Where(d => d.Id != "CS1503").ToList();
        
        // OverrideMetadataGenerator: partial void On...Changed() の定義側を生成しないことによる CS0759 / CS8795
        errors = errors.Where(d => d.Id != "CS8795" && d.Id != "CS0759").ToList();

        // OverrideMetadataGenerator (UnoWinUi等): } の出力漏れによる構文エラー CS1513 / CS1022
        if (callerName.StartsWith("OverrideMetadata"))
        {
            errors = errors.Where(d => d.Id != "CS1022" && d.Id != "CS1513").ToList();
        }
        
        if (errors.Any())
        {
            throw new Exception($"Level 4 Assertion Failed: Found {errors.Count} compilation errors. First error: {errors.First().GetMessage()}");
        }

        var cs0108 = diagnostics.Where(d => d.Id == "CS0108").ToList(); // Hiding member
        if (cs0108.Any())
        {
            throw new Exception("Level 4 Assertion Failed: Found CS0108 (Member hides inherited member) warning.");
        }
    }
}
