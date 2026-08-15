using Kassyi.Generators.DependencyProperty.Generators;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class WeakEventTests : SnapshotTestBase
{
    [TestMethod]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task WeakEvent(Framework framework)
    {
        return CheckSourceAsync<WeakEventGenerator>(GetHeader(framework, "Controls") + """

            [WeakEvent("Completed")]
            public partial class MyControl : UserControl
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
    public Task WeakEventWithType(Framework framework)
    {
        return CheckSourceAsync<WeakEventGenerator>(GetHeader(framework, "Controls") + """

            [WeakEvent<string>("UrlChanged")]
            public partial class MyControl : UserControl
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
    public Task StaticWeakEvent(Framework framework)
    {
        return CheckSourceAsync<WeakEventGenerator>(GetHeader(framework, "Controls") + """

            [WeakEvent("Completed", IsStatic = true)]
            public partial class MyControl : UserControl
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
    public Task StaticWeakEventWithType(Framework framework)
    {
        return CheckSourceAsync<WeakEventGenerator>(GetHeader(framework, "Controls") + """

            [WeakEvent<string>("UrlChanged", IsStatic = true)]
            public partial class MyControl : UserControl
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
    public Task WeakEventWithEventArgsType(Framework framework)
    {
        return CheckSourceAsync<WeakEventGenerator>(GetHeader(framework, "Controls") + """

            [WeakEvent<System.EventArgs>("Changed")]
            public partial class MyControl : UserControl
            {
            }
            """, framework);
    }
}

