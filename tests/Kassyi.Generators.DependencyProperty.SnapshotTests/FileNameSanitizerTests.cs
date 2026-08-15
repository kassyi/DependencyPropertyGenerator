#nullable enable

using Kassyi.Generators.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class FileNameSanitizerTests
{
    [TestMethod]
    public void SanitizeFileName_Injective_NoCollisions()
    {
        var typeNames = new[]
        {
            "MyNamespace.MyClass",
            "MyNamespace.MyClass<T>",
            "MyNamespace.MyClass_lt_T_gt_",
            "MyNamespace.MyClass<T1, T2>",
            "MyNamespace.MyClass<T1_T2>",
            "MyNamespace.MyClass<Dictionary<string, int>>",
            "MyNamespace.MyClass<Dictionary<string_int>>"
        };

        var sanitizedSet = new HashSet<string>(StringComparer.Ordinal);
        foreach (var typeName in typeNames)
        {
            var sanitized = typeName.SanitizeFileName();
            Assert.IsTrue(sanitizedSet.Add(sanitized), $"Collision detected for {typeName} -> {sanitized}");
        }

        // Fast path: non-generic string returns exact same reference
        var nonGeneric = "RegularClass";
        Assert.AreSame(nonGeneric, nonGeneric.SanitizeFileName());
    }
}
