//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.Properties.Property.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class Generatable
    {
        /// <summary>
        /// Identifies the <see cref = "Property"/> dependency property.<br/>
        /// Default value: default(int?)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty PropertyProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "Property", propertyType: typeof(int? ), ownerType: typeof(Generatable), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default(int? ), propertyChangedCallback: static (sender, args) =>
        {
            ((Generatable)sender).OnPropertyChanged();
            ((Generatable)sender).OnPropertyChanged((int? )args.NewValue);
            ((Generatable)sender).OnPropertyChanged((int? )args.OldValue, (int? )args.NewValue);
        }));
        /// <summary>
        /// Default value: default(int?)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int? Property { get => (int? )GetValue(PropertyProperty); set => SetValue(PropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanged(int? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanged(int? oldValue, int? newValue);
    }
}