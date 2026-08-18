# 03. Generation and Optimization

[English](./03_generation_and_optimization.md) | [日本語](../ja/03_generation_and_optimization.md) | [Index (Intro)](./intro.md)

## I. Interface Specification and Generated Code Structure

The generator consumes extracted DTOs (such as `DependencyPropertyData`) and outputs framework-specific C# source files targeting WPF, MAUI, Avalonia, Uno, and WinUI. The generated code extends user-declared `partial` classes.

### Boundaries and Contracts
**Input (User Code)** consists of classes declared with `partial` modifiers and decorated with `[DependencyProperty]` or related attributes. It also accepts optional `partial void On...Changed()` hook declarations.
**Output (Generated Code)** includes the following elements:
- Static dependency property fields (`...Property`)
- CLR property wrappers (`get` / `set`)
- Property change callback wiring (`propertyChangedCallback`)
- XML documentation comments

---

## II. Code Generation Engine (`SourceWriter` and `ClassScope`)

The system handles code emission using `SourceWriter` from `Kassyi.Generators.Extensions`. To eliminate boilerplate while maintaining zero heap allocation and safe indentation scope lifecycle, we standardise the following patterns.

### 1. Outer Envelope Helper (`writer.ClassScope(@class)`)
The generator encapsulates the repetitive three-step boilerplate (`#nullable enable` → `namespace` → `partial class`) into a single line across all source templates.

```csharp
// Outer envelope (#nullable enable, namespace, partial class) generated in 1 line
using var _ = writer.ClassScope(@class);

// Core member generation logic goes here
```

This pattern operates with zero allocation. It returns a `ref struct SourceWriterClassScope` that emits closing braces (`}`) on `Dispose()` without any heap allocation.

### 2. Header-Direct Block Scoping (`writer.Scope(...)`)
The generator passes method or static constructor signatures directly into the `Scope(...)` method.

```csharp
using (writer.Scope($"static {@class.Name}()"))
{
    // Static constructor registration statements
}
```

---

## III. Property and Callback Resolution Rules

### Target-Typed `new` Expansion for `DefaultValueExpression`
When `DefaultValueExpression` in `[DependencyProperty<T>("Name", DefaultValueExpression = "...")]` begins with `new(...)` or `new (...)` (C# 9.0+ syntax), the `PrepareData` extraction phase automatically expands it into a fully-qualified global type name (`new global::...`).

- Input: `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
- Output: `new global::MyNamespace.MyProfile(1.5, 48.0)`

This prevents verbose manual namespaces inside string literals, improving code clarity and refactoring resilience when instantiating types from other namespaces.

### Callback Method (`OnChanged` / `OnChanging`) Matching Rules

#### 1. Signature Rule Engine (`IMethodSignatureRule`)
The generator resolves callback signatures using dedicated rule classes under `Rules/Signatures/` (`NoParametersRule`, `SingleParameterRule`, `DoubleParameterRule`, `TripleParameterRule`).

```csharp
// ✅ 0 parameters (NoParametersRule)
partial void OnTextChanged();

// ✅ 1 parameter (SingleParameterRule: newValue OR EventArgs)
partial void OnTextChanged(string newValue);
partial void OnTextChanged(DependencyPropertyChangedEventArgs e);

// ✅ 2 parameters (DoubleParameterRule: oldValue & newValue / sender & newValue / sender & EventArgs)
partial void OnTextChanged(string oldValue, string newValue);
partial void OnTextChanged(MyControl sender, string newValue);
partial void OnTextChanged(MyControl sender, DependencyPropertyChangedEventArgs e);

// ✅ 3 parameters (TripleParameterRule: sender, oldValue & newValue)
partial void OnTextChanged(MyControl sender, string oldValue, string newValue);

// ❌ 4+ parameters are unsupported (Ignored due to lack of available arguments)
void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra);
```

#### 2. Error Reporting vs. Silent Fallback

```csharp
// ----------------------------------------------------------------------------
// Pattern A: Explicit specification via OnChanged parameter
// ----------------------------------------------------------------------------
[DependencyProperty<string>("Text", OnChanged = nameof(OnTextChanged))]
public partial class MyControl : UserControl
{
    // 🚨 Invalid signature (e.g. 4 parameters) or missing method:
    // Generator emits compile error #error DPG0001 to immediately fail the build.
    private void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra) { }
}

// ----------------------------------------------------------------------------
// Pattern B: Convention-based partial void On...Changed() auto-discovery
// ----------------------------------------------------------------------------
[DependencyProperty<string>("Text")]
public partial class MyControl : UserControl
{
    // 🚨 Unmatched signature (e.g. WPF standard (DependencyObject, DependencyPropertyChangedEventArgs)):
    // Previously treated as an unrelated method (silently ignored), but now emits compile error #error DPG0007
    // to immediately fail the build and prevent silent callback bugs.
    private void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
}
```

#### 3. Troubleshooting Callback Signature Mismatch Errors (DPG0007) (Agentic Ground Truth)

If a method matches the `On...Changed` naming convention but has an unsupported parameter signature, the generator emits a `DPG0007` error to stop the build, preventing a silent bug where the event is ignored (resulting in `propertyChangedCallback: null`).

The most common cause of this issue is when developers habitually define a callback using the standard WPF signature, `private void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)`. However, to enforce strict type safety, the generator's rule engine intentionally does not support methods where the first argument is a generic `DependencyObject`.

To resolve this error, you must change the first argument of your method from `DependencyObject` to the specific type of the class defining the property (for example, `MyControl sender`). Furthermore, since the generator automatically emits a static proxy method behind the scenes to wire up the event, your custom callback must be defined as a standard instance method rather than a static one.

---

## IV. Performance Optimization Rules

To maintain maximum IDE responsiveness during typing, adhere to the following generator performance guidelines.

### Benchmark-Backed Principles
- **AST Node Traversal over String Parsing**: For expression analysis, direct `ExpressionSyntax` AST traversal completely avoids re-tokenization and intermediate syntax tree allocations, making it significantly faster and lighter on memory compared to re-parsing extracted strings (`SyntaxFactory.ParseExpression()`). Avoid string re-parsing in generator hot paths.
- **SourceWriter over SyntaxFactory for Code Generation**: In code generation hot paths, emitting code directly via [`SourceWriter`](../../src/Kassyi.Generators.Extensions/SourceWriter.cs) (custom interpolated string handler) is preferred over heavy syntax tree construction and formatting via `SyntaxFactory.NormalizeWhitespace().ToFullString()`. (Note: `SyntaxFactory` remains acceptable in non-hot paths or unit tests).

### Dos (Best Practices)
- **Use `ForAttributeWithMetadataName`**: Filter declarations based on attributes to drastically limit generator invocation. Avoid obsolete syntax receivers.
- **Early Primitive Projection**: Immediately transform `SyntaxNode` or `ISymbol` into primitives or readonly record structs during the extraction phase.
- **Wrap Collections in `EquatableArray<T>`**: Ensure element-by-element equality checks for collections in DTOs.
- **Eliminate LINQ in Hot Paths**: Replace `.Select()`, `.Where()`, and `.Any()` with indexed `for` loops in hot extraction and formatting methods to avoid iterator allocations.
- **Pre-Cache Attribute Arguments**: Cache `NamedArguments` in dictionaries to achieve $O(1)$ property lookups.

### Don'ts (Anti-Patterns)
- **Do NOT retain `ISymbol` or `SyntaxNode` in DTOs**: Retaining compilation references causes severe memory leaks and forces 100% cache misses.
- **Do NOT use raw `List<T>` or `T[]` in DTOs**: Reference comparisons invalidate incremental caching.
- **Do NOT allocate intermediate strings in hot paths**: Avoid `string.Split()`, `string.Join()`, or unnecessary string buffers. Use `SourceWriter`, `StringBuilder`, and `stackalloc Span<char>` to prevent GC spikes.

---

## V. Profiling Methodologies

Use the following methodologies to investigate performance bottlenecks.

1. **MSBuild Structured Log Analysis (`.binlog`)**
   ```bash
   dotnet build -c Release -bl:msbuild.binlog
   ```
   Open `msbuild.binlog` in MSBuild Structured Log Viewer to inspect generator execution time in milliseconds.

2. **BenchmarkDotNet Execution**
   Feed synthetic source trees into `CSharpGeneratorDriver` using BenchmarkDotNet to measure execution duration and memory allocation (Gen0/Gen1/Gen2, Allocated Bytes).
