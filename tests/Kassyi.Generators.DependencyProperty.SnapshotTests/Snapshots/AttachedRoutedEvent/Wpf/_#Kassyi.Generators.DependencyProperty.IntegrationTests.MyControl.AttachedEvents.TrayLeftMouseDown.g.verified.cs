//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.AttachedEvents.TrayLeftMouseDown.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// </summary>
        public static readonly global::System.Windows.RoutedEvent TrayLeftMouseDownEvent = global::System.Windows.EventManager.RegisterRoutedEvent(name: "TrayLeftMouseDown", routingStrategy: global::System.Windows.RoutingStrategy.Bubble, handlerType: typeof(global::System.Windows.RoutedEventHandler), ownerType: typeof(MyControl));
        /// <summary>
        /// </summary>
        public static void AddTrayLeftMouseDownHandler(global::System.Windows.DependencyObject element, global::System.Windows.RoutedEventHandler handler)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            if (element is global::System.Windows.UIElement uiElement)
            {
                uiElement.AddHandler(TrayLeftMouseDownEvent, handler);
            }
            else if (element is global::System.Windows.ContentElement contentElement)
            {
                contentElement.AddHandler(TrayLeftMouseDownEvent, handler);
            }
        }

        /// <summary>
        /// </summary>
        public static void RemoveTrayLeftMouseDownHandler(global::System.Windows.DependencyObject element, global::System.Windows.RoutedEventHandler handler)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            if (element is global::System.Windows.UIElement uiElement)
            {
                uiElement.RemoveHandler(TrayLeftMouseDownEvent, handler);
            }
            else if (element is global::System.Windows.ContentElement contentElement)
            {
                contentElement.RemoveHandler(TrayLeftMouseDownEvent, handler);
            }
        }
    }
}