//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.UnsafeExtensions.AttachedProperties.CallbackPtr.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public static partial class UnsafeExtensions
    {
        /// <summary>
        /// Identifies the CallbackPtr dependency property.<br/>
        /// Default value: default(delegate* unmanaged&lt;int, void&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty CallbackPtrProperty = global::System.Windows.DependencyProperty.RegisterAttached(name: "CallbackPtr", propertyType: typeof(delegate* unmanaged<int, void> ), ownerType: typeof(UnsafeExtensions), defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(delegate* unmanaged<int, void> ), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            OnCallbackPtrChanged();
            OnCallbackPtrChanged((global::System.Windows.DependencyObject)sender);
            OnCallbackPtrChanged((global::System.Windows.DependencyObject)sender, (delegate* unmanaged<int, void> )args.NewValue);
            OnCallbackPtrChanged((global::System.Windows.DependencyObject)sender, (delegate* unmanaged<int, void> )args.OldValue, (delegate* unmanaged<int, void> )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(delegate* unmanaged&lt;int, void&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetCallbackPtr(global::System.Windows.DependencyObject element, delegate* unmanaged<int, void> value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(CallbackPtrProperty, value);
        }

        /// <summary>
        /// Default value: default(delegate* unmanaged&lt;int, void&gt;)
        /// </summary>
        [global::System.Windows.AttachedPropertyBrowsableForType(typeof(global::System.Windows.DependencyObject))]
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static delegate* unmanaged<int, void> GetCallbackPtr(global::System.Windows.DependencyObject element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (delegate* unmanaged<int, void> )element.GetValue(CallbackPtrProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnCallbackPtrChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnCallbackPtrChanged(global::System.Windows.DependencyObject dependencyObject);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnCallbackPtrChanged(global::System.Windows.DependencyObject dependencyObject, delegate* unmanaged<int, void> newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnCallbackPtrChanged(global::System.Windows.DependencyObject dependencyObject, delegate* unmanaged<int, void> oldValue, delegate* unmanaged<int, void> newValue);
    }
}