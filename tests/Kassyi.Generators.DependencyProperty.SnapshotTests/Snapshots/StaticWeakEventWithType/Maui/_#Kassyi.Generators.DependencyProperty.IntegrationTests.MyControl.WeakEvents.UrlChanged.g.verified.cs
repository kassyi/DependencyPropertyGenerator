//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.WeakEvents.UrlChanged.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        private static global::Microsoft.Maui.WeakEventManager UrlChangedWeakEventManager { get; } = new global::Microsoft.Maui.WeakEventManager();

        /// <summary>
        /// </summary>
        public static event global::System.EventHandler<string?>? UrlChanged { add => UrlChangedWeakEventManager.AddEventHandler(value); remove => UrlChangedWeakEventManager.RemoveEventHandler(value); }

        /// <summary>
        /// A helper method to raise the UrlChanged event.
        /// </summary>
        internal static void RaiseUrlChangedEvent(object? sender, string? args)
        {
            UrlChangedWeakEventManager.HandleEvent(sender!, args!, eventName: nameof(UrlChanged));
        }
    }
}