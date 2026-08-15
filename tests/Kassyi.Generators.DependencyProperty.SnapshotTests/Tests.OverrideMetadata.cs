using Kassyi.Generators.DependencyProperty.Generators;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class OverrideMetadataTests : SnapshotTestBase
{
    [TestMethod]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task OverrideMetadata(Framework framework)
    {
        return CheckSourceAsync<OverrideMetadataGenerator>(GetHeader(framework, string.Empty, "System") + """

            [DependencyProperty<Uri>("AquariumGraphic", AffectsRender = true,
                DefaultValueExpression = "new System.Uri(\"http://www.contoso.com/aquarium-graphic.jpg\")")]
            public partial class Aquarium : UIElement
            {
            }

            [OverrideMetadata<Uri>("AquariumGraphic",
                DefaultValueExpression = "new System.Uri(\"http://www.contoso.com/tropical-aquarium-graphic.jpg\")")]
            public partial class TropicalAquarium : Aquarium
            {
                partial void OnAquariumGraphicChanged()
                {
                }
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new DependencyPropertyGenerator());
    }

    [TestMethod]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task OverrideMetadataForReadOnlyProperty(Framework framework)
    {
        return CheckSourceAsync<OverrideMetadataGenerator>(GetHeader(framework, string.Empty, "System") + """

            [DependencyProperty<Uri>("AquariumGraphic", IsReadOnly = true,
                DefaultValueExpression = "new System.Uri(\"http://www.contoso.com/aquarium-graphic.jpg\")")]
            public partial class Aquarium : UIElement
            {
            }

            [OverrideMetadata<Uri>("AquariumGraphic", IsReadOnly = true,
                DefaultValueExpression = "new System.Uri(\"http://www.contoso.com/tropical-aquarium-graphic.jpg\")")]
            public partial class TropicalAquarium : Aquarium
            {
                partial void OnAquariumGraphicChanged()
                {
                }
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new DependencyPropertyGenerator());
    }
    [TestMethod]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Avalonia)]
    public Task OverrideMetadataWithOldAndNewValue_EmitsError(Framework framework)
    {
        return CheckSourceAsync<OverrideMetadataGenerator>(GetHeader(framework, string.Empty, "System") + """

            [DependencyProperty<int>("AquariumSize", IsReadOnly = true, DefaultValue = 10)]
            public partial class Aquarium : UIElement
            {
            }

            [OverrideMetadata<int>("AquariumSize", IsReadOnly = true, DefaultValue = 20)]
            public partial class TropicalAquarium : Aquarium
            {
                partial void OnAquariumSizeChanged(int oldValue, int newValue)
                {
                }
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new DependencyPropertyGenerator());
    }
}


