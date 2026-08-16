# Kassyi.Generators.DependencyProperty

[![Nuget package](https://img.shields.io/nuget/vpre/Kassyi.Generators.DependencyProperty)](https://www.nuget.org/packages/Kassyi.Generators.DependencyProperty/)
[![CI/CD](https://github.com/kassyi/DependencyPropertyGenerator/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/kassyi/DependencyPropertyGenerator/actions/workflows/main.yml)
[![License: MIT](https://img.shields.io/github/license/kassyi/DependencyPropertyGenerator)](https://github.com/kassyi/DependencyPropertyGenerator/blob/main/LICENSE)
[![Performance](https://img.shields.io/badge/performance-+26%25_faster-brightgreen.svg)](#zero-allocation--no-ide-lag)
[![Zero Gen2 GC](https://img.shields.io/badge/Gen2_GC-zero_alloc-blue.svg)](#zero-allocation--no-ide-lag)

A high-performance .NET source generator for Dependency Properties, Routed Events, and Weak Events. Supports WPF, UWP, WinUI, Uno, Avalonia, and MAUI.

## Why this fork?

This project is an independently maintained, highly optimized fork of [`HavenDV/DependencyPropertyGenerator`](https://github.com/HavenDV/DependencyPropertyGenerator). The generator has been fundamentally re-architected to eliminate silent bugs, enforce strict type safety, and deliver **up to 26% faster code generation with zero Gen2 allocations**.

### Zero-Allocation & No IDE Lag

Incremental source generators run continuously in the background on every keystroke. Heavy memory allocations in the original generator often triggered Gen2 garbage collections, causing noticeable typing lag in IDEs like Visual Studio and JetBrains Rider.

We rebuilt the pipeline from the ground up for maximum throughput:

- **Zero Gen2 GC Pauses**: Memory allocations are slashed by ~20% (saving ~600 KB per class), entirely eliminating Gen2 GC pauses.
- **Optimized Pipeline**: Replaced expensive AST round-trips (`NormalizeWhitespace()`) with direct, formatted streaming.
- **Zero-Allocation Scopes**: Implemented custom `ref struct` scope handlers to eliminate intermediate string allocations.
- **Declarative Rule Engine**: Shifted from runtime string parsing to compile-time semantic flag analysis.

<details>
<summary><b>View Benchmark Results (AMD Ryzen 9 7900X / .NET 9)</b></summary>

| Metric                            | Upstream Baseline  | This Fork              | Improvement                     |
| :-------------------------------- | :----------------- | :--------------------- | :------------------------------ |
| **Initial Generation (WPF)**      | 5.349 ms (2.87 MB) | **3.922 ms (2.28 MB)** | **-26.7% time / -20.6% memory** |
| **Incremental Gen (WPF)**         | 7.176 ms (3.59 MB) | **5.783 ms (3.02 MB)** | **-19.4% time / -15.9% memory** |
| **Initial Generation (WinUI)**    | 5.720 ms (2.81 MB) | **4.218 ms (2.27 MB)** | **-26.3% time / -19.2% memory** |
| **Incremental Gen (WinUI)**       | 7.412 ms (3.55 MB) | **6.420 ms (3.02 MB)** | **-13.4% time / -14.9% memory** |
| **Initial Generation (Avalonia)** | 5.282 ms (2.86 MB) | **4.574 ms (2.33 MB)** | **-13.4% time / -18.5% memory** |
| **Incremental Gen (Avalonia)**    | 7.103 ms (3.62 MB) | **5.884 ms (3.09 MB)** | **-17.2% time / -14.6% memory** |
| **Initial Generation (MAUI)**     | 5.533 ms (2.90 MB) | **4.528 ms (2.30 MB)** | **-18.2% time / -20.7% memory** |
| **Incremental Gen (MAUI)**        | 7.095 ms (3.67 MB) | **6.112 ms (3.09 MB)** | **-13.9% time / -15.8% memory** |
| **GC Gen2 (Initial)**             | 7.8–15.6 / 1k ops  | **0.0000 / 1k ops**    | **100% Eliminated**             |

</details>

### Critical Bug Fixes & Enhancements

- **Strict Type Safety (`#error DPG0001`)**: The original generator silently failed (emitting `propertyChangedCallback: null`) if the `OnChanged` callback signature was invalid. This fork immediately surfaces signature mismatches as compile-time diagnostic errors.
- **Target-Typed `new(...)` Expansion**: Seamlessly supports C# 9.0+ `DefaultValueExpression = "new(...)"`. The generator automatically expands these into fully-qualified constructors, removing the need for verbose type names.
- **Modular Architecture**: Decoupled monolithic logic into modular framework strategies (WPF, WinUI, Avalonia, etc.), significantly improving maintainability and testability.
- **Package Renaming**: Published as `Kassyi.Generators.DependencyProperty` to provide a clean namespace and prevent collisions.

## Quick Start

Define your properties using generic attributes. The generator automatically wires up the boilerplate.

```csharp
using DependencyPropertyGenerator;
using System.Windows.Controls;

#nullable enable

namespace MyApp.Controls;

[DependencyProperty<bool>("IsSpinning", DefaultValue = true, Category = "Category", Description = "Description")]
public partial class MyControl : UserControl
{
    // Optional callback, automatically wired up by the generator
    partial void OnIsSpinningChanged(bool oldValue, bool newValue)
    {
    }
}

[AttachedDependencyProperty<object, TreeView>("SelectedItem", DefaultBindingMode = DefaultBindingMode.TwoWay)]
public static partial class TreeViewExtensions
{
    // Optional callback, automatically wired up by the generator
    static partial void OnSelectedItemChanged(TreeView sender, object? oldValue, object? newValue)
    {
    }
}
```

<details>
<summary><b>View the generated code</b></summary>

```csharp
// HintName: MyControl.Properties.IsSpinning.generated.cs
#nullable enable

namespace MyApp.Controls
{
    public partial class MyControl
    {
        public static readonly global::System.Windows.DependencyProperty IsSpinningProperty =
            global::System.Windows.DependencyProperty.Register(
                name: "IsSpinning",
                propertyType: typeof(bool),
                ownerType: typeof(global::MyApp.Controls.MyControl),
                typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
                    defaultValue: (bool)true,
                    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
                    propertyChangedCallback: static (sender, args) =>
                    {
                        ((global::MyApp.Controls.MyControl)sender).OnIsSpinningChanged(
                            (bool)args.OldValue,
                            (bool)args.NewValue);
                    }));

        [global::System.ComponentModel.Category("Category")]
        [global::System.ComponentModel.Description("Description")]
        public bool IsSpinning
        {
            get => (bool)GetValue(IsSpinningProperty);
            set => SetValue(IsSpinningProperty, value);
        }

        partial void OnIsSpinningChanged();
        partial void OnIsSpinningChanged(bool newValue);
        partial void OnIsSpinningChanged(bool oldValue, bool newValue);
    }
}
```

_(Note: Code shortened for brevity)_

</details>

## Advanced Features

### Target-Typed `new(...)` in Default Values

You can cleanly define default values using C# 9.0+ target-typed `new()` expressions. The generator safely expands it to the full type:

```csharp
public readonly record struct Data(int Value);

// Automatically expands to the property's type (new global::MyNamespace.Data(42))
[AttachedDependencyProperty<Data, TreeView>("SelectedItem", DefaultValueExpression = "new(42)")]

// Also supports the default constructor:
[AttachedDependencyProperty<Data, TreeView>("SelectedItem", DefaultValueExpression = "new()")]
```

### Event Binding

The generator can automatically manage properties tied to UI events:

```csharp
[AttachedDependencyProperty<object, Grid>("BindEventProperty", BindEvent = nameof(Grid.MouseWheel), DefaultValueExpression = "new()")]
public static partial class GridExtensions
{
    private static void OnBindEventPropertyChanged_MouseWheel(object? sender, System.Windows.Input.MouseWheelEventArgs args)
    {
        // Handle the mouse wheel event
    }
}
```

When the property value changes, it automatically unsubscribes the old value and subscribes the new value to `sender.MouseWheel += OnBindEventPropertyChanged_MouseWheel;`.

### XML Documentation

The easiest way to generate XML documentation is via the `Description` property:

```csharp
[DependencyProperty<bool>("IsSpinning", Description = "Indicates whether the element is spinning.")]
```

This adds the `[Description]` attribute and embeds the text directly into the generated XML docs. For raw XML, use the `XmlDocumentation` or `PropertyXmlDocumentation` properties.

### Platform Setup

If automatic platform detection fails (e.g., when targeting multiple platforms or custom builds), you can explicitly set your target in the `.csproj`:

```xml
<PropertyGroup>
  <DefineConstants>$(DefineConstants);HAS_WPF</DefineConstants>
  <!-- <DefineConstants>$(DefineConstants);HAS_UNO</DefineConstants> -->
  <!-- <DefineConstants>$(DefineConstants);HAS_UNO_WINUI</DefineConstants> -->
  <!-- <DefineConstants>$(DefineConstants);HAS_AVALONIA</DefineConstants> -->
</PropertyGroup>
```

### UWP / WinUI / Uno Metadata Overrides

For UWP, WinUI, and Uno, the generator creates a `RegisterPropertyChangedCallbacks()` method. You must call this method manually in your control's constructor to properly register the property change callbacks.

## Prerequisites

- To use generic attributes (`[DependencyProperty<T>]`), ensure `LangVersion` is set to `11.0` or higher (or `preview`) in your `.csproj`:
    ```xml
    <LangVersion>preview</LangVersion>
    ```
- Non-generic attributes are also available for older language versions.

## Support & Feedback

- **Bug Reports & Issues**: [GitHub Issues](https://github.com/kassyi/DependencyPropertyGenerator/issues)
- **Discussions & Ideas**: [GitHub Discussions](https://github.com/kassyi/DependencyPropertyGenerator/discussions)
