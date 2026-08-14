//HintName: MyControl_lt_T1_comma__space_T2_gt_.Properties.MyProperty.g.cs
#nullable enable
internal partial class MyControl<T1, T2>
{
    /// <summary>
    /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
    /// Default value: default(object)
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
    public static readonly global::System.Windows.DependencyProperty MyPropertyProperty = global::System.Windows.DependencyProperty.Register(name: "MyProperty", propertyType: typeof(object), ownerType: typeof(MyControl<T1, T2>), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(object), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
    {
        ((MyControl<T1, T2>)sender).OnMyPropertyChanged();
        ((MyControl<T1, T2>)sender).OnMyPropertyChanged((object? )args.NewValue);
        ((MyControl<T1, T2>)sender).OnMyPropertyChanged((object? )args.OldValue, (object? )args.NewValue);
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