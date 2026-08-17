using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class DynamicSyntaxTests
{
    [TestMethod]
    public void Verify_PartialProperty_Init_Set_Conflict()
    {
        var code = """
using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
    public class RequiredMemberAttribute : Attribute {}
    public class CompilerFeatureRequiredAttribute : Attribute { public CompilerFeatureRequiredAttribute(string name) {} }
}

public class UserControl { }

public class DependencyPropertyAttribute : Attribute {
    public DependencyPropertyAttribute(string name, Type t) {}
}

[DependencyProperty("MyProperty", typeof(int))]
public partial class MyControl : UserControl
{
    public required partial int MyProperty { get; init; }
}

public partial class MyControl
{
    public partial int MyProperty { get => 0; set { } }
}
""";
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        var syntaxTree = CSharpSyntaxTree.ParseText(code, parseOptions);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        };

        var compilation = CSharpCompilation.Create("TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = compilation.GetDiagnostics();
        
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        
        // Assert that the compilation fails because 'init' and 'set' conflict in partial properties.
        Assert.IsTrue(errors.Count > 0, 
            $"Expected compilation errors for mismatched partial property accessors, but found none. Diagnostics:\n{string.Join(Environment.NewLine, diagnostics)}");
    }
}
