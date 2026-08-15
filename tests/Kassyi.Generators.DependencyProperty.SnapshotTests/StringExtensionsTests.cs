#nullable enable

using Kassyi.Generators.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    [DataRow("string", "@string")]
    [DataRow("class", "@class")]
    [DataRow("int", "@int")]
    [DataRow("event", "@event")]
    [DataRow("MyProperty", "MyProperty")]
    [DataRow("value", "value")]
    public void EscapeKeyword_ShouldEscapeCSharpKeywords(string input, string expected)
    {
        Assert.AreEqual(expected, input.EscapeKeyword());
    }

    [TestMethod]
    [DataRow("myProperty", "MyProperty")]
    [DataRow("MyProperty", "MyProperty")]
    public void ToPropertyName_ShouldConvertToPascalCase(string input, string expected)
    {
        Assert.AreEqual(expected, input.ToPropertyName());
    }

    [TestMethod]
    [DataRow("MyProperty", "myProperty")]
    [DataRow("Class", "@class")]
    [DataRow("Event", "@event")]
    public void ToParameterName_ShouldConvertToCamelCaseAndEscape(string input, string expected)
    {
        Assert.AreEqual(expected, input.ToParameterName());
    }

    [TestMethod]
    [DataRow("Line1\n   \nLine3", "Line1\nLine3")]
    [DataRow("Line1\r\n\t\r\nLine3", "Line1\nLine3")]
    [DataRow("\n\nLine1", "\n\nLine1")] // Keeps pure empty lines (length 0) but removes lines with only spaces
    public void RemoveBlankLinesWhereOnlyWhitespaces_ShouldRemoveSpacesOnlyLines(string input, string expected)
    {
        var result = input.RemoveBlankLinesWhereOnlyWhitespaces();
        // Adjust for line endings normalization output
        Assert.AreEqual(expected.Replace("\r\n", "\n"), result);
    }
}
