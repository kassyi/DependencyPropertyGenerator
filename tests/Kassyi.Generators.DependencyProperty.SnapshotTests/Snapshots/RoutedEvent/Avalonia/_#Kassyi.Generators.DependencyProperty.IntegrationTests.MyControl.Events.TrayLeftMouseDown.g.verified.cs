//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Events.TrayLeftMouseDown.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        /// <summary>
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Avalonia.Interactivity.RoutedEvent<global::Avalonia.Interactivity.RoutedEventArgs> TrayLeftMouseDownEvent = global::Avalonia.Interactivity.RoutedEvent.Register<global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl, global::Avalonia.Interactivity.RoutedEventArgs>(name: "TrayLeftMouseDown", routingStrategy: global::Avalonia.Interactivity.RoutingStrategies.Bubble);
        /// <summary>
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public event global::System.EventHandler<global::Avalonia.Interactivity.RoutedEventArgs> TrayLeftMouseDown { add => AddHandler(TrayLeftMouseDownEvent, value); remove => RemoveHandler(TrayLeftMouseDownEvent, value); }

        /// <summary>
        /// A helper method to raise the TrayLeftMouseDown event.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected global::Avalonia.Interactivity.RoutedEventArgs OnTrayLeftMouseDown()
        {
            var args = new global::Avalonia.Interactivity.RoutedEventArgs(TrayLeftMouseDownEvent);
            this.RaiseEvent(args);
            return args;
        }
    }
}