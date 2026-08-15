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
        Assert.IsFalse((bool)window.GetValue(MyControl.IsSpinningProperty));
        Assert.IsTrue(window.IsChanged);

        var treeView = new TreeView();
        TreeViewExtensions.SetSelectedItem(treeView, new object());
        Assert.IsNotNull(TreeViewExtensions.GetSelectedItem(treeView));
    }
}
