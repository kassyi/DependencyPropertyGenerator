# 01. Foundation and Domain

[English](./01_foundation_and_domain.md) | [日本語](../ja/01_foundation_and_domain.md) | [Index (Intro)](./intro.md)

## Ⅰ. Purpose and Philosophy

The primary purpose of **DependencyPropertyGenerator** (`Kassyi.Generators.DependencyProperty`) is to automatically generate boilerplate declaration code for DependencyProperties, RoutedEvents, and WeakEvents across multiple .NET UI frameworks (WPF, UWP, WinUI, Uno, Avalonia, MAUI).

### Module Architecture

- **`Kassyi.Generators.DependencyProperty`**: The core Roslyn Incremental Source Generator. It extracts metadata at compile time and generates source code tailored to each UI framework.
- **`Kassyi.Generators.DependencyProperty.Attributes`**: Provides the declarative attributes (`[DependencyProperty]`, `[AttachedDependencyProperty]`, `[RoutedEvent]`, `[WeakEvent]`, etc.) that developers apply in their code.
- **`Kassyi.Generators.Extensions`**: The core library providing a zero-allocation foundation (`SourceWriter`, `EquatableArray<T>`, `HashCode`, etc.) shared across source generators.

### Technical Constraints and Policies

- **Roslyn Incremental Source Generator**: Targets `.NET Standard 2.0` and operates with high speed and ultra-low allocation during incremental evaluations (such as typing in the IDE).
- **Framework Abstraction**: Absorbs API differences among various UI frameworks internally, generating the appropriate code from a single attribute (`[DependencyProperty]`).
- **Leveraging `partial` Classes and Methods**: Adds generated code as `partial` classes and provides `partial void On...Changed(...)` methods for event hooking.

---

## Ⅱ. Ubiquitous Language Glossary

The following terms are standardized across the generator's codebase.

| Term (English)        | Term (Code)                  | Description                                                                                      |
| --------------------- | ---------------------------- | ------------------------------------------------------------------------------------------------ |
| UI Framework          | `Framework`                  | Enum identifying the target platform (e.g., WPF, Uno, MAUI, Avalonia, WinUI).                    |
| Dependency Property   | `DependencyProperty`         | Extended property mechanism for UI controls to retain state and support data binding.            |
| Attached Property     | `AttachedDependencyProperty` | Property mechanism allowing child elements to set values on parent elements.                     |
| Class Data            | `ClassData`                  | Metadata (type name, namespace, modifiers, etc.) of the target class (owner) with the attribute. |
| Property Data         | `DependencyPropertyData`     | The root data model encapsulating complete metadata for the property to be generated.            |
| Component Model Data  | `ComponentModelData`         | UI/designer metadata such as `[Description]`, `[Category]`, and `[TypeConverter]`.               |
| Framework Metadata    | `FrameworkMetadataData`      | Settings for `FrameworkPropertyMetadataOptions` (e.g., `AffectsMeasure`) in WPF and others.      |
| Validation & Callback | `ValidationAndCallbackData`  | Configuration for behavior such as validation, coercion, and change callbacks (`OnChanged`).     |
| Event Data            | `EventData`                  | Metadata for `RoutedEvent` and `WeakEvent`.                                                      |

---

## Ⅲ. Domain Data Models

These are pure data models (DTOs) extracted from Roslyn's `SyntaxNode` and `ISymbol` to flow through the incremental pipeline. **To maximize cache efficiency, they are exclusively defined as `readonly record struct` and support strict equality comparison via `IEquatable<T>`.**

### Main Data Models (DTOs)

```mermaid
classDiagram
    class ClassData {
        <<readonly record struct>>
        +string Namespace
        +string Name
        +string FullName
        +string Type
        +string Modifiers
        +string Version
        +bool IsStatic
        +Framework Framework
    }

    class DependencyPropertyData {
        <<readonly record struct>>
        +string Name
        +string Version
        +string Type
        +string ShortType
        +bool IsValueType
        +bool IsSpecialType
        +string? DefaultValue
        +string? DefaultValueDocumentation
        +bool IsReadOnly
        +bool IsDirect
        +bool IsAttached
        +bool IsAddOwner
        +Framework Framework
        +ComponentModelData ComponentModel
        +FrameworkMetadataData FrameworkMetadata
        +XmlDocumentationData XmlDocumentation
        +ValidationAndCallbackData ValidationAndCallbacks
    }

    class ComponentModelData {
        <<readonly record struct>>
        +string? Description
        +string? Category
        +string? TypeConverter
        +bool? Bindable
        +bool? Browsable
        +string? DesignerSerializationVisibility
        +bool? ClsCompliant
        +string? Localizability
        +string? BrowsableForType
        +string? FromType
    }

    class FrameworkMetadataData {
        <<readonly record struct>>
        +bool AffectsMeasure
        +bool AffectsArrange
        +bool AffectsParentMeasure
        +bool AffectsParentArrange
        +bool AffectsRender
        +bool Inherits
        +bool OverridesInheritanceBehavior
        +bool NotDataBindable
        +bool Journal
        +bool SubPropertiesDoNotAffectRender
        +bool IsAnimationProhibited
        +string? DefaultUpdateSourceTrigger
        +string? DefaultBindingMode
    }

    class ValidationAndCallbackData {
        <<readonly record struct>>
        +bool EnableDataValidation
        +bool Coerce
        +bool Validate
        +bool CreateDefaultValueCallback
        +EquatableArray~string~ BindEvents
        +string OnChanged
        +EventCallbackData Callbacks
    }

    class XmlDocumentationData {
        <<readonly record struct>>
        +string? XmlDocumentation
        +string? PropertyXmlDocumentation
        +string? GetterXmlDocumentation
        +string? SetterXmlDocumentation
    }

    class EventData {
        <<readonly record struct>>
        +string Name
        +string Strategy
        +string Type
        +bool IsValueType
        +bool IsAttached
        +string? Description
        +string? Category
        +string? XmlDocumentation
        +string? EventXmlDocumentation
        +bool WinRtEvents
    }

    DependencyPropertyData *-- ComponentModelData
    DependencyPropertyData *-- FrameworkMetadataData
    DependencyPropertyData *-- XmlDocumentationData
    DependencyPropertyData *-- ValidationAndCallbackData
```

### Data Structure Design Guidelines

- **Structural Separation by Responsibility**: `DependencyPropertyData`, which contains many properties, is structured into sub-models such as component models, UI metadata, XML documentation, and validation/callbacks. This improves maintainability and clarity.
- **Early Conversion to Primitive Types**: Retaining Roslyn's `INamedTypeSymbol` or `IPropertySymbol` directly causes memory leaks and invalidates the generator cache. Therefore, they must be converted into primitive types like `string` and `bool`, or `EquatableArray<T>`, during the extraction phase.
- **Collection Equality**: Collection data (e.g., `BindEvents`) uses `EquatableArray<T>` (a custom implementation) that guarantees structural equality, rather than standard arrays or `List<T>`.

---

## IV. Agentic DTO Mapping Specification (Agentic Ground Truth)

To allow autonomous agents (AI assistants) to investigate and modify the code, this section documents the mapping between C# attributes and their corresponding DTO properties. Use this specification as a structural map when performing bug fixes or feature additions.

### 1. `[DependencyProperty]` Attribute Mapping

Attributes defined in user code such as `[DependencyProperty<string>("Text", DefaultValue = "Foo")]` are parsed by `DependencyPropertyDataBuilder.cs` and `PrepareData.cs`, and stored in the following DTO fields.

#### Root Properties (DependencyPropertyData)

| Attribute Argument / Property | DTO Target Field                      | Type      | Description                                                                  |
| ----------------------------- | ------------------------------------- | --------- | ---------------------------------------------------------------------------- |
| Type Argument `<T>`           | `DependencyPropertyData.Type`         | `string`  | The property type (fully qualified).                                         |
| 1st Argument (Constructor)    | `DependencyPropertyData.Name`         | `string`  | The name of the dependency property (e.g., `"Text"`).                        |
| `DefaultValue`                | `DependencyPropertyData.DefaultValue` | `string?` | The default value such as string literals.                                   |
| `DefaultValueExpression`      | `DependencyPropertyData.DefaultValue` | `string?` | The default value as a C# expression like `new()`.                           |
| `IsReadOnly`                  | `DependencyPropertyData.IsReadOnly`   | `bool`    | Generates a read-only property using `DependencyPropertyKey` if `true`.      |
| `IsDirect`                    | `DependencyPropertyData.IsDirect`     | `bool`    | Avalonia-specific. Indicates if it should be generated as a direct property. |

#### Mapping to ValidationAndCallbackData

| Attribute Argument / Property | DTO Target Field                    | Type                     | Description                                                         |
| ----------------------------- | ----------------------------------- | ------------------------ | ------------------------------------------------------------------- |
| `OnChanged`                   | `ValidationAndCallbacks.OnChanged`  | `string`                 | Custom change callback method name.                                 |
| `Coerce`                      | `ValidationAndCallbacks.Coerce`     | `bool`                   | Indicates whether to generate value coercion (CoerceValueCallback). |
| `Validate`                    | `ValidationAndCallbacks.Validate`   | `bool`                   | Indicates whether to generate validation (ValidateValueCallback).   |
| `BindEvents`                  | `ValidationAndCallbacks.BindEvents` | `EquatableArray<string>` | List of control events to wire up.                                  |

#### Mapping to ComponentModelData

| Attribute Argument / Property | DTO Target Field               | Type      | Description                                        |
| ----------------------------- | ------------------------------ | --------- | -------------------------------------------------- |
| `Description`                 | `ComponentModel.Description`   | `string?` | Generated as the `[Description("...")]` attribute. |
| `Category`                    | `ComponentModel.Category`      | `string?` | Generated as the `[Category("...")]` attribute.    |
| `TypeConverter`               | `ComponentModel.TypeConverter` | `string?` | Converter type name in the `typeof(...)` format.   |

#### Mapping to FrameworkMetadataData (For WPF)

| Attribute Argument / Property | DTO Target Field                       | Type      | Description                                |
| ----------------------------- | -------------------------------------- | --------- | ------------------------------------------ |
| `AffectsMeasure`              | `FrameworkMetadata.AffectsMeasure`     | `bool`    | Requests a layout update (Measure pass).   |
| `AffectsRender`               | `FrameworkMetadata.AffectsRender`      | `bool`    | Requests a redraw (Render pass).           |
| `BindsTwoWayByDefault`        | `FrameworkMetadata.DefaultBindingMode` | `string?` | The default binding mode (e.g., `TwoWay`). |

### 2. Mapping to `ClassData`

Information about the parent class itself, where the properties are defined, is extracted into `ClassData`.

| Target              | DTO Target Field                   | Type        | Description                                           |
| ------------------- | ---------------------------------- | ----------- | ----------------------------------------------------- |
| Enclosing Namespace | `ClassData.Namespace`              | `string`    | The outer `namespace` declaration.                    |
| Class Name          | `ClassData.Name`                   | `string`    | The name of the `partial class` or `partial record`.  |
| Type Parameters     | `ClassData.NameWithTypeParameters` | `string`    | Generic type signature like `MyControl<T>`.           |
| Class Modifiers     | `ClassData.Modifiers`              | `string`    | Modifiers such as `public`, `internal`, `sealed`.     |
| `[AvaloniaObject]`  | `ClassData.Framework`              | `Framework` | The type of framework used (`WPF`, `Avalonia`, etc.). |
