using Kassyi.Generators.DependencyProperty.Generators;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
[TestCategory(TestCategoryNames.Metadata)]
public class OverrideMetadataTests : SnapshotTestBase
{
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Metadata}-001")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task OverrideMetadata(Framework framework)
    {
        return CheckSourceAsync<OverrideMetadataGenerator>(GetHeader(framework, string.Empty, "System") + """

            [DependencyProperty<int>("AquariumSize", AffectsRender = true, DefaultValue = 10)]
            public partial class Aquarium : UIElement
            {
            }

            [OverrideMetadata<int>("AquariumSize", DefaultValue = 20)]
            public partial class TropicalAquarium : Aquarium
            {
                partial void OnAquariumSizeChanged()
                {
                }
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new DependencyPropertyGenerator());
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Metadata}-001B")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task OverrideMetadataForReadOnlyProperty(Framework framework)
    {
        return CheckSourceAsync<OverrideMetadataGenerator>(GetHeader(framework, string.Empty, "System") + """

            [DependencyProperty<int>("AquariumSize", IsReadOnly = true, DefaultValue = 10)]
            public partial class Aquarium : UIElement
            {
            }

            [OverrideMetadata<int>("AquariumSize", IsReadOnly = true, DefaultValue = 20)]
            public partial class TropicalAquarium : Aquarium
            {
                partial void OnAquariumSizeChanged()
                {
                }
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new DependencyPropertyGenerator());
    }
}
