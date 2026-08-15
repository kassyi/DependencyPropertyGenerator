using Kassyi.Generators.DependencyProperty.Generators;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class AddOwnerTests : SnapshotTestBase
{
    [TestMethod]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task AddOwner(Framework framework)
    {
        return CheckSourceAsync<AddOwnerGenerator>(GetHeader(framework, string.Empty, "Media", "Controls") + """

            [AddOwner<Brush, Border>(nameof(Border.Background))]
            public partial class UnrelatedStateControl : UIElement
            {
            }
            """, framework);
    }

    [TestMethod]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task AddOwner2(Framework framework)
    {
        return CheckSourceAsync<AddOwnerGenerator>(GetHeader(framework, string.Empty, "Controls") + """

            [AddOwner<string, TextBox>(nameof(TextBox.Text))]
            public partial class UnrelatedStateControl : UIElement
            {
            }
            """, framework);
    }
}

