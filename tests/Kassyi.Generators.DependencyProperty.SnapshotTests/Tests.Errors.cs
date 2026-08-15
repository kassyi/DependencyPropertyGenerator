using Kassyi.Generators.DependencyProperty.Generators;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class ErrorTests : SnapshotTestBase
{
    [TestMethod]
    [DataRow(Framework.None)]
    public Task NoneFramework(Framework framework)
    {
        return CheckSourceAsync<WeakEventGenerator>(GetHeader(framework, "Controls") + """

            [WeakEvent<string>("UrlChanged", IsStatic = true)]
            public partial class MyControl : UserControl
            {
            }
            """, framework, skipE2EValidation: true);
    }

    [TestMethod]
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

    [TestMethod]
    [DataRow(Framework.Wpf)]
    public async Task AttachedCustomOnChangedUnsupportedSignature(Framework framework)
    {
        var source = GetHeader(framework, "Controls") + """

            [AttachedDependencyProperty<string>("Test", OnChanged = nameof(OnTestChanged))]
            public static partial class TestHelper
            {
                // Unsupported signature (3 parameters, not conforming to any known pattern)
                private static void OnTestChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, int extra)
                {
                }
            }
            """;
        var generated = await GenerateSourceAsync<AttachedDependencyPropertyGenerator>(source, framework);

        Assert.IsTrue(generated.Contains("#error DPG0001: The specified OnChanged method 'OnTestChanged' was not found or has an unsupported signature on 'Kassyi.Generators.DependencyProperty.IntegrationTests.TestHelper'."));
        Assert.IsTrue(generated.Contains("propertyChangedCallback: null"));
    }
}

