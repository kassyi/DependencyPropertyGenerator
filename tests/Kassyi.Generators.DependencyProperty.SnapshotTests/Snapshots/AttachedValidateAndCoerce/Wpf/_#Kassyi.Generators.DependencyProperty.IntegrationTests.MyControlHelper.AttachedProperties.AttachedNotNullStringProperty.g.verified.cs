//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControlHelper.AttachedProperties.AttachedNotNullStringProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public static partial class MyControlHelper
    {
        /// <summary>
        /// Identifies the AttachedNotNullStringProperty dependency property.<br/>
        /// Default value: ""
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty AttachedNotNullStringPropertyProperty = global::System.Windows.DependencyProperty.RegisterAttached(name: "AttachedNotNullStringProperty", propertyType: typeof(string), ownerType: typeof(MyControlHelper), defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: (string)"", flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            OnAttachedNotNullStringPropertyChanged();
            OnAttachedNotNullStringPropertyChanged((global::System.Windows.Controls.UserControl)sender);
            OnAttachedNotNullStringPropertyChanged((global::System.Windows.Controls.UserControl)sender, (string)args.NewValue);
            OnAttachedNotNullStringPropertyChanged((global::System.Windows.Controls.UserControl)sender, (string)args.OldValue, (string)args.NewValue);
        }, coerceValueCallback: static (sender, value) => CoerceAttachedNotNullStringProperty((global::System.Windows.Controls.UserControl)sender, (string? )value), isAnimationProhibited: false), validateValueCallback: static value => IsAttachedNotNullStringPropertyValid((string? )value));
        /// <summary>
        /// Default value: ""
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetAttachedNotNullStringProperty(global::System.Windows.Controls.UserControl element, string value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(AttachedNotNullStringPropertyProperty, value);
        }

        /// <summary>
        /// Default value: ""
        /// </summary>
        [global::System.Windows.AttachedPropertyBrowsableForType(typeof(global::System.Windows.Controls.UserControl))]
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static string GetAttachedNotNullStringProperty(global::System.Windows.Controls.UserControl element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (string)element.GetValue(AttachedNotNullStringPropertyProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged(global::System.Windows.Controls.UserControl userControl);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged(global::System.Windows.Controls.UserControl userControl, string newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged(global::System.Windows.Controls.UserControl userControl, string oldValue, string newValue);
        private static partial string CoerceAttachedNotNullStringProperty(global::System.Windows.Controls.UserControl userControl, string? value);
        private static partial bool IsAttachedNotNullStringPropertyValid(string? value);
    }
}