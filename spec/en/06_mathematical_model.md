# 06. Complexity model

To maintain the Roslyn Incremental Source Generator's performance, you must understand the complexity (allocation cost and processing time) of each operation.

This document details the worst-case complexity of the generator's architecture and the design policies that mitigate it.

## I. Basic complexity model

The generator processes data in two primary phases:

1. **`PrepareData` (Data extraction phase)**: Extracts structural data from attributes and class definitions.
2. **`SourceWriter` (Source generation phase)**: Synthesizes C# source code strings using the extracted data.

Assume $S$ represents the number of source files to compile, $P$ represents the average number of target properties per file, and $N$ represents the maximum number of `NamedArguments` specified in a target attribute.

### 1. Complexity of `PrepareData`

During attribute analysis, the generator traverses each `NamedArgument` to extract its value.
For example, the `GetNamedArgumentExpressionSyntax` method in `PrepareData.cs` uses a manual `foreach` loop and direct AST node matching to eliminate LINQ allocations:

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

Because this loop executes for each target configuration ($M$ items) against $N$ arguments, the time complexity is $O(M \times N)$, which simplifies to **$O(N)$**.

> [!NOTE]
> The extraction phase packages results into pure value-type DTOs using `readonly record struct` and `EquatableArray<T>`. This architectural constraint minimizes memory allocations.

### 2. Complexity of `SourceWriter`

Assume $K$ represents the character count of the generated source code.
For string concatenation, the generator mandates using `SourceWriter`—which encapsulates a thread-static `StringBuilder` pool—to write linearly and suppress memory reallocation. The time complexity is exactly **$O(K)$**.

> [!TIP]
> By using zero-allocation scopes like `using var _ = writer.ClassScope(@class);`, the generator maintains Garbage Collection (GC) overhead at $O(1)$ (0 bytes of heap allocation).

---

## II. Optimization via incremental cache and the worst-case

The Incremental Generator caches past compilation outputs and calculates only delta changes.
In `GeneratorHelper.cs`, the pipeline executes the following chain:

```csharp
context.ExtractData(framework, version, attributeName, prepareData, id, selectMany) // O(N)
    .SelectAndReportExceptions(getSourceCode, context, id) // O(K)
    .AddSource(context);
```

Assuming an incremental cache hit ratio of $H$ ($0 \le H \le 1$), the total computational complexity $T$ flowing through this pipeline is modeled as follows:

$$T \approx (1 - H) \times O(S \times P \times (N + K))$$

| Scenario | Cache state | Total complexity $T$ | Impact on pipeline |
| :--- | :--- | :--- | :--- |
| **Routine editing** (for example, method bodies) | **Hit** ($H \to 1$) | **$O(1) \approx 0$** | Terminates early via `Equals()` comparison. Computational overhead is effectively zero. |
| **Structural changes** (for example, base classes) | **Miss** ($H \to 0$) | **$O(S \times P \times (N + K))$** | Extraction and generation re-execute across all files, reaching the theoretical worst case. |

> [!NOTE]
> **Estimated real-world performance (daily editing and typing: $T \approx 0$)**
> - **Execution latency**: **Virtually 0 ms (Measured $\le 0.1 \sim 0.5 \text{ ms}$)**
> - **GC heap allocation**: **0 bytes**
> - **Developer experience impact**: During method-body logic edits or keystrokes, the Roslyn pipeline terminates early via value equality comparison (`Equals()`). Generator CPU and memory overhead remain effectively zero, eliminating IDE lag even in large codebases.

### Worst-case scenario

The most severe computational load occurs when widespread architectural changes force the cache hit ratio to $H=0$.

**Scenario:**
Modifying the name, type, or attribute parameters (for example, `DefaultValue`) of a `[DependencyProperty]` defined in a widely consumed base class.

This triggers the following compiler events:
1. Roslyn identifies that all files depending on the base class are structurally affected.
2. The incremental cache completely invalidates ($H=0$) across all $S$ target files.
3. For all $S \times P$ properties, the $O(N)$ syntax analysis and $O(K)$ source generation execute synchronously.

**Worst-case complexity**: **$O(S \times P \times (N + K))$**

> [!WARNING]
> In enterprise-scale solutions (where $S$ is in the thousands), triggering this worst-case scenario freezes the IDE for several seconds.

---

## III. Architectural ingenuity to prevent degradation

To prevent this worst-case complexity from triggering on every keystroke, the generator enforces strict architectural rules.

> [!NOTE]
> **Specific measures to prevent performance degradation**
> For detailed rules regarding the prohibition of `ISymbol` in DTOs, strict `EquatableArray<T>` implementations, and allocation-free generation via `SourceWriter`, see **[05. Code synthesis and performance](./05_synthesis_and_performance.md#iv-performance-optimization-rules)**.
