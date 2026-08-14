# 03. Generation & Optimization

[English](./03_generation_and_optimization.md) | [日本語](../ja/03_generation_and_optimization.md) | [Index (Intro)](./intro.md)

## I. Interface Specification (Generated Code Structure)

The generator consumes extracted DTOs (`DependencyPropertyData`, etc.) and outputs framework-specific C# source files targeting WPF, MAUI, Avalonia, Uno, and WinUI.
Generated code extends user-declared partial classes.

### Boundaries & Contracts
- **Input (User Code)**: Classes declared with `partial` modifiers and decorated with `[DependencyProperty]` or related attributes. Optional `partial void On...Changed()` hook declarations.
- **Output (Generated Code)**: 
  - Static dependency property fields (`...Property`)
  - CLR property wrappers (`get` / `set`)
  - Property change callback wiring (`propertyChangedCallback`)
  - XML documentation comments

---

## II. Code Generation Engine (`SourceWriter` & `ClassScope`)

Code emission is handled by `SourceWriter` from `Kassyi.Generators.Extensions`.
To eliminate boilerplate while maintaining zero heap allocation and safe indentation scope lifecycle, the following patterns are standardized:

### 1. Outer Envelope Helper (`writer.ClassScope(@class)`)
Encapsulates the repetitive 3-step boilerplate (`#nullable enable` → `namespace` → `partial class`) into a single line across all generator source templates:

```csharp
// Outer envelope (#nullable enable, namespace, partial class) in 1 line
using var _ = writer.ClassScope(@class);

// Core member generation logic here...
```

- **Zero-Allocation**: Returns a `ref struct SourceWriterClassScope` that emits closing braces (`}`) on `Dispose()` without heap allocation.

### 2. Header-Direct Block Scoping (`writer.Scope(...)`)
Passes method or static constructor signatures directly into `Scope(...)`:

```csharp
using (writer.Scope($"static {@class.Name}()"))
{
    // Static constructor registration statements...
}
```

---

## III. Property & Callback Resolution Rules

### Target-Typed `new` Expansion for `DefaultValueExpression`
- When `DefaultValueExpression` in `[DependencyProperty<T>("Name", DefaultValueExpression = "...")]` begins with `new(...)` or `new (...)` (C# 9.0+ syntax), `PrepareData` automatically expands it into a fully-qualified global type name (`new global::...`).
  - Input: `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
  - Output: `new global::MyNamespace.MyProfile(1.5, 48.0)`
- This prevents verbose manual namespaces inside string literals, improving code clarity and refactoring resilience.

### Callback Method (`OnChanged` / `OnChanging`) Matching Rules

#### 1. Signature Rule Engine (`IMethodSignatureRule`)
Callback signatures are resolved using dedicated rule classes under `Rules/Signatures/` (`NoParametersRule`, `SingleParameterRule`, `DoubleParameterRule`, `TripleParameterRule`).

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
    // ⚠️ Unmatched signature (e.g. WPF standard static (DependencyObject, DependencyPropertyChangedEventArgs)):
    // Treated as an unrelated method, producing propertyChangedCallback: null.
    // 💡 To avoid silent misses, prefer explicit declaration (Pattern A).
    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
}
```

---

## IV. Performance Optimization Rules (Dos & Don'ts)

To maintain maximum IDE responsiveness during typing, adhere to the following generator performance guidelines:

### 🟢 Dos (Best Practices)
- **Use `ForAttributeWithMetadataName`**: Avoid obsolete syntax receivers. Filter declarations based on attributes to drastically limit generator invocation.
- **Early Primitive Projection**: Immediately transform `SyntaxNode` / `ISymbol` into primitives or readonly record structs.
- **Wrap Collections in `EquatableArray<T>`**: Ensure element-by-element equality checks for collections in DTOs.
- **Eliminate LINQ in Hot Paths**: Replace `.Select()`, `.Where()`, and `.Any()` with indexed `for` loops in hot extraction and formatting methods to avoid iterator allocations.
- **Pre-Cache Attribute Arguments**: Cache `NamedArguments` in dictionaries to achieve $O(1)$ property lookups.

### 🔴 Don'ts (Anti-Patterns)
- **❌ Do NOT retain `ISymbol` or `SyntaxNode` in DTOs**: Retaining compilation references causes severe memory leaks and forces 100% cache misses.
- **❌ Do NOT use raw `List<T>` or `T[]` in DTOs**: Reference comparisons invalidate incremental caching.
- **❌ Do NOT allocate intermediate strings in hot paths**: Avoid `string.Split()`, `string.Join()`, or unnecessary string buffers. Use `SourceWriter`, `StringBuilder`, and `stackalloc Span<char>`.

---

## V. Profiling Methodologies

1. **MSBuild Structured Log Analysis (`.binlog`)**
   ```bash
   dotnet build -c Release -bl:msbuild.binlog
   ```
   Open `msbuild.binlog` in [MSBuild Structured Log Viewer](https://msbuildlog.com/) to inspect generator execution time in milliseconds.

2. **BenchmarkDotNet Execution**
   Feed synthetic source trees into `CSharpGeneratorDriver` using BenchmarkDotNet to measure execution duration and memory allocation (Gen0/Gen1/Gen2, Allocated Bytes).
