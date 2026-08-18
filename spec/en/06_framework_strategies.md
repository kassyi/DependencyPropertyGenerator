# 06. Framework Strategies

[English](./06_framework_strategies.md) | [日本語](../ja/06_framework_strategies.md) | [Index (Intro)](./intro.md)

DependencyPropertyGenerator dynamically generates optimal boilerplate code for target UI frameworks—WPF, UWP, WinUI, Uno, Avalonia, or MAUI—based on a single `[DependencyProperty]` attribute.

This document serves as the authoritative API mapping dictionary (Ground Truth) for executing framework-specific bug fixes or feature extensions. All architectural differences between platforms are abstracted away through the `IFrameworkGeneratorStrategy` implementation classes located in the `Sources/Strategies/` directory.

---

## I. Property Registration API Mapping

### WPF (`WpfFrameworkGenerator`)

WPF utilizes `System.Windows.DependencyProperty` and `DependencyPropertyKey` as the foundation for its property system.

- **Registration**: Invokes `DependencyProperty.Register` or `RegisterAttached`.
- **Read-Only**: Utilizes `RegisterReadOnly` and `RegisterAttachedReadOnly`.
- **Metadata**: Managed via `System.Windows.FrameworkPropertyMetadata` or `PropertyMetadata`.
- **Callbacks**: Wired using dedicated delegate types (`PropertyChangedCallback`, `CoerceValueCallback`, `ValidateValueCallback`).

> [!NOTE]
> The WPF `FrameworkPropertyMetadata` contains an extensive set of layout and data binding flags (e.g., `AffectsMeasure`, `BindsTwoWayByDefault`). The generator heavily relies on the fields in `FrameworkMetadataData` to emit these WPF-specific flags securely.

### Avalonia (`AvaloniaFrameworkGenerator`)

Avalonia builds upon `Avalonia.AvaloniaProperty`, typically defining properties as `StyledProperty<T>`, `AttachedProperty<T>`, or `DirectProperty<T>`.

- **Registration**: Invokes `AvaloniaProperty.Register` or `RegisterAttached`.
- **Direct Properties**: When the `IsDirect` flag is enabled, the generator emits the specialized generic method `RegisterDirect` for fast, field-backed access.
- **Metadata**: Passed directly as arguments or managed via Avalonia's native metadata classes.
- **Callbacks**: Routed through Observables or event-based subscription models like `AvaloniaPropertyChanged`.

### MAUI (`MauiFrameworkGenerator`)

MAUI employs a distinct type system utilizing `Microsoft.Maui.Controls.BindableProperty` and `BindablePropertyKey`.

- **Registration**: Performed via `BindableProperty.Create` or `CreateAttached`.
- **Read-Only**: Utilizes `CreateReadOnly` or `CreateAttachedReadOnly`.
- **Metadata**: Passed as flat arguments to the API rather than encapsulated in a dedicated class.
- **Callbacks**: Mapped to specific delegates (`BindingPropertyChangedDelegate`, `CoerceValueDelegate`, `ValidateValueDelegate`).

### UWP, WinUI, and Uno (`UwpFrameworkGenerator`)

UWP and Uno rely on `Windows.UI.Xaml.DependencyProperty`, whereas WinUI 3 relies on `Microsoft.UI.Xaml.DependencyProperty`.

- **Registration**: Strictly limited to `DependencyProperty.Register` and `RegisterAttached`.
- **Metadata**: Handled using `PropertyMetadata`.
- **Callbacks**: Natively provides only `PropertyChangedCallback`.

> [!WARNING]
> These platforms lack native API support for coercion and validation. The generator must emit specialized fallback implementations that manually clamp or correct values inside the property's getter/setter or within the PropertyChanged event itself.

---

## II. Strategy Extension Guidelines

When adding support for new UI frameworks or addressing breaking API changes (e.g., Avalonia v12), you must strictly follow these architectural principles:

> [!IMPORTANT]
> **1. Preserve the DTOs**
> Never mutate the shared DTO models (e.g., `DependencyPropertyData`). All platform-specific differences must be aggressively isolated and absorbed by overriding the methods in the corresponding generator class (e.g., `XxxFrameworkGenerator.cs`) under the `Sources/Strategies/` directory.

> [!TIP]
> **2. Method Extraction for Signature Variances**
> Leverage method extraction to resolve API signature differences. For example, use the `GenerateRegisterMethodArguments` method to construct the exact string of arguments passed to the `Register` method, gracefully accommodating varying argument configurations.

> [!CAUTION]
> **3. Strictly Zero-Allocation**
> To preserve benchmark scores, strictly prohibit the use of LINQ or unnecessary `string.Join` calls within the string generation paths (`SourceWriter`).

---

## III. Automatic Framework Detection and Fallback

During Roslyn pipeline initialization, the generator automatically resolves the target UI framework utilizing the following strict priority cascade:

1. **High-Precision Symbol Inspection (`Compilation.TryRecognizeFramework`)**
   Inspects core framework type symbols present in the compilation context:
    - `Microsoft.Maui.Controls.BindableObject` $\rightarrow$ `Framework.Maui`
    - `Avalonia.AvaloniaObject` $\rightarrow$ `Framework.Avalonia`
    - `Uno.UI.FeatureConfiguration` $\rightarrow$ `Framework.Uno` / `Framework.UnoWinUi`
    - `Microsoft.UI.Xaml.DependencyObject` $\rightarrow$ `Framework.WinUi`
    - `Windows.UI.Xaml.DependencyObject` $\rightarrow$ `Framework.Uwp`
    - `System.Windows.DependencyObject` $\rightarrow$ `Framework.Wpf`

2. **MSBuild Property / Compilation Constant Fallback (`AnalyzerConfigOptionsProvider`)**
   If symbols cannot be resolved, inspects `DefineConstants` (`HAS_WPF`, `HAS_WINUI`, `HAS_UWP`, `HAS_UNO`, `HAS_UNO_WINUI`, `HAS_AVALONIA`, `HAS_MAUI`) or the `UseMaui` property in project files.

3. **Unrecognized Framework Fallback (`Framework.None`)**
   If no framework successfully matches, the generator safely assigns `Framework.None`. In this state, it emits diagnostic `DPG0000` (Framework is not recognized) while selectively skipping platform-specific `using` imports and registrations. It securely emits only the raw attribute definitions to completely prevent compilation failure.
