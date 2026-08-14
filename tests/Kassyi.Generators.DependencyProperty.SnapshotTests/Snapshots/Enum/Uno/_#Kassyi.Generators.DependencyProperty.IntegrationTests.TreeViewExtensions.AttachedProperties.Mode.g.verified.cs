//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TreeViewExtensions.AttachedProperties.Mode.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public static partial class TreeViewExtensions
    {
        /// <summary>
        /// Identifies the Mode dependency property.<br/>
        /// Default value: Mode2
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Windows.UI.Xaml.DependencyProperty ModeProperty = global::Windows.UI.Xaml.DependencyProperty.RegisterAttached(name: "Mode", propertyType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode), ownerType: typeof(TreeViewExtensions), new global::Windows.UI.Xaml.PropertyMetadata(defaultValue: (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)1, propertyChangedCallback: static (sender, args) =>
        {
            OnModeChanged();
            OnModeChanged((global::Windows.UI.Xaml.Controls.TreeView)sender);
            OnModeChanged((global::Windows.UI.Xaml.Controls.TreeView)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)args.NewValue);
            OnModeChanged((global::Windows.UI.Xaml.Controls.TreeView)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)args.OldValue, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)args.NewValue);
        }));
        /// <summary>
        /// Default value: Mode2
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetMode(global::Windows.UI.Xaml.Controls.TreeView element, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Default value: Mode2
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode GetMode(global::Windows.UI.Xaml.Controls.TreeView element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)element.GetValue(ModeProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged(global::Windows.UI.Xaml.Controls.TreeView treeView);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged(global::Windows.UI.Xaml.Controls.TreeView treeView, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged(global::Windows.UI.Xaml.Controls.TreeView treeView, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);
    }
}