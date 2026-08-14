//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyHelper.AttachedProperties.MyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public static partial class MyHelper
    {
        /// <summary>
        /// Identifies the MyProperty dependency property.<br/>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty MyPropertyProperty = global::System.Windows.DependencyProperty.RegisterAttached(name: "MyProperty", propertyType: typeof(int), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyHelper), defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(int), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            OnMyPropertyChanged();
            OnMyPropertyChanged((DependencyObject)sender);
            OnMyPropertyChanged((DependencyObject)sender, (int)args.NewValue);
            OnMyPropertyChanged((DependencyObject)sender, (int)args.OldValue, (int)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetMyProperty(DependencyObject element, int value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(MyPropertyProperty, value);
        }

        /// <summary>
        /// Default value: default(int)
        /// </summary>
        [global::System.Windows.AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static int GetMyProperty(DependencyObject element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (int)element.GetValue(MyPropertyProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnMyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnMyPropertyChanged(DependencyObject dependencyObject);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnMyPropertyChanged(DependencyObject dependencyObject, int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnMyPropertyChanged(DependencyObject dependencyObject, int oldValue, int newValue);
    }
}