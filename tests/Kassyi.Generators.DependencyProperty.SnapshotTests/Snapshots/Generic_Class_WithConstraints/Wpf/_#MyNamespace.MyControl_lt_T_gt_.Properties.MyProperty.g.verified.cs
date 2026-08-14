//HintName: MyNamespace.MyControl_lt_T_gt_.Properties.MyProperty.g.cs
#nullable enable
namespace MyNamespace
{
    partial class MyControl<T>
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
        /// Default value: default(object)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty MyPropertyProperty = global::System.Windows.DependencyProperty.Register(name: "MyProperty", propertyType: typeof(object), ownerType: typeof(global::MyNamespace.MyControl<T>), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(object), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::MyNamespace.MyControl<T>)sender).OnMyPropertyChanged();
            ((global::MyNamespace.MyControl<T>)sender).OnMyPropertyChanged((object? )args.NewValue);
            ((global::MyNamespace.MyControl<T>)sender).OnMyPropertyChanged((object? )args.OldValue, (object? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(object)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public object? MyProperty { get => (object? )GetValue(MyPropertyProperty); set => SetValue(MyPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(object? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(object? oldValue, object? newValue);
    }
}