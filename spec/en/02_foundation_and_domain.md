# 02. Foundation & Domain Architecture

[English](./02_foundation_and_domain.md) | [日本語](../ja/02_foundation_and_domain.md)
Prev: [⬅ 01. Design Rationale & FAQ](./01_faq_and_rationale.md) | [Index (Intro)](./intro.md) | Next: [03. Pipeline Architecture ➡](./03_pipeline_architecture.md)

## I. Purpose and Architecture

The primary architectural goal of **DependencyPropertyGenerator** (`Kassyi.Generators.DependencyProperty`) is to automatically generate boilerplate code for DependencyProperties, RoutedEvents, and WeakEvents across multiple .NET UI frameworks. Supported platforms include WPF, UWP, WinUI, Uno, Avalonia, and MAUI.

### Module Topology

- **`Kassyi.Generators.DependencyProperty`**: The core Roslyn Incremental Source Generator. It extracts metadata at compile time and emits framework-specific C# source code.
- **`Kassyi.Generators.DependencyProperty.Attributes`**: Provides declarative attributes (e.g., `[DependencyProperty]`, `[AttachedDependencyProperty]`, `[RoutedEvent]`) for developers.
- **`Kassyi.Generators.Extensions`**: The core utility library providing a zero-allocation foundation. It exposes primitives like `SourceWriter` and `EquatableArray<T>` shared across source generators.

### Technical Constraints and Policies

- **Incremental Evaluation:** Targeting `.NET Standard 2.0`, the generator ensures high-speed execution and ultra-low memory allocation during incremental IDE evaluations.
- **Framework Abstraction:** The generator internally abstracts API differences among UI frameworks, synthesizing platform-compliant code from a single unified attribute (`[DependencyProperty]`).
- **Partial Class Composition:** The generator appends code using `partial` classes. It provides `partial void On...Changed(...)` methods exclusively for event hooking.

### Supported C# Language Versions and Runtime Requirements

| Category                             | Version               | Description & Supported Features                                                                               |
| :----------------------------------- | :-------------------- | :------------------------------------------------------------------------------------------------------------- |
| **Generator Host Runtime**           | **.NET Standard 2.0** | Executes within the Roslyn 4.3.0+ (.NET SDK 6.0 to 9.0+) compiler pipeline.                                    |
| **Minimum Requirement (Base)**       | **C# 8.0+**           | Non-generic attribute declarations (`typeof(T)` argument), nullable reference types, standard property output. |
| **Expression Expansion**             | **C# 9.0+**           | Automatic expansion of Target-Typed new expressions via `DefaultValueExpression = "new(...)"`.                 |
| **Generic Attributes (Recommended)** | **C# 11.0+**          | Generic attribute syntax such as `[DependencyProperty<T>]` and `[RoutedEvent<T>]`.                             |
| **Latest Syntax Support**            | **C# 13.0 (Preview)** | Full support for `partial` property syntax (`public partial int Value { get; set; }`).                         |

---

## II. Ubiquitous Language Glossary

The following terminology governs the generator's internal codebase.

| Term (English)        | Term (Code)                  | Description                                                                                  |
| :-------------------- | :--------------------------- | :------------------------------------------------------------------------------------------- |
| UI Framework          | `Framework`                  | Enum identifying the target platform (e.g., WPF, Uno, MAUI, Avalonia, WinUI).                |
| Dependency Property   | `DependencyProperty`         | Extended property mechanism for UI controls to retain state and support data binding.        |
| Attached Property     | `AttachedDependencyProperty` | Property mechanism allowing child elements to set values on parent elements.                 |
| Class Data            | `ClassData`                  | Metadata of the target class (owner) decorated with the attribute.                           |
| Property Data         | `DependencyPropertyData`     | The root data model containing complete metadata for the property to be generated.           |
| Component Model Data  | `ComponentModelData`         | UI/designer metadata such as `[Description]`, `[Category]`, and `[TypeConverter]`.           |
| Framework Metadata    | `FrameworkMetadataData`      | Settings for `FrameworkPropertyMetadataOptions` (e.g., `AffectsMeasure`) in WPF and others.  |
| Validation & Callback | `ValidationAndCallbackData`  | Configuration for behavior such as validation, coercion, and change callbacks (`OnChanged`). |
| Event Data            | `EventData`                  | Metadata for `RoutedEvent` and `WeakEvent`.                                                  |

---

## III. Domain Data Models

The incremental pipeline utilizes pure data models (DTOs) extracted from Roslyn's `SyntaxNode` and `ISymbol` structures.

> [!IMPORTANT]
> To maximize cache efficiency, you must define all DTOs as `readonly record struct` and implement value-based equality comparison via `IEquatable<T>`.

### Data Structure Design Guidelines

- **Separation of Concerns:** `DependencyPropertyData` contains many properties. To maintain modularity, it is divided into sub-models such as component models, UI metadata, XML documentation, validation/callbacks, and property modifiers (`PropertyModifiersData`).
- **Early Primitive Projection and Collection Equality:** To eliminate memory leaks and maximize cache hit ratios, Roslyn type instances are projected into primitive types or `EquatableArray<T>` rather than directly retained. For detailed performance rules, see **[05. Code Synthesis and Performance (IV. Performance Optimization Rules)](./05_synthesis_and_performance.md#iv-performance-optimization-rules)**.

### Main Data Models (DTOs)

#### 0. Comprehensive Architecture Model

The following diagram illustrates the overall class dependencies of the generator. See the subsequent subsections for details on individual models.

```mermaid
classDiagram
    direction LR
    class ClassData {
        +string Name
        +EquatableArray~ParentClassData~ ParentClasses
    }
    class DependencyPropertyData {
        +string Name
        +PropertyModifiersData Modifiers
        %% ComponentModel, FrameworkMetadata, ValidationAndCallbackData, XmlDocumentationData
    }
    class EventData {
        +string Name
        +string Strategy
    }
    class PropertyModifiersData { }
    class ComponentModelData { }
    class FrameworkMetadataData { }
    class ValidationAndCallbackData { }
    class XmlDocumentationData { }

    ClassData *-- DependencyPropertyData
    ClassData *-- EventData
    DependencyPropertyData *-- PropertyModifiersData
    DependencyPropertyData *-- ComponentModelData
    DependencyPropertyData *-- FrameworkMetadataData
    DependencyPropertyData *-- ValidationAndCallbackData
    DependencyPropertyData *-- XmlDocumentationData
```

#### 1. Class and Event Structure Models (`ClassData` / `EventData`)

```mermaid
classDiagram
    class ClassData {
        <<readonly record struct>>
        +string Namespace
        +string Name
        +string FullName
        +string Type
        +string Keyword
        +string NameWithTypeParameters
        +string Modifiers
        +string Version
        +bool IsStatic
        +Framework Framework
        +EquatableArray~ParentClassData~ ParentClasses
    }
    class ParentClassData {
        <<readonly record struct>>
        +string Keyword
        +string NameWithTypeParameters
        +string Modifiers
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
    ClassData *-- ParentClassData
    ClassData *-- EventData
```

#### 2. Core Dependency Property Structure (`DependencyPropertyData`)

```mermaid
classDiagram
    direction LR
    class DependencyPropertyData {
        <<readonly record struct>>
        +string Name
        +string Version
        +string Type
        +string ShortType
        +string? DefaultValue
        +string? DefaultValueDocumentation
        +Framework Framework
        +PropertyModifiersData Modifiers
        %% Other SubModels (ComponentModel, FrameworkMetadata, etc.)
    }
    class PropertyModifiersData {
        <<readonly record struct>>
        +bool IsValueType
        +bool IsSpecialType
        +bool IsReadOnly
        +bool IsDirect
        +bool IsAttached
        +bool IsAddOwner
        +bool IsPartialProperty
        +bool HidesBaseProperty
        +bool IsRequired
        +bool IsInitOnly
    }
    DependencyPropertyData *-- PropertyModifiersData
```

#### 3. Framework Metadata & UI Component Models

```mermaid
classDiagram
    direction LR
    class DependencyPropertyData {
        <<readonly record struct>>
        +string Name
        +string Version
        +string Type
        +string ShortType
        +string? DefaultValue
        +string? DefaultValueDocumentation
        +Framework Framework
        +PropertyModifiersData Modifiers
        %% Other SubModels (ComponentModel, FrameworkMetadata, etc.)
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
    DependencyPropertyData *-- ComponentModelData
    DependencyPropertyData *-- FrameworkMetadataData
```

#### 4. Validation, Callbacks, and XML Documentation

```mermaid
classDiagram
    direction LR
    class DependencyPropertyData {
        <<readonly record struct>>
        +string Name
        +string Version
        +string Type
        +string ShortType
        +string? DefaultValue
        +string? DefaultValueDocumentation
        +Framework Framework
        +PropertyModifiersData Modifiers
        %% Other SubModels (ComponentModel, FrameworkMetadata, etc.)
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
    DependencyPropertyData *-- ValidationAndCallbackData
    DependencyPropertyData *-- XmlDocumentationData
```

---

## IV. DTO Mapping Specification

This section details the explicit mapping between C# attributes and their corresponding Data Transfer Object (DTO) properties.

> [!TIP]
> Autonomous agents and AI assistants must use this specification as the ground truth when implementing bug fixes or feature additions.

### `[DependencyProperty]` Attribute Mapping

`DependencyPropertyDataBuilder.cs` and `PrepareData.cs` parse the attributes defined in user code. The generator stores the extracted data in the corresponding DTO fields.

#### 1. Root Properties (DependencyPropertyData & PropertyModifiersData)

| Attribute Argument / Property | DTO Target Field                      | Type      | Description                                                                  |
| :---------------------------- | :------------------------------------ | :-------- | :--------------------------------------------------------------------------- |
| Type Argument `<T>`           | `DependencyPropertyData.Type`         | `string`  | The property type (fully qualified).                                         |
| 1st Argument (Constructor)    | `DependencyPropertyData.Name`         | `string`  | The name of the dependency property (e.g., `"Text"`).                        |
| `DefaultValue`                | `DependencyPropertyData.DefaultValue` | `string?` | The default value such as string literals.                                   |
| `DefaultValueExpression`      | `DependencyPropertyData.DefaultValue` | `string?` | The default value as a C# expression like `new()`.                           |
| `IsReadOnly`                  | `Modifiers.IsReadOnly`                | `bool`    | Generates a read-only property using `DependencyPropertyKey` if `true`.      |
| `IsDirect`                    | `Modifiers.IsDirect`                  | `bool`    | Avalonia-specific. Indicates if it should be generated as a direct property. |
| (Partial property modifier)   | `Modifiers.IsPartialProperty`         | `bool`    | Target of C# 13 partial property syntax.                                     |
| (`new` modifier)              | `Modifiers.HidesBaseProperty`         | `bool`    | Explicitly hides an inherited member (`new` keyword).                        |

#### 2. Mapping to ValidationAndCallbackData

| Attribute Argument / Property | DTO Target Field                    | Type                     | Description                                                     |
| :---------------------------- | :---------------------------------- | :----------------------- | :-------------------------------------------------------------- |
| `OnChanged`                   | `ValidationAndCallbacks.OnChanged`  | `string`                 | Custom change callback method name.                             |
| `Coerce`                      | `ValidationAndCallbacks.Coerce`     | `bool`                   | Generates value coercion (`CoerceValueCallback`) if `true`.     |
| `Validate`                    | `ValidationAndCallbacks.Validate`   | `bool`                   | Generates value validation (`ValidateValueCallback`) if `true`. |
| `BindEvents`                  | `ValidationAndCallbacks.BindEvents` | `EquatableArray<string>` | List of control events to wire up.                              |

#### 3. Mapping to ComponentModelData

| Attribute Argument / Property | DTO Target Field               | Type      | Description                                        |
| :---------------------------- | :----------------------------- | :-------- | :------------------------------------------------- |
| `Description`                 | `ComponentModel.Description`   | `string?` | Generated as the `[Description("...")]` attribute. |
| `Category`                    | `ComponentModel.Category`      | `string?` | Generated as the `[Category("...")]` attribute.    |
| `TypeConverter`               | `ComponentModel.TypeConverter` | `string?` | Converter type name in the `typeof(...)` format.   |

#### 4. Mapping to FrameworkMetadataData (For WPF)

| Attribute Argument / Property | DTO Target Field                       | Type      | Description                                |
| :---------------------------- | :------------------------------------- | :-------- | :----------------------------------------- |
| `AffectsMeasure`              | `FrameworkMetadata.AffectsMeasure`     | `bool`    | Requests a layout update (Measure pass).   |
| `AffectsRender`               | `FrameworkMetadata.AffectsRender`      | `bool`    | Requests a redraw (Render pass).           |
| `BindsTwoWayByDefault`        | `FrameworkMetadata.DefaultBindingMode` | `string?` | The default binding mode (e.g., `TwoWay`). |

### Mapping to `ClassData` and `ParentClassData`

Information defining the parent class context is extracted into the `ClassData` and `ParentClasses` records.

| Target                     | DTO Target Field                   | Type                              | Description                                                    |
| :------------------------- | :--------------------------------- | :-------------------------------- | :------------------------------------------------------------- |
| Enclosing Namespace        | `ClassData.Namespace`              | `string`                          | The outer `namespace` declaration.                             |
| Class Name                 | `ClassData.Name`                   | `string`                          | The name of the `partial class` or `partial record`.           |
| Type Keyword               | `ClassData.Keyword`                | `string`                          | Declaration keyword such as `class`, `struct`, `record class`. |
| Type Parameters            | `ClassData.NameWithTypeParameters` | `string`                          | Generic type signature like `MyControl<T>`.                    |
| Class Modifiers            | `ClassData.Modifiers`              | `string`                          | Modifiers such as `public`, `internal`, `sealed`.              |
| `[AvaloniaObject]` etc.    | `ClassData.Framework`              | `Framework`                       | The type of framework used (`WPF`, `Avalonia`, etc.).          |
| Enclosing Parent Hierarchy | `ClassData.ParentClasses`          | `EquatableArray<ParentClassData>` | Outer nesting parent classes list with keywords and modifiers. |

---

Prev: [← 01. Design Rationale & FAQ](./01_faq_and_rationale.md) | [Index (Intro)](./intro.md) | Next: [03. Pipeline Architecture →](./03_pipeline_architecture.md)

