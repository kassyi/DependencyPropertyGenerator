# 03. Code Synthesis and Performance Optimization

[English](./03_synthesis_and_performance.md) | [日本語](../ja/03_synthesis_and_performance.md) | [Index (Intro)](./intro.md)

## I. Interface Specification and Generated Code Structure

The generator consumes extracted Data Transfer Objects, including `DependencyPropertyData`. It then produces framework-specific C# source files targeting WPF, MAUI, Avalonia, Uno, and WinUI. This generated code extends user-declared `partial` classes.

### Boundaries and Contracts

**Input Constraints**
The generator processes user code meeting the following criteria:
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

The system handles code emission using `SourceWriter` from the `Kassyi.Generators.Extensions` namespace. The architecture standardizes specific structural patterns to eliminate boilerplate code. This approach ensures zero heap allocation and enforces a safe indentation scope lifecycle.

### 1. Outer Envelope Helper 

The generator encapsulates repetitive structural boilerplate into a single line across all source templates. This structural envelope includes the `#nullable enable` directive, the `namespace` declaration, any outer nested parent classes, and the target `partial class` definition.

```csharp
// The ClassScope helper generates the complete outer envelope in one operation.
using var _ = writer.ClassScope(@class);

// Core member generation logic follows.
```

> [!TIP]
> This pattern operates strictly with **zero memory allocation**. The `ClassScope` method returns a `ref struct SourceWriterClassScope`. Upon disposal, this struct emits the necessary closing braces for all opened nested classes and namespaces without any heap allocation.

> [!NOTE]
> When a target class is nested within outer parent classes defined in `ClassData.ParentClasses`, `ClassScope` systematically opens enclosing partial classes from the outermost to the innermost scope. It then automatically closes them in reverse order upon disposal.

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

The `PrepareData` extraction phase automatically expands target-typed `new` expressions. If a `DefaultValueExpression` begins with `new(...)` or `new (...)` utilizing C# 9.0+ syntax, the pipeline transforms it into a fully-qualified global type name.

**Example Transformation:**
- **Input:** `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
- **Output:** `new global::MyNamespace.MyProfile(1.5, 48.0)`

This expansion mechanism improves code clarity by eliminating verbose manual namespaces inside string literals. It also increases refactoring resilience when instantiating types from external namespaces.

### C# 13 Partial Property Syntax Resolution

When user code defines a partial property utilizing C# 13 syntax (e.g., `public partial int Value { get; set; }`), the generator automatically detects `Modifiers.IsPartialProperty` and emits the property's implementation block. This transparently supports both standard property synthesis and modern partial property declarations.


### Callback Method Matching Rules

#### 1. Signature Rule Engine

The generator resolves callback signatures utilizing dedicated rule classes located under the `Rules/Signatures/` directory. The engine strictly enforces parameter limits and type requirements.

**Supported Signatures:**
- **0 parameters:** Handled by `NoParametersRule`.
- **1 parameter:** Handled by `SingleParameterRule`. Accepts the new value or `EventArgs`.
- **2 parameters:** Handled by `DoubleParameterRule`. Accepts pairs such as old and new value, sender and new value, or sender and `EventArgs`.
- **3 parameters:** Handled by `TripleParameterRule`. Accepts sender, old value, and new value.

> [!WARNING]
> Signatures defining 4 or more parameters remain unsupported and are explicitly ignored by the rule engine due to a lack of available internal arguments.

```csharp
// Example of a valid 2-parameter signature:
partial void OnTextChanged(string oldValue, string newValue);

// Example of an unsupported 4-parameter signature:
void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra);
```

#### 2. Error Reporting and Compilation Safety

The generator prevents silent runtime failures by enforcing strict compilation errors for invalid callback signatures.

**Explicit Specification**
When a callback is explicitly defined via the `OnChanged` parameter, an invalid signature or missing method triggers the `DPG0001` compilation error. This immediately stops the build.

**Convention-Based Discovery**
When relying on the auto-discovery of `partial void On...Changed()` methods, an unmatched signature triggers the `DPG0007` compilation error.

> [!IMPORTANT]
> **Elimination of Silent Callback Failure ([HavenDV#165](https://github.com/HavenDV/DependencyPropertyGenerator/issues/165))**
> In the upstream generator, defining a callback with an unsupported signature (such as the WPF-standard `(DependencyObject, DependencyPropertyChangedEventArgs)`) silently emitted `propertyChangedCallback: null` without diagnostic warnings, causing change notifications to fail silently at runtime. This specification strictly prevents such failures by surfacing them immediately as compile-time errors (`DPG0001` / `DPG0007`).

#### 3. Resolving Callback Signature Mismatches 

The `DPG0007` diagnostic prevents a silent bug where the event registration ignores the target callback, resulting in a null `propertyChangedCallback`.

A frequent cause of this diagnostic involves defining a callback using the standard WPF signature with a generic `DependencyObject` parameter. To enforce strict type safety, the generator's rule engine explicitly rejects generic `DependencyObject` arguments.

**Resolution Steps:**
1. Update the first parameter of the callback method to utilize the exact type of the class defining the property.
2. Ensure the custom callback is defined as a standard instance method. The generator automatically emits a static proxy method to wire up the event.

---

## IV. Performance Optimization Rules

To maintain maximum IDE responsiveness during typing, the architecture enforces strict performance guidelines. Adhere to these principles when extending the generator.

> [!NOTE]
> **Historical Benchmarks and Optimization Reports**
> Detailed phase-by-phase benchmark measurements and performance improvement reports conducted on this architecture are documented in [`tests/Kassyi.Generators.DependencyProperty.Benchmarks`](../../tests/Kassyi.Generators.DependencyProperty.Benchmarks) (specifically under the `Reports/` directory spanning `Phase0` through `Phase5`).

### Benchmark-Backed Principles

> [!TIP]
> **AST Node Traversal over String Parsing**
> For expression analysis, utilize direct `ExpressionSyntax` Abstract Syntax Tree traversal. This approach completely avoids re-tokenization and intermediate syntax tree allocations. It operates significantly faster and consumes less memory compared to re-parsing extracted strings with `SyntaxFactory.ParseExpression()`. String re-parsing is strictly prohibited in generator hot paths.

> [!TIP]
> **SourceWriter over SyntaxFactory for Code Generation**
> In code generation hot paths, emit code directly utilizing the custom interpolated string handler `SourceWriter`. This approach outperforms heavy syntax tree construction and formatting via `SyntaxFactory.NormalizeWhitespace().ToFullString()`. 

> [!NOTE]
> The use of `SyntaxFactory` remains acceptable for non-hot paths or within unit testing environments.

### Best Practices

- **Targeted Declaration Filtering:** Utilize `ForAttributeWithMetadataName` to filter declarations based on attributes. This drastically limits generator invocation. The use of obsolete syntax receivers is prohibited.
- **Early Primitive Projection:** Immediately transform `SyntaxNode` or `ISymbol` instances into primitives or readonly record structs during the initial extraction phase.
- **Collection Equality:** Wrap all collection types in `EquatableArray<T>` to enforce element-by-element equality checks within Data Transfer Objects.
- **LINQ Elimination:** Replace LINQ operators like `.Select()`, `.Where()`, and `.Any()` with indexed `for` loops in hot extraction and formatting methods. This prevents iterator allocations.
- **Pre-Cache Attribute Arguments:** Cache `NamedArguments` utilizing dictionaries to guarantee $O(1)$ property lookups.

### Architectural Anti-Patterns

> [!CAUTION]
> **Retaining Compilation References**
> Do not retain `ISymbol` or `SyntaxNode` within Data Transfer Objects. This practice causes severe memory leaks and forces 100% cache misses in the incremental pipeline.

> [!CAUTION]
> **Mutable Collection Types**
> Do not utilize raw `List<T>` or `T[]` in Data Transfer Objects. Default reference comparisons invalidate incremental caching.

> [!WARNING]
> **Intermediate String Allocations**
> Do not allocate intermediate strings in hot paths. Avoid operations like `string.Split()` or `string.Join()`. Utilize `SourceWriter`, `StringBuilder`, and `stackalloc Span<char>` to prevent garbage collection spikes.

---

## V. Profiling Methodologies

The following diagnostic methodologies dictate how to investigate performance bottlenecks within the generator pipeline.

**1. MSBuild Structured Log Analysis**
Generate a binary log during the build process to inspect generator execution time. Analyze the resulting `msbuild.binlog` utilizing the MSBuild Structured Log Viewer.
```bash
dotnet build -c Release -bl:msbuild.binlog
```

**2. BenchmarkDotNet Execution**
Feed synthetic source trees into `CSharpGeneratorDriver` utilizing BenchmarkDotNet. This methodology accurately measures execution duration and memory allocation across Gen0, Gen1, and Gen2 heaps.
