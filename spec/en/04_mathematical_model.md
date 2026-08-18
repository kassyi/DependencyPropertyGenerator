# 04. Complexity Model of Incremental Generator

[English](./04_mathematical_model.md) | [日本語](../ja/04_mathematical_model.md) | [Index (Intro)](./intro.md)

To maintain the performance of the Roslyn Incremental Source Generator, developers must strictly understand the complexity (allocation cost and processing time) incurred by each operation.

This document dictates the Worst-Case Complexity derived from the generator's architecture and the design policies enforced to mitigate it.

## I. Basic Complexity Model

The generator's processing is architecturally divided into two primary phases:

1. **`PrepareData` (Data Extraction Phase)**: Extracts structural data from attributes and class definitions.
2. **`SourceWriter` (Source Generation Phase)**: Synthesizes C# source code strings utilizing the extracted data.

Let $S$ represent the number of source files to be compiled, $P$ represent the average number of target properties per file, and $N$ represent the maximum number of `NamedArguments` specified within a target attribute.

### 1. Complexity of `PrepareData`

During attribute analysis, the generator must traverse the specified `NamedArguments` individually to extract values.
For example, the `GetNamedArgumentExpressionSyntax` method within `PrepareData.cs` enforces a manual `foreach` loop and direct AST node matching to eliminate LINQ allocations:

```csharp
// [WHY] Avoid LINQ to eliminate array and enumerator allocations on every keystroke.
foreach (var argument in attributeSyntax.ArgumentList.Arguments)
{
    if (argument.NameEquals?.Name.Identifier.ValueText == name)
    {
        return argument.Expression;
    }
}
```

Because this loop executes for each target configuration ($M$ items) against the number of arguments $N$, the time complexity equates to $O(M \times N)$, which simplifies structurally to **$O(N)$**.

> [!NOTE]
> The extraction results are strictly packaged into pure value-type DTOs composed of `readonly record struct` and `EquatableArray<T>`. This architectural constraint minimizes memory allocation costs during the extraction phase.

### 2. Complexity of `SourceWriter`

Let $K$ represent the number of characters of the generated source code.
For string concatenation, the system mandates the use of `SourceWriter` (which encapsulates a thread-static `StringBuilder` pool) to write linearly while actively suppressing memory reallocation. The time complexity is exactly **$O(K)$**.

> [!TIP]
> By strictly utilizing zero-allocation scopes such as `using var _ = writer.ClassScope(@class);`, the operational load on Garbage Collection (GC) is forcefully maintained at $O(1)$ (0 Bytes heap allocation).

---

## II. Optimization via Incremental Cache and the "Worst-Case"

An Incremental Generator actively caches past compilation outputs and exclusively recalculates delta changes.
Within `GeneratorHelper.cs`, the pipeline executes the following execution chain:

```csharp
context.ExtractData(framework, version, attributeName, prepareData, id, selectMany) // O(N)
    .SelectAndReportExceptions(getSourceCode, context, id) // O(K)
    .AddSource(context);
```

Assuming an incremental cache hit ratio of $H$ ($0 \le H \le 1$), the actual total computational complexity $T$ flowing through this pipeline is modeled as follows:

$$ T \approx (1 - H) \times O(S \times P \times (N + K)) $$

- **Routine Editing (Cache Hit $H \to 1$):**
  $$ T = (1 - 1) \times O(S \times P \times (N + K)) + O(1) = O(1) \approx 0 $$
  The pipeline terminates early via value equality comparison (`Equals()`), reducing total computational complexity $T$ effectively to $O(1) \approx 0$.
- **Structural Changes (Cache Miss $H \to 0$):**
  $$ T = (1 - 0) \times O(S \times P \times (N + K)) = O(S \times P \times (N + K)) $$
  Extraction and source generation are re-executed across all files, driving total computational complexity $T$ to its theoretical worst case.

> [!NOTE]
> **Estimated Real-World Performance (Daily Editing & Typing: $T \approx 0$)**
> - **Execution Latency:** **Virtually 0 ms (Measured $\le 0.1 \sim 0.5 \text{ ms}$)**
> - **GC Heap Allocation:** **Strictly 0 Bytes**
> - **Developer Experience Impact:** During method-body logic edits or keystrokes, the Roslyn pipeline terminates early via value equality comparison (`Equals()`). Generator CPU and memory overhead remain effectively zero, eliminating IDE lag even in large codebases.

### Worst-Case Scenario

The most severe computational load occurs when widespread architectural changes force the cache hit ratio to $H = 0$.

**Scenario:**
Modifying the name, type, or attribute parameters (e.g., `DefaultValue`) of a `[DependencyProperty]` defined within a widely consumed common Base class.

This action triggers the following compiler events:
1. Roslyn identifies that all files depending on the Base class are structurally affected.
2. The incremental cache is totally invalidated ($H = 0$) across all target files $S$.
3. For all properties $S \times P$, the $O(N)$ syntax analysis and $O(K)$ source generation execute synchronously.

**Worst-Case Complexity:** **$O(S \times P \times (N + K))$**

> [!WARNING]
> In enterprise-scale solutions (where $S$ is in the thousands), triggering this worst-case scenario will cause the IDE to freeze for several seconds.

---

## III. Architectural Ingenuity to Prevent Degradation

To absolutely prevent this worst-case complexity from triggering on every keystroke, the generator strictly enforces the following architectural rules:

> [!CAUTION]
> **1. Prohibition of `ISymbol` or `SyntaxNode` within DTOs**
> If Roslyn reference objects pollute the data model, the underlying instance mutates on every keystroke, forcing `Equals` to return `false`. This immediately invalidates caches of completely unrelated files, continuously triggering the worst-case complexity $O(S \times P)$, resulting in continuous IDE freezes.

> [!IMPORTANT]
> **2. Strict Implementation of `IEquatable` (`EquatableArray<T>`)**
> Enforcing deep value-based comparisons for array data mathematically guarantees that semantically identical code yields cache hits, artificially maintaining $H \approx 1$.

> [!TIP]
> **3. Allocation-Free Generation via `SourceWriter`**
> Even if a full pipeline flush (worst-case scenario) occurs, aggressive `StringBuilder` pooling and zero-allocation `ref struct` wrappers prevent secondary performance degradation caused by GC spikes.
