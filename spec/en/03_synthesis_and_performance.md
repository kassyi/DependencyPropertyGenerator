# 03. Code Synthesis and Performance Optimization

[English](./03_synthesis_and_performance.md) | [日本語](../ja/03_synthesis_and_performance.md) | [Index (Intro)](./intro.md)

## I. Interface Specification and Generated Code Structure

The generator consumes extracted Data Transfer Objects (DTOs), such as `DependencyPropertyData`, to emit framework-specific C# source files targeting WPF, MAUI, Avalonia, Uno, and WinUI. This generated code extends user-declared `partial` classes.

### Boundaries and Contracts

**Input Constraints**
The generator processes user code that meets the following criteria:
- Classes declared with the `partial` modifier.
- Classes decorated with `[DependencyProperty]` or related attributes.
- Optional `partial void On...Changed()` hook declarations.

**Output Artifacts**
The generated code includes the following structural elements:
- Static dependency property fields, typically suffixed with `Property`.
- CLR property wrappers implementing `get` and `set` accessors.
- Property change callback wiring bound to `propertyChangedCallback`.
- Comprehensive XML documentation comments.

---

## II. Code Generation Engine

The engine handles code emission using `SourceWriter` from the `Kassyi.Generators.Extensions` namespace. It standardizes structural patterns to eliminate boilerplate, ensuring zero heap allocation and safely managing indentation scopes.

### 1. Outer Envelope Helper 

The generator condenses repetitive boilerplate into a single line across all source templates. This envelope includes the `#nullable enable` directive, the `namespace` declaration, nested parent classes, and the target `partial class` definition.

```csharp
// The ClassScope helper generates the complete outer envelope in one operation.
using var _ = writer.ClassScope(@class);

// Core member generation logic follows.
```

> [!TIP]
> This pattern guarantees **zero memory allocation**. The `ClassScope` method returns a `ref struct SourceWriterClassScope`. Upon disposal, this struct emits closing braces for all opened nested classes and namespaces without allocating on the heap.

> [!NOTE]
> If a target class is nested within outer parent classes (defined in `ClassData.ParentClasses`), `ClassScope` opens enclosing partial classes from the outermost to the innermost scope. It automatically closes them in reverse order upon disposal.

### 2. Header-Direct Block Scoping

The generator passes method or static constructor signatures directly into the `Scope` method to manage block indentation.

```csharp
using (writer.Scope($"static {@class.Name}()"))
{
    // Static constructor registration statements
}
```

---

## III. Property and Callback Resolution Rules

### Target-Typed Object Creation Expansion

The `PrepareData` extraction phase automatically expands target-typed `new` expressions. If a `DefaultValueExpression` starts with `new(...)` or `new (...)` (using C# 9.0+ syntax), the pipeline transforms it into a fully-qualified global type name.

**Example Transformation:**
- **Input:** `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
- **Output:** `new global::MyNamespace.MyProfile(1.5, 48.0)`

This mechanism improves code clarity by eliminating verbose manual namespaces within string literals and increases refactoring resilience when instantiating types from external namespaces.

### C# 13 Partial Property Syntax Resolution

When user code defines a partial property using C# 13 syntax (e.g., `public partial int Value { get; set; }`), the generator detects `Modifiers.IsPartialProperty` and emits the implementation block. This transparently supports both standard and modern partial property declarations.


### Callback Method Matching Rules

#### 1. Signature Rule Engine

The generator resolves callback signatures using dedicated rule classes in the `Rules/Signatures/` directory. The engine strictly enforces parameter limits and type requirements.

**Supported Signatures:**
- **0 parameters:** Handled by `NoParametersRule`.
- **1 parameter:** Handled by `SingleParameterRule`. Accepts the new value or `EventArgs`.
- **2 parameters:** Handled by `DoubleParameterRule`. Accepts pairs such as old and new value, sender and new value, or sender and `EventArgs`.
- **3 parameters:** Handled by `TripleParameterRule`. Accepts sender, old value, and new value.

> [!WARNING]
> The rule engine ignores signatures with 4 or more parameters because it lacks the internal arguments to satisfy them.

```csharp
// Example of a valid 2-parameter signature:
partial void OnTextChanged(string oldValue, string newValue);

// Example of an unsupported 4-parameter signature:
void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra);
```

#### 2. Error Reporting and Compilation Safety

The generator enforces strict compilation errors for invalid callback signatures to prevent silent runtime failures.

**Explicit Specification**
If you explicitly define a callback via the `OnChanged` parameter, an invalid signature or missing method triggers the `DPG0001` compilation error, instantly stopping the build.

**Convention-Based Discovery**
If you rely on the auto-discovery of `partial void On...Changed()` methods, an unmatched signature triggers the `DPG0007` compilation error.

> [!IMPORTANT]
> **Elimination of Silent Callback Failure ([HavenDV#165](https://github.com/HavenDV/DependencyPropertyGenerator/issues/165))**
> In the upstream generator, defining a callback with an unsupported signature (like the WPF-standard `(DependencyObject, DependencyPropertyChangedEventArgs)`) emitted `propertyChangedCallback: null` without warnings, causing silent runtime failures. This specification prevents these failures by surfacing them immediately as compile-time errors (`DPG0001` / `DPG0007`).

#### 3. Resolving Callback Signature Mismatches 

The `DPG0007` diagnostic prevents a silent bug where event registration ignores the target callback, resulting in a null `propertyChangedCallback`.

A common cause of this diagnostic is defining a callback using the standard WPF signature with a generic `DependencyObject` parameter. To enforce type safety, the rule engine explicitly rejects generic `DependencyObject` arguments.

**Resolution Steps:**
1. Update the callback's first parameter to use the exact type of the class defining the property.
2. Ensure the custom callback is an instance method; the generator automatically emits a static proxy method to wire up the event.

---

## IV. Performance Optimization Rules

To maintain IDE responsiveness during typing, the architecture enforces strict performance guidelines. Adhere to these principles when extending the generator.

> [!NOTE]
> **Historical Benchmarks and Optimization Reports**
> Detailed phase-by-phase benchmark measurements and performance improvement reports conducted on this architecture are documented in [`tests/Kassyi.Generators.DependencyProperty.Benchmarks`](../../tests/Kassyi.Generators.DependencyProperty.Benchmarks) (specifically under the `Reports/` directory spanning `Phase0` through `Phase5`).

### Benchmark-Backed Principles

> [!TIP]
> **AST Node Traversal over String Parsing**
> For expression analysis, use direct `ExpressionSyntax` Abstract Syntax Tree (AST) traversal. This completely avoids re-tokenization and intermediate syntax tree allocations. It operates significantly faster and uses less memory than re-parsing strings with `SyntaxFactory.ParseExpression()`. String re-parsing is prohibited in generator hot paths.

> [!TIP]
> **SourceWriter over SyntaxFactory for Code Generation**
> In code generation hot paths, emit code directly using the custom interpolated string handler `SourceWriter`. This outperforms heavy syntax tree construction and formatting via `SyntaxFactory.NormalizeWhitespace().ToFullString()`.

> [!NOTE]
> You may still use `SyntaxFactory` in non-hot paths or unit testing environments.

### Best Practices

- **Targeted Declaration Filtering:** Use `ForAttributeWithMetadataName` to filter declarations by attribute. This drastically limits generator invocations. Obsolete syntax receivers are prohibited.
- **Early Primitive Projection:** Immediately transform `SyntaxNode` or `ISymbol` instances into primitives or `readonly record struct`s during extraction.
- **Collection Equality:** Wrap all collections in `EquatableArray<T>` to enforce element-by-element equality checks within DTOs.
- **LINQ Elimination:** Replace LINQ operators (e.g., `.Select()`, `.Where()`, `.Any()`) with indexed `for` loops in hot extraction and formatting methods to prevent iterator allocations.
- **Pre-Cache Attribute Arguments:** Cache `NamedArguments` using dictionaries to guarantee $O(1)$ property lookups.

### Architectural Anti-Patterns

> [!CAUTION]
> **Retaining Compilation References**
> Never retain `ISymbol` or `SyntaxNode` in DTOs. This causes severe memory leaks and forces 100% cache misses in the incremental pipeline.

> [!CAUTION]
> **Mutable Collection Types**
> Never use raw `List<T>` or `T[]` in DTOs. Their default reference comparisons invalidate the incremental cache.

> [!WARNING]
> **Intermediate String Allocations**
> Avoid allocating intermediate strings in hot paths (e.g., `string.Split()` or `string.Join()`). Use `SourceWriter`, `StringBuilder`, and `stackalloc Span<char>` to prevent GC spikes.

---

## V. Profiling Methodologies

Use the following diagnostic methods to investigate performance bottlenecks in the generator pipeline.

**1. MSBuild Structured Log Analysis**
Generate a binary log during the build to inspect generator execution time. Analyze the resulting `msbuild.binlog` using the MSBuild Structured Log Viewer.
```bash
dotnet build -c Release -bl:msbuild.binlog
```

**2. BenchmarkDotNet Execution**
Feed synthetic source trees into `CSharpGeneratorDriver` using BenchmarkDotNet. This accurately measures execution duration and memory allocation across the Gen0, Gen1, and Gen2 heaps.
