using Avalonia.Controls;

namespace Kassyi.Generators.DependencyProperty.IntegrationTests;

[TestClass]
public class DependencyPropertyGeneratorTests
{
    [TestMethod]
    public void GeneratesCorrectly()
    {
        var window = new MyControl();
        window.SetValue(MyControl.IsSpinningProperty, false);
        window.GetValue(MyControl.IsSpinningProperty).Should().BeFalse();
        window.IsChanged.Should().BeTrue();

        var treeView = new TreeView();
        TreeViewExtensions.SetSelectedItem(treeView, new object());
        TreeViewExtensions.GetSelectedItem(treeView).Should().NotBeNull();
    }
}
