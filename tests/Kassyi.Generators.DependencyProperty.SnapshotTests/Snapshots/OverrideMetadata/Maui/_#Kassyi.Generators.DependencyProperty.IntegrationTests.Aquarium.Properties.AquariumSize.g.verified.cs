//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium.Properties.AquariumSize.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class Aquarium
    {
        /// <summary>
        /// Identifies the <see cref = "AquariumSize"/> dependency property.<br/>
        /// Default value: 10
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty AquariumSizeProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "AquariumSize", returnType: typeof(int), declaringType: typeof(Aquarium), defaultValue: (int)10, defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((Aquarium)sender).OnAquariumSizeChanged();
            ((Aquarium)sender).OnAquariumSizeChanged((int)newValue);
            ((Aquarium)sender).OnAquariumSizeChanged((int)oldValue, (int)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((Aquarium)sender).OnAquariumSizeChanging();
            ((Aquarium)sender).OnAquariumSizeChanging((int)newValue);
            ((Aquarium)sender).OnAquariumSizeChanging((int)oldValue, (int)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: 10
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int AquariumSize { get => (int)GetValue(AquariumSizeProperty); set => SetValue(AquariumSizeProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumSizeChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumSizeChanged(int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumSizeChanged(int oldValue, int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumSizeChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumSizeChanging(int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumSizeChanging(int oldValue, int newValue);
    }
}