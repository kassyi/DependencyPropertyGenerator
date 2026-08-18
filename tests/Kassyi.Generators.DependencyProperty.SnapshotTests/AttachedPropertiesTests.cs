using Kassyi.Generators.DependencyProperty.Generators;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
[TestCategory(TestCategoryNames.Attached)]
public class AttachedPropertyTests : SnapshotTestBase
{
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-001")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task Enum(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            public enum Mode
            {
                Mode1,
                Mode2,
            }

            [AttachedDependencyProperty<Mode, {{FrameworkTestData.GetTreeView(framework)}}>("Mode", DefaultValue = Mode.Mode2)]
            public static partial class TreeViewExtensions
            {
                static partial void OnModeChanged({{FrameworkTestData.GetTreeView(framework)}} sender, Mode oldValue, Mode newValue)
                {
                }
            }
            """, framework, additionalGenerators: new StaticConstructorGenerator());
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-002")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task AttachedReadOnlyProperty(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty<object, Grid>("AttachedReadOnlyProperty", IsReadOnly = true)]
            public static partial class GridExtensions
            {
            }
            """, framework);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-003")]
    public async Task AttachedPropertyAccessors_UseBrowsableForType()
    {
        var source = GetHeader(Framework.Wpf, "Controls") + $$"""

                                                            [AttachedDependencyProperty<bool, Grid>("GeneratedTest")]
                                                            public static partial class TestProps
                                                            {
                                                            }
                                                            """;
        var generated = await GenerateSourceAsync<AttachedDependencyPropertyGenerator>(source, Framework.Wpf);

        Assert.IsTrue(generated.Contains("SetGeneratedTest(global::System.Windows.Controls.Grid element, bool value)"));
        Assert.IsTrue(generated.Contains("GetGeneratedTest(global::System.Windows.Controls.Grid element)"));
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-004")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task BindEvent(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, string.Empty, "Input") + $$"""

            [AttachedDependencyProperty<object, {{FrameworkTestData.GetUIElement(framework)}}>("BindEventProperty", BindEvent = nameof({{FrameworkTestData.GetUIElement(framework)}}.{{FrameworkTestData.GetBindEventPropertyName(framework)}}))]
            public static partial class UIElementExtensions
            {
                private static void OnBindEventPropertyChanged_{{FrameworkTestData.GetBindEventPropertyName(framework)}}(object? sender, {{FrameworkTestData.GetKeyEventArgsType(framework)}} args)
                {
                }
            }
            """, framework, additionalGenerators: new StaticConstructorGenerator());
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-005")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task AttachedPropertyWithoutSecondType(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework) + $$"""

            [AttachedDependencyProperty<object>("SomeProperty")]
            public static partial class GridExtensions
            {
            }
            """, framework);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-006")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task MultilineDescription(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty<string, Grid>("UserAgentSuffix",
            	Description = @"A suffix that is added to the default user agent, surrounded by square brackets.
            Can be used to identify the web view as belonging to a certain app/version on the server side.")]
            public static partial class GridExtensions
            {
            }

            """, framework, skipE2EValidation: true);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-007")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task CustomOnChangedAttached(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty<int, Grid>("RowCount", OnChanged = nameof(OnRowCountChanged), DefaultValue = -1)]
            public static partial class GridHelpers
            {
                static void OnRowCountChanged(Grid grid, int newValue)
                {
                }
            }
            """, framework, additionalGenerators: new StaticConstructorGenerator());
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-008")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task SameClassAsTypeParameter(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty<Test, Grid>("TestProp", OnChanged = nameof(TestChanged))]
            public partial class Test
            {
                private static void TestChanged(Grid grid, Test? newValue)
                {
                }
            }
            """, framework, additionalGenerators: new StaticConstructorGenerator());
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-009")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task InheritClass(Framework framework)
    {
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, string.Empty, "Controls") + $$"""

            [AttachedDependencyProperty<int, {{FrameworkTestData.GetFrameworkElement(framework)}}>("MyColumn")]
            public partial class MyGird : Grid
            {
            }
            """, framework, additionalGenerators: new StaticConstructorGenerator());
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Attached}-010")]
    [DataRow(Framework.Wpf)]
    public async Task AttachedOnChangedWithEventArgs(Framework framework)
    {
        var source = GetHeader(framework, "Controls") + $$"""

                                                        [AttachedDependencyProperty<string>("Test", OnChanged = nameof(OnTestChanged))]
                                                        public static partial class TestHelper
                                                        {
                                                            private static void OnTestChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                                                            {
                                                            }
                                                        }
                                                        """;
        var generated = await GenerateSourceAsync<AttachedDependencyPropertyGenerator>(source, framework);

        Assert.IsTrue(generated.Contains("propertyChangedCallback: static (sender, args) =>"));
        Assert.IsTrue(generated.Contains("OnTestChanged("));
        Assert.IsTrue(generated.Contains("(global::System.Windows.DependencyObject)sender,"));
        Assert.IsTrue(generated.Contains("args);"));
    }
}
