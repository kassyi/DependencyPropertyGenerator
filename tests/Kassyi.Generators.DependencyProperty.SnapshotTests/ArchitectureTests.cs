#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

/// <summary>Validates architectural constraints and code invariants across the generator source files.</summary>
[TestClass]
public class ArchitectureTests
{
    private static readonly string[] s_forbiddenMethods = ["Replace", "Substring", "Remove", "Insert", "Trim", "TrimStart", "TrimEnd"];
    private static readonly string[] s_targetDirectories = ["Models", "Rules"];

    /// <summary>Ensures semantic models and rule components do not use destructive string mutations that can corrupt type signatures.</summary>
    [TestMethod]
    [TestCategory(TestCategoryNames.Integration)]
    public void ModelsAndRules_ShouldNotUseStringMutationMethods()
    {
        var rootDir = GetSolutionRoot();
        Assert.IsNotNull(rootDir, "Could not find solution root directory.");

        var srcDir = Path.Combine(rootDir, "src", "Kassyi.Generators.DependencyProperty");
        Assert.IsTrue(Directory.Exists(srcDir), $"Source directory not found: {srcDir}");

        var violations = new List<string>();

        // [WHY] Flattens directory scanning into an O(N) streaming query to avoid nested loop overhead.
        var csFiles = s_targetDirectories
            .Select(dir => Path.Combine(srcDir, dir))
            .Where(Directory.Exists)
            .SelectMany(dir => Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories));

        // [WHY] Uses a single-pass CSharpSyntaxWalker to inspect AST invocation nodes with minimal allocations.
        foreach (var file in csFiles)
        {
            var relativePath = Path.GetRelativePath(rootDir, file);
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
            var walker = new ForbiddenMethodWalker(relativePath, violations);
            walker.Visit(tree.GetRoot());
        }

        if (violations.Count > 0)
        {
            Assert.Fail($"Destructive string methods ({string.Join(", ", s_forbiddenMethods)}) should not be used in Models or Rules to prevent type corruption during semantic analysis.\n" +
                        $"Found {violations.Count} violations:\n" +
                        string.Join(Environment.NewLine, violations));
        }
    }

    private static string? GetSolutionRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            if (File.Exists(Path.Combine(dir, "DependencyPropertyGenerator.sln")) ||
                File.Exists(Path.Combine(dir, "Kassyi.Generators.DependencyProperty.sln")))
            {
                return dir;
            }

            dir = Path.GetDirectoryName(dir);
        }

        return null;
    }

    private sealed class ForbiddenMethodWalker : CSharpSyntaxWalker
    {
        private readonly string _relativeFilePath;
        private readonly List<string> _violations;

        public ForbiddenMethodWalker(string relativeFilePath, List<string> violations)
        {
            _relativeFilePath = relativeFilePath;
            _violations = violations;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                s_forbiddenMethods.Contains(memberAccess.Name.Identifier.Text))
            {
                bool isAllowed = false;

                // [WHY] Explicitly allows TrimStart('@') because it safely sanitizes verbatim identifier keywords in property names without altering type signatures.
                if (memberAccess.Name.Identifier.Text == "TrimStart" &&
                    node.ArgumentList.Arguments.Count == 1 &&
                    node.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literal)
                {
                    if (literal.Kind() == SyntaxKind.CharacterLiteralExpression 
                        || literal.Kind() == SyntaxKind.StringLiteralExpression)
                    {
                        if (literal.Token.ValueText == "@")
                        {
                            isAllowed = true;
                        }
                    }
                }

                if (!isAllowed)
                {
                    var line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    _violations.Add($"{_relativeFilePath}:{line} -> {node}");
                }
            }

            base.VisitInvocationExpression(node);
        }
    }
}
