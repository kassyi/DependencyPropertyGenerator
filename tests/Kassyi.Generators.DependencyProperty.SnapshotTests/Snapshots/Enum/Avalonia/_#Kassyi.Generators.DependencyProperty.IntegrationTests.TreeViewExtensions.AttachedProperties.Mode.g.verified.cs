//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TreeViewExtensions.AttachedProperties.Mode.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class TreeViewExtensions
    {
        /// <summary>
        /// Identifies the Mode dependency property.<br/>
        /// Default value: Mode2
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Avalonia.AttachedProperty<global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode> ModeProperty = global::Avalonia.AvaloniaProperty.RegisterAttached<TreeViewExtensions, global::Avalonia.Controls.TreeView, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode>(name: "Mode", defaultValue: (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)1, inherits: false, defaultBindingMode: global::Avalonia.Data.BindingMode.OneWay, validate: null, coerce: null);
        /// <summary>
        /// Default value: Mode2
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetMode(global::Avalonia.Controls.TreeView element, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Default value: Mode2
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode GetMode(global::Avalonia.Controls.TreeView element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)element.GetValue(ModeProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged(global::Avalonia.Controls.TreeView treeView);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged(global::Avalonia.Controls.TreeView treeView, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnModeChanged(global::Avalonia.Controls.TreeView treeView, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);
    }
}