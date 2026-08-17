//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Test.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class Test
{
static Test()
{
TestPropProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test?>>(static x =>
{
#pragma warning disable CS8600, CS8604
TestChanged((global::Avalonia.Controls.Grid)x.Sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test?)x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
}));
}
}
}
