using Kassyi.Generators.DependencyProperty.Generators;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
[TestCategory(TestCategoryNames.Error)]
public class ErrorTests : SnapshotTestBase
{
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-001")]
    [DataRow(Framework.Wpf)]
    public Task AttachedCustomOnChangedUnsupportedSignature(Framework framework)
    {
        var source = GetHeader(framework, "Controls", "System.Windows") + $$"""

            [AttachedDependencyProperty<string>("Test", OnChanged = nameof(OnTestChanged))]
            public static partial class TestHelper
            {
                // Unsupported signature (3 parameters, not conforming to any known pattern)
                private static void OnTestChanged({{FrameworkTestData.GetDependencyObject(framework)}} d, DependencyPropertyChangedEventArgs e, int extra)
                {
                }
            }
            """;
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(source, framework, skipE2EValidation: true);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-002")]
    [DataRow(Framework.Wpf)]
    public Task AttachedCustomOnChangedNotFound(Framework framework)
    {
        var source = GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty<string>("Test", OnChanged = "NonExistentMethod")]
            public static partial class TestHelper
            {
            }
            """;
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(source, framework, skipE2EValidation: true);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-003")]
    [DataRow(Framework.Wpf)]
    public Task FileLocalType_EmitsError(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("MyProperty", typeof(int))]
            file partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework, skipE2EValidation: true);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-004")]
    [DataRow(Framework.Wpf)]
    public Task RefStructPropertyType_EmitsError(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System") + $$"""

            public ref struct MyRefStruct { }

            [DependencyProperty("MyProperty", typeof(MyRefStruct))]
            public partial class MyControl : UserControl
            {
            }
            """, framework, skipE2EValidation: true);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-005")]
    [DataRow(Framework.Wpf)]
    public Task ReferenceTypeDefaultValue_EmitsError(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Collections.Generic") + $$"""

            [DependencyProperty("MyProperty", typeof(List<int>), DefaultValueExpression = "new List<int>()")]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework, skipE2EValidation: true);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-006")]
    [DataRow(Framework.Wpf)]
    public Task NestedReferenceTypeDefaultValue_EmitsError(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Collections.Generic") + $$"""

            public static class ExternalConfig
            {
                public static readonly List<int> SharedList = new();
            }

            public record struct MyStruct(List<int> Items, int Id);

            [DependencyProperty("MyProperty1", typeof(MyStruct), DefaultValueExpression = "new(new List<int>(), 2)")]
            [DependencyProperty("MyProperty2", typeof(MyStruct), DefaultValueExpression = "new MyStruct { Items = [] }")]
            [DependencyProperty<List<int>>("MyProperty3", DefaultValueExpression = "[]")]
            [DependencyProperty<List<int>>("MyProperty4", DefaultValueExpression = "s_items")]
            [DependencyProperty<MyStruct>("MyProperty5", DefaultValueExpression = "new(s_items, 2)")]
            [DependencyProperty<List<int>>("MyProperty6", DefaultValueExpression = "new List<int>(")]
            [DependencyProperty<List<int>>("MyProperty7", DefaultValueExpression = "invalid expression %%%")]
            [DependencyProperty<List<int>>("MyProperty8", DefaultValueExpression = "MyControl.s_items")]
            [DependencyProperty<List<int>>("MyProperty9", DefaultValueExpression = "ExternalConfig.SharedList")]
            public partial class MyControl : UserControl
            {
                public static readonly List<int> s_items = new();
            }
            """, framework, skipE2EValidation: true);
    }
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-007")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Avalonia)]
    public Task OverrideMetadataWithOldAndNewValue_EmitsError(Framework framework)
    {
        return CheckSourceAsync<OverrideMetadataGenerator>(GetHeader(framework, string.Empty, "System") + $$"""

            [DependencyProperty<int>("AquariumSize", IsReadOnly = true, DefaultValue = 10)]
            public partial class Aquarium : {{FrameworkTestData.GetUIElement(framework)}}
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

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-008")]
    [DataRow(Framework.Wpf)]
    public Task InvalidDefaultValueExpression_EmitsError(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            public record struct MyProfile(double A, double B);

            [DependencyProperty<MyProfile>("BrokenProfile1", DefaultValueExpression = "new(1.5, 48.0")]
            [DependencyProperty<MyProfile>("BrokenProfile2", DefaultValueExpression = "new(???")]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework, skipE2EValidation: true);
            
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-009")]
    [DataRow(Framework.None)]
    public Task NoneFramework(Framework framework)
    {
        return CheckSourceAsync<WeakEventGenerator>(GetHeader(framework) + $$"""

            [WeakEvent<string>("UrlChanged", IsStatic = true)]
            public partial class MyControl
            {
            }
            """, framework, skipE2EValidation: true);
    }


    
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Error}-010")]
    [DataRow(Framework.Wpf)]
    public Task UnsupportedCallbackSignature_EmitsError(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, string.Empty) + $$"""

            [DependencyProperty<string>("MyProperty")]
            public partial class MyControl : {{FrameworkTestData.GetFrameworkElement(framework)}}
            {
                // Unsupported signature (matches name, but missing or wrong parameter types)
                partial void OnMyPropertyChanged({{FrameworkTestData.GetDependencyObject(framework)}} d, DependencyPropertyChangedEventArgs e)
                {
                }
            }
            """, framework, skipE2EValidation: true);
    }

}
