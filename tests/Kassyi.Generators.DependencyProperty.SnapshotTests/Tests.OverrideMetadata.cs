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
}


