# 06. Framework Strategies

[English](./06_framework_strategies.md) | [日本語](../ja/06_framework_strategies.md) | [Index (Intro)](./intro.md)

DependencyPropertyGenerator dynamically generates the appropriate boilerplate code for the target UI framework—such as WPF, UWP, WinUI, Uno, Avalonia, or MAUI—based on a single `[DependencyProperty]` attribute.
This document serves as an API mapping dictionary (Ground Truth) for developers and autonomous agents to perform framework-specific bug fixes or feature extensions.

The architectural differences between each platform are entirely abstracted away through the `IFrameworkGeneratorStrategy` implementation classes located in the `Sources/Strategies/` directory.

---

## I. Property Registration API Mapping

### WPF (WpfFrameworkGenerator)
WPF uses `System.Windows.DependencyProperty` and `DependencyPropertyKey` as the foundation for its property system.
To register a property, the generator invokes `DependencyProperty.Register`, or `RegisterAttached` for attached properties. When generating read-only properties, it uses `RegisterReadOnly` and `RegisterAttachedReadOnly`.
Metadata is managed through `System.Windows.FrameworkPropertyMetadata` or `PropertyMetadata`, and callbacks are wired up using dedicated delegate types such as `PropertyChangedCallback`, `CoerceValueCallback`, and `ValidateValueCallback`.
The most notable implementation detail for WPF is that its metadata (`FrameworkPropertyMetadata`) contains an extensive set of flags tailored for layout and data binding, such as `AffectsMeasure` and `BindsTwoWayByDefault`. Consequently, the generator utilizes the fields in `FrameworkMetadataData` most heavily for WPF code generation.

### Avalonia (AvaloniaFrameworkGenerator)
Avalonia builds upon `Avalonia.AvaloniaProperty`, typically defining properties as `StyledProperty<T>`, `AttachedProperty<T>`, or `DirectProperty<T>`.
The generator calls `AvaloniaProperty.Register` or `RegisterAttached` for standard registrations. However, Avalonia introduces the concept of `DirectProperty`, which is a fast, field-backed property. When the `IsDirect` flag is enabled, the generator emits a specialized generic method named `RegisterDirect`.
Metadata is either passed directly as arguments to the registration method or managed using Avalonia's native metadata features. Change notifications are handled through Observables or event-based subscription models like `AvaloniaPropertyChanged`.

### MAUI (MauiFrameworkGenerator)
MAUI employs a unique type system, utilizing `Microsoft.Maui.Controls.BindableProperty` and `BindablePropertyKey` instead of the traditional DependencyProperty.
Property registration is performed using `BindableProperty.Create` or `CreateAttached`, while read-only properties use `CreateReadOnly` or `CreateAttachedReadOnly`. Unlike WPF, metadata is not encapsulated in a dedicated class but is instead passed as flat arguments to the API.
Callbacks are mapped to specific delegate types, namely `BindingPropertyChangedDelegate`, `CoerceValueDelegate`, and `ValidateValueDelegate`.

### UWP, WinUI, and Uno (UwpFrameworkGenerator)
For these platforms, UWP and Uno rely on `Windows.UI.Xaml.DependencyProperty`, whereas WinUI 3 uses `Microsoft.UI.Xaml.DependencyProperty`.
The registration API is strictly limited to `DependencyProperty.Register` and `RegisterAttached`, and metadata is handled using `PropertyMetadata`. The only callback natively provided is the `PropertyChangedCallback`.
An important caveat is that these platforms lack native APIs for coercion and validation. To compensate, the generator emits fallback implementations that manually clamp or correct values inside the property's getter and setter, or within the PropertyChanged event itself, mimicking the missing behavior.

---

## II. Strategy Extension Guidelines

When adding support for a new UI framework—or addressing breaking API changes like those in Avalonia v12—you must follow these core principles to extend the implementation safely.

The first rule is to never modify the shared DTO models, such as `DependencyPropertyData`. All platform-specific differences must be isolated and absorbed by overriding the methods in the appropriate generator class (e.g., `XxxFrameworkGenerator.cs`) under the `Sources/Strategies/` directory.

Next, leverage method extraction to resolve differences in API signatures across frameworks. For example, the `GenerateRegisterMethodArguments` method is responsible for constructing the exact string of arguments passed to the `Register` method, allowing you to easily accommodate varying argument orders or types.

Finally, to preserve benchmark scores and maintain zero-allocation characteristics, you must avoid using LINQ or unnecessary `string.Join` calls within the string generation paths (`SourceWriter`).
