# DependencyPropertyGenerator

[![Nuget package](https://img.shields.io/nuget/vpre/DependencyPropertyGenerator)](https://www.nuget.org/packages/DependencyPropertyGenerator/)
[![CI/CD](https://github.com/HavenDV/DependencyPropertyGenerator/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/HavenDV/DependencyPropertyGenerator/actions/workflows/main.yml)
[![License: MIT](https://img.shields.io/github/license/HavenDV/DependencyPropertyGenerator)](https://github.com/HavenDV/DependencyPropertyGenerator/blob/main/LICENSE.txt)
[![Discord](https://img.shields.io/discord/988253265550532680?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discord.gg/g8u2t9dKgE)

Dependency property, routed event and weak event source generator for WPF/UWP/WinUI/Uno/Avalonia/MAUI platforms.

## Fork Improvements & Motivation

This repository is a custom fork of [`HavenDV/DependencyPropertyGenerator`](https://github.com/HavenDV/DependencyPropertyGenerator), independently extended and maintained.  
Since commit [`6758690d299c`](https://github.com/HavenDV/DependencyPropertyGenerator/commit/6758690d299c8777133e8351350d91126f44d314), the following critical bug fixes, feature enhancements, and performance optimizations have been implemented.

### 1. Why Forking Was Necessary

- **Resolution of Silent Callback Suppression / Silent Failure Bug**:
    - In the upstream (original) repository, specifying an unsupported signature for an `OnChanged` callback (such as WPF's standard signature `(DependencyObject d, DependencyPropertyChangedEventArgs e)` or mismatched parameter counts/types) or misspelling the method name caused the generator to **silently emit `propertyChangedCallback: null` without raising any compilation errors or warnings**.
    - As a result, code would compile successfully, but property change callbacks would **silently fail to execute at runtime**, creating severe and hard-to-detect silent bugs.
    - In this fork, failure to resolve an explicitly specified callback signature has been strictly escalated from a silent fallback/warning to a **`#error DPG0001` compile-time error**, ensuring issues are caught and resolved immediately at build time.

### 2. Key Improvements Since Commit `6758690d299c`

1. **Promotion of Unresolved Callbacks to Compile Errors (`#error DPG0001`)**
    - Mismatched or non-existent callback method declarations are flagged with a compile error (`#error DPG0001`) instead of being silently ignored.
2. **Automatic Target-Typed `new(...)` Expansion in `DefaultValueExpression`**
    - Added support for C# 9.0+ `DefaultValueExpression = "new(...)"` syntax.
    - Parses expressions via Roslyn AST (`SyntaxFactory` / `ImplicitObjectCreationExpressionSyntax`) and automatically expands target-typed `new(...)` expressions into fully-qualified constructor calls based on the property type. Eliminates the need to write verbose fully-qualified type names when referencing types from external namespaces.
3. **Incremental Source Generator (ISG) Pipeline Optimization & Allocation Reduction**
    - Streamlined `ClassData` DTO by removing heavy method string lists and replacing them with pre-calculated boolean flags (`IsChanged0`–`IsChanged3`).
    - Replaced temporary `List<string>` allocations and `string.Join` calls during code generation with `StringBuilder` and `stackalloc Span<char>` to eliminate intermediate heap allocations. Significantly reduces GC overhead and prevents IDE typing lags.
    - Introduced `DependencyPropertyGenerator.Benchmarks` (using BenchmarkDotNet) to continuously profile execution time and memory allocations across optimization phases.
4. **Modernization of Tests, Documentation, and Project Structure**
    - Modernized snapshot test fixtures (`Tests.*.cs`) using C# 11 Raw String Literals and unified MSTest attributes.
    - Reorganized project layout into standard C# `src/` and `tests/` directories.
    - Expanded architectural and generator specification documents in `spec/`.
5. **Solution and Package Renaming (`Kassyi.Generators.DependencyProperty`)**
    - Renamed the solution, project files, and NuGet package targets to `Kassyi.Generators.DependencyProperty` to prevent namespace collisions and clearly distinguish this custom fork from the original upstream package.

## Usage

```cs
using DependencyPropertyGenerator;
using System.Windows.Controls;

#nullable enable

namespace H.Generators.IntegrationTests;

[DependencyProperty<bool>("IsSpinning", DefaultValue = true, Category = "Category", Description = "Description")]
public partial class MyControl : UserControl
{
    // Optional
    partial void OnIsSpinningChanged(bool oldValue, bool newValue)
    {
    }
}

[AttachedDependencyProperty<object, TreeView>("SelectedItem", DefaultBindingMode = DefaultBindingMode.TwoWay)]
public static partial class TreeViewExtensions
{
    // Optional
    static partial void OnSelectedItemChanged(TreeView sender, object? oldValue, object? newValue)
    {
    }
}
```

will generate:

```cs
//HintName: MyControl.Properties.IsSpinning.generated.cs

#nullable enable

namespace H.Generators.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Description<br/>
        /// Default value: true
        /// </summary>
        public static readonly global::System.Windows.DependencyProperty IsSpinningProperty =
            global::System.Windows.DependencyProperty.Register(
                name: "IsSpinning",
                propertyType: typeof(bool),
                ownerType: typeof(global::H.Generators.IntegrationTests.MyControl),
                typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
                    defaultValue: (bool)true,
                    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
                    propertyChangedCallback: static (sender, args) =>
                    {
                        ((global::H.Generators.IntegrationTests.MyControl)sender).OnIsSpinningChanged(
                            (bool)args.OldValue,
                            (bool)args.NewValue);
                    },
                    coerceValueCallback: null,
                    isAnimationProhibited: false),
                validateValueCallback: null);

        /// <summary>
        /// Description<br/>
        /// Default value: true
        /// </summary>
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

```cs
//HintName: TreeViewExtensions.AttachedProperties.SelectedItem.generated.cs

#nullable enable

namespace H.Generators.IntegrationTests
{
    public static partial class TreeViewExtensions
    {
        /// <summary>
        /// Default value: default(object)
        /// </summary>
        public static readonly global::System.Windows.DependencyProperty SelectedItemProperty =
            global::System.Windows.DependencyProperty.RegisterAttached(
                name: "SelectedItem",
                propertyType: typeof(object),
                ownerType: typeof(global::H.Generators.IntegrationTests.TreeViewExtensions),
                defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(
                    defaultValue: default(object),
                    flags: global::System.Windows.FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    propertyChangedCallback: static (sender, args) =>
                    {
                        OnSelectedItemChanged(
                            (global::System.Windows.Controls.TreeView)sender,
                            (object?)args.OldValue,
                            (object?)args.NewValue);
                    },
                    coerceValueCallback: null,
                    isAnimationProhibited: false),
                validateValueCallback: null);

        /// <summary>
        /// Default value: default(object)
        /// </summary>
        public static void SetSelectedItem(global::System.Windows.DependencyObject element, object? value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));

            element.SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// Default value: default(object)
        /// </summary>
        [global::System.Windows.AttachedPropertyBrowsableForType(typeof(global::System.Windows.Controls.TreeView))]
        public static object? GetSelectedItem(global::System.Windows.DependencyObject element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));

            return (object?)element.GetValue(SelectedItemProperty);
        }

        static partial void OnSelectedItemChanged();
        static partial void OnSelectedItemChanged(global::System.Windows.Controls.TreeView treeView);
        static partial void OnSelectedItemChanged(global::System.Windows.Controls.TreeView treeView, object? newValue);
        static partial void OnSelectedItemChanged(global::System.Windows.Controls.TreeView treeView, object? oldValue, object? newValue);
    }
}
```

## Advanced usage

### Target-typed `new(...)` / `new()` expressions in DefaultValue

While it's [generally not recommended](https://devblogs.microsoft.com/oldnewthing/20191002-00/?p=102950)
to use new expressions in properties, the generator supports C# 9.0+ target-typed `new(...)` and `new()` expressions:

```cs
public readonly record struct Data(int Value);

// Target-typed new() syntax is automatically expanded to the property type:
[AttachedDependencyProperty<Data, TreeView>("SelectedItem", DefaultValueExpression = "new(42)")]
// Or default constructor:
[AttachedDependencyProperty<Data, TreeView>("SelectedItem", DefaultValueExpression = "new()")]
```

Target-typed `new(...)` expressions are automatically expanded by the generator to the property type (e.g. `new global::MyNamespace.Data(...)`), eliminating the need to write verbose fully-qualified type names even when referencing types from external namespaces.

### XML documentation

The easiest way to add documentation is the `Description` parameter.
It will add a `System.ComponentModel.Description` attribute to the property and will also be used in the xml documentation.  
If for some reason you need to save raw xml documentation for your properties,
there is an option to specify raw xml text for both DependencyProperty and getter/setter
via `XmlDocumentation`/`PropertyXmlDocumentation` attribute properties.

### Platform detection

For some platforms there is no automatic detection. In these cases, the generator needs a little help by adding:

```xml
  <PropertyGroup>
    <DefineConstants>$(DefineConstants);HAS_UNO</DefineConstants>
    <DefineConstants>$(DefineConstants);HAS_UNO_WINUI</DefineConstants>
    <DefineConstants>$(DefineConstants);HAS_AVALONIA</DefineConstants>
  </PropertyGroup>
```

Automatic detection of Uno [was added in Uno 4.5](https://github.com/unoplatform/uno/pull/9443).

### Bind event

The generator can automatically control properties that depend on events (and supports `DefaultValueExpression = "new()"`):

```cs
[AttachedDependencyProperty<object, Grid>("BindEventProperty", BindEvent = nameof(Grid.MouseWheel), DefaultValueExpression = "new()")]
public static partial class GridExtensions
{
    private static void OnBindEventPropertyChanged_MouseWheel(object? sender, System.Windows.Input.MouseWheelEventArgs args)
    {
    }
}
```

will generate additional code:

```cs
static partial void OnBindEventPropertyChanged_BeforeBind(
    global::System.Windows.Controls.Grid sender,
    object? oldValue,
    object? newValue);
static partial void OnBindEventPropertyChanged_AfterBind(
    global::System.Windows.Controls.Grid sender,
    object? oldValue,
    object? newValue);

static partial void OnBindEventPropertyChanged(
    global::System.Windows.Controls.Grid sender,
    object? oldValue,
    object? newValue)
{
    OnBindEventPropertyChanged_BeforeBind(
        sender,
        oldValue,
        newValue);

    if (oldValue is not default(object))
    {
        sender.MouseWheel -= OnBindEventPropertyChanged_MouseWheel;
    }
    if (newValue is not default(object))
    {
        sender.MouseWheel += OnBindEventPropertyChanged_MouseWheel;
    }

    OnBindEventPropertyChanged_AfterBind(
        sender,
        oldValue,
        newValue);
}
```

### Override metadata

For UWP/WinUI/Uno, a special `RegisterPropertyChangedCallbacks()` method will be created,
which you will need to call in the constructor to register property change callbacks.

## Notes

To use generic attributes, you need to set up `LangVersion` in your .csproj:

```xml
<LangVersion>preview</LangVersion>
```

There are also non-Generic attributes here.

## Possible features

- [Mutable default values in constructor](https://devblogs.microsoft.com/oldnewthing/20191003-00/?p=102959)

## Support

Priority place for bugs: https://github.com/HavenDV/DependencyPropertyGenerator/issues  
Priority place for ideas and general questions: https://github.com/HavenDV/DependencyPropertyGenerator/discussions  
I also have a Discord support channel:  
https://discord.gg/g8u2t9dKgE
