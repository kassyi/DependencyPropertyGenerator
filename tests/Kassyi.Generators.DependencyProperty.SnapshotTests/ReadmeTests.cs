using Kassyi.Generators.DependencyProperty.Generators;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
[TestCategory(TestCategoryNames.Doc)]
public class ReadmeTests : SnapshotTestBase
{
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Doc}-001")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task ReadmeExample(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty<bool>("IsSpinning", DefaultValue = true, Category = "Category", Description = "Description")]
            public partial class MyControl : UserControl
            {
                // Optional
                partial void OnIsSpinningChanged(bool oldValue, bool newValue)
                {
                }
            }

            [AttachedDependencyProperty<object, TreeView>("SelectedItem", DefaultBindingMode = DefaultBindingMode.TwoWay)]
            public static partial class TreeViewExtensions
            {
                // Optional
                static partial void OnSelectedItemChanged(TreeView sender, object? oldValue, object? newValue)
                {
                }
            }
            """, framework, additionalGenerators: [new AttachedDependencyPropertyGenerator(), new StaticConstructorGenerator()]);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Doc}-002")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task DescriptionWithCref(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty<bool>("IsSpinning", Description = "<see cref=\"Style.TargetType\"/> must be Label.")]
            public partial class MyControl : UserControl
            {
            }

            """, framework);
    }
}
