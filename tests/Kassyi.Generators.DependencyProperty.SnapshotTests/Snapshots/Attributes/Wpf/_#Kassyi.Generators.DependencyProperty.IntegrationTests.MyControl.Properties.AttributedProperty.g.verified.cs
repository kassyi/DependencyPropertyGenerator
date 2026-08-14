//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.AttributedProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "AttributedProperty"/> dependency property.<br/>
        /// Default value: default(string)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty AttributedPropertyProperty = global::System.Windows.DependencyProperty.Register(name: "AttributedProperty", propertyType: typeof(string), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(string), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnAttributedPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnAttributedPropertyChanged((string? )args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnAttributedPropertyChanged((string? )args.OldValue, (string? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Description<br/>
        /// Default value: default(string)
        /// </summary>
        [global::System.ComponentModel.Category("Category")]
        [global::System.ComponentModel.Description("Description")]
        [global::System.ComponentModel.TypeConverter(typeof(global::System.ComponentModel.EnumConverter))]
        [global::System.ComponentModel.Bindable(true)]
        [global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        [global::System.CLSCompliant(false)]
        [global::System.Windows.Localizability(global::System.Windows.LocalizationCategory.Text)]
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public string? AttributedProperty { get => (string? )GetValue(AttributedPropertyProperty); set => SetValue(AttributedPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAttributedPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAttributedPropertyChanged(string? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAttributedPropertyChanged(string? oldValue, string? newValue);
    }
}