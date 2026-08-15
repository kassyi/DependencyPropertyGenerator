//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium.Properties.AquariumSize.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Aquarium
    {
        /// <summary>
        /// Identifies the <see cref = "AquariumSize"/> dependency property.<br/>
        /// Default value: 10
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Avalonia.StyledProperty<int> AquariumSizeProperty = global::Avalonia.AvaloniaProperty.Register<Aquarium, int>(name: "AquariumSize", defaultValue: (int)10, inherits: false, defaultBindingMode: global::Avalonia.Data.BindingMode.OneWay, validate: null, coerce: null);
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
    }
}