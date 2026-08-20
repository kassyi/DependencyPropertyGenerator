# Kassyi.Generators.DependencyProperty

[English](README.md) | [日本語](README.ja.md)

[![Nuget package](https://img.shields.io/nuget/vpre/Kassyi.Generators.DependencyProperty)](https://www.nuget.org/packages/Kassyi.Generators.DependencyProperty/)
[![CI/CD](https://github.com/kassyi/DependencyPropertyGenerator/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/kassyi/DependencyPropertyGenerator/actions/workflows/main.yml)
[![License: MIT](https://img.shields.io/github/license/kassyi/DependencyPropertyGenerator)](https://github.com/kassyi/DependencyPropertyGenerator/blob/main/LICENSE)
[![Wiki](https://img.shields.io/badge/docs-Wiki-blue.svg)](https://github.com/kassyi/DependencyPropertyGenerator/wiki)
[![Specifications](https://img.shields.io/badge/docs-specifications-blue.svg)](./spec/en/intro.md)
[![Performance](https://img.shields.io/badge/performance-+30%25_faster-brightgreen.svg)](#zero-allocation--no-ide-lag)
[![Zero Gen2 GC](https://img.shields.io/badge/Gen2_GC-zero_alloc-blue.svg)](#zero-allocation--no-ide-lag)

The `Kassyi.Generators.DependencyProperty` is a zero-allocation, highly optimized generator for dotnet/runtime-scale codebases. It delivers microsecond-tier throughput and supports WPF, UWP, WinUI, Uno, Avalonia, and MAUI.

## Why this fork?

This project is an independently maintained, highly optimized fork of [`HavenDV/DependencyPropertyGenerator`](https://github.com/HavenDV/DependencyPropertyGenerator). We re-architected the generator to eliminate critical silent failures (such as emitting `propertyChangedCallback: null` without warnings, as seen in [HavenDV#165](https://github.com/HavenDV/DependencyPropertyGenerator/issues/165)), enforce strict compile-time type safety, and deliver **up to 30% faster code generation (+62.4% overall throughput score) with zero Gen2 allocations**.

### Zero-Allocation & No IDE Lag

Incremental source generators run continuously in the background on every keystroke. Heavy memory allocations in the original generator triggered Gen2 garbage collections, causing noticeable typing lag in IDEs like Visual Studio and JetBrains Rider.

We rebuilt the pipeline to maximize throughput:

- **Zero Gen2 GC Pauses**: Reduces memory allocations by ~22% (saving ~650 KB per class) to completely eliminate Gen2 GC pauses.
- **Optimized Pipeline**: Replaces expensive AST round-trips (`NormalizeWhitespace()`) with direct, zero-allocation formatted streaming.
- **Zero-Allocation Scopes**: Implements custom `ref struct` scope handlers (`ClassScope`) to prevent intermediate string allocations.
- **Declarative Rule Engine**: Shifts from runtime string parsing to compile-time semantic flag analysis.

<details>
<summary><b>View Benchmark Results (AMD Ryzen 9 7900X / .NET 9)</b></summary>

| Metric | Upstream Baseline | This Fork (Phase 5) | Improvement |
| :--- | :--- | :--- | :--- |
| **Initial Generation (WPF)** | 5.349 ms (2.87 MB) | **3.729 ms (2.22 MB)** | **-30.3% time / -22.6% memory** |
| **Incremental Gen (WPF)** | 7.176 ms (3.59 MB) | **5.663 ms (2.93 MB)** | **-21.1% time / -18.4% memory** |
| **Initial Generation (WinUI)** | 5.720 ms (2.81 MB) | **4.192 ms (2.21 MB)** | **-26.7% time / -21.4% memory** |
| **Incremental Gen (WinUI)** | 7.412 ms (3.55 MB) | **5.847 ms (2.94 MB)** | **-21.1% time / -17.2% memory** |
| **Initial Generation (Avalonia)** | 5.282 ms (2.86 MB) | **4.137 ms (2.25 MB)** | **-21.7% time / -21.3% memory** |
| **Incremental Gen (Avalonia)** | 7.103 ms (3.62 MB) | **5.665 ms (3.01 MB)** | **-20.2% time / -16.9% memory** |
| **Initial Generation (MAUI)** | 5.533 ms (2.90 MB) | **4.147 ms (2.26 MB)** | **-25.0% time / -22.1% memory** |
| **Incremental Gen (MAUI)** | 7.095 ms (3.67 MB) | **5.843 ms (3.02 MB)** | **-17.6% time / -17.7% memory** |
| **Overall Throughput Score** | 1,000 pts (1,288 ops/s) | **1,624 pts (1,685 ops/s)** | **+62.4% Score Boost** 🚀 |
| **GC Gen2 (Initial)** | 7.8–15.6 / 1k ops | **0.0000 / 1k ops** | **100% Eliminated** |

</details>

### Critical Bug Fixes & Enhancements

- **Strict Type Safety (`#error DPG0001` / `DPG0007`)**: The original generator silently failed ([HavenDV#165](https://github.com/HavenDV/DependencyPropertyGenerator/issues/165)), emitting `propertyChangedCallback: null` without diagnostic errors for invalid `OnChanged` callbacks. This fork immediately surfaces signature mismatches as compile-time errors.
- **Target-Typed `new(...)` Expansion**: Supports C# 9.0+ `DefaultValueExpression = "new(...)"`. The generator automatically expands these into fully-qualified constructors to remove verbose type names.
- **Modular Architecture**: Decouples monolithic logic into modular framework strategies (WPF, WinUI, Avalonia, etc.) to improve maintainability and testability.
- **Package Renaming**: Publishes as `Kassyi.Generators.DependencyProperty` to provide a clean namespace and prevent collisions.

## Installation

Install the package via the .NET CLI or NuGet Package Manager:

```bash
dotnet add package Kassyi.Generators.DependencyProperty
```

Or add the package reference directly to your `.csproj`:

```xml
<PackageReference Include="Kassyi.Generators.DependencyProperty" Version="0.1.0" PrivateAssets="all" />
```

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

The generator automatically manages properties tied to UI events:

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

<details>
<summary><b>View Generated XML Documentation Code</b></summary>

```csharp
/// <summary>
/// Identifies the <see cref="IsSpinning"/> dependency property.<br/>
/// Default value: default(bool)
/// </summary>
public static readonly global::System.Windows.DependencyProperty IsSpinningProperty =
    global::System.Windows.DependencyProperty.Register(...);

/// <summary>
/// Indicates whether the element is spinning.<br/>
/// Default value: default(bool)
/// </summary>
[global::System.ComponentModel.Description("Indicates whether the element is spinning.")]
public bool IsSpinning
{
    get => (bool)GetValue(IsSpinningProperty);
    set => SetValue(IsSpinningProperty, value);
}
```

</details>

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

> [!IMPORTANT]
> **C# Language Version Requirement (`LangVersion`)**
> To use generic attributes (e.g., `[DependencyProperty<T>]`, `[RoutedEvent<T>]`), ensure your project's `.csproj` sets `LangVersion` to **`11.0` or higher** (or `preview` / `latest`):
>
> ```xml
> <PropertyGroup>
>   <LangVersion>11.0</LangVersion> <!-- or preview / latest -->
> </PropertyGroup>
> ```
>
> _(Note: Non-generic attributes such as `[DependencyProperty("Name", typeof(Type))]` remain fully functional on older C# versions like C# 8.0+.)_

## Architecture & Specifications

Detailed architectural blueprints, zero-allocation design patterns, complexity models, and framework-specific code generation rules are available on the **[GitHub Wiki](https://github.com/kassyi/DependencyPropertyGenerator/wiki)** (or under the [`spec/`](./spec) directory for local offline reading):

- 🌐 **[📖 Read on GitHub Wiki (Recommended)](https://github.com/kassyi/DependencyPropertyGenerator/wiki)**
- 🇺🇸 **English Specifications (`spec/en/`)**:
    - **[Overview & Index](./spec/en/intro.md)**
    - **[01. FAQ & Design Rationale](./spec/en/01_faq_and_rationale.md)**: Architectural philosophy & zero-allocation rationale
    - **[02. Foundation & Domain](./spec/en/02_foundation_and_domain.md)**: DTO structure, ubiquitous language & supported platforms
    - **[03. Pipeline Architecture](./spec/en/03_pipeline_architecture.md)**: Incremental generator pipeline & caching strategy
    - **[04. Framework Strategies](./spec/en/04_framework_strategies.md)**: Platform API mapping & generator extension guidelines
    - **[05. Code Synthesis & Performance](./spec/en/05_synthesis_and_performance.md)**: `SourceWriter` (`ClassScope`), zero-allocation synthesis, & profiling
    - **[06. Complexity Model](./spec/en/06_mathematical_model.md)**: Worst-case complexity analysis & scaling limits
    - **[07. Test Specification](./spec/en/07_test_specification.md)**: Test architecture, combinatorial matrix, & diagnostics
    - **[08. Diagnostics Reference](./spec/en/08_diagnostics_reference.md)**: Causes and solutions for `DPG0000`-`DPG9999`
- 🇯🇵 **日本語仕様書 (`spec/ja/`)**:
    - **[仕様書概要・インデックス](./spec/ja/intro.md)**

## Support & Feedback

- **Bug Reports & Issues**: [GitHub Issues](https://github.com/kassyi/DependencyPropertyGenerator/issues)
- **Discussions & Ideas**: [GitHub Discussions](https://github.com/kassyi/DependencyPropertyGenerator/discussions)
