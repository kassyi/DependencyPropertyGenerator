using Avalonia.Controls;

namespace Kassyi.Generators.DependencyProperty.IntegrationTests;

[TestClass]
[TestCategory(TestCategoryNames.Integration)]
public class DependencyPropertyGeneratorTests
{
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Integration}-001")]
    [TestCategory($"{TestCategoryNames.Integration}-002")]
    [TestCategory($"{TestCategoryNames.Integration}-003")]
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
