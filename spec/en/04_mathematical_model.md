# 04. Complexity Model of Incremental Generator

[English](./04_mathematical_model.md) | [日本語](../ja/04_mathematical_model.md) | [Table of Contents (Intro)](./intro.md)

To maintain the performance of the Roslyn Incremental Source Generator, developers must understand "how much complexity (allocation cost and processing time) each operation incurs".

This document explains the **Worst-Case Complexity** based on the generator architecture of this project, and the design intentions to avoid it.

## Ⅰ. Basic Complexity Model

The generator's processing is roughly divided into two phases:

1. **`PrepareData` (Data Extraction Phase)**: Extracts data from attributes and class structures.
2. **`SourceWriter` (Source Generation Phase)**: Generates C# code as strings from the extracted data.

Let $S$ be the number of source files to be compiled, $P$ be the average number of target properties (attributes) per file, and $N$ be the maximum number of `NamedArguments` specified in an attribute.

### 1. Complexity of `PrepareData`
During attribute analysis, the specified `NamedArguments` are traversed one by one to read their values.
For example, in the `GetNamedArgumentExpression` method of [`PrepareData.cs`](../../src/Kassyi.Generators.DependencyProperty/PrepareData.cs), an intentional `foreach` loop is used to avoid LINQ allocations:

```csharp
// [WHY] Avoid LINQ FirstOrDefault(predicate) to eliminate delegate allocations during syntax tree analysis.
foreach (var argument in attributeSyntax.ArgumentList.Arguments)
{
    var nameEquals = argument.NameEquals?.ToFullString().Trim('=', ' ', '\t', '\r', '\n');
    if (nameEquals == name)
    {
        return argument.Expression.ToFullString();
    }
}
```

Since this loop process occurs for each of the target configurations ($M$ items) against the number of arguments $N$, the time complexity is $O(M \times N)$, which simplifies to **$O(N)$**.
In this project, the extraction results are packed into **pure value-type DTOs** consisting of `readonly record struct` and `EquatableArray<T>`. This minimizes the memory allocation cost in this phase.

### 2. Complexity of `SourceWriter`
Let $K$ be the number of lines (or characters) of the generated source code.
For string concatenation, since it uses `SourceWriter` (which wraps `StringBuilder` internally) to write linearly while suppressing memory reallocation, the time complexity is **$O(K)$**.
Furthermore, by utilizing zero-allocation scopes like `using var _ = writer.ClassScope(@class);`, the load on Garbage Collection (GC) is also kept to $O(1)$ (almost zero).

---

## Ⅱ. Optimization via Incremental Cache and the "Worst-Case"

An Incremental Generator caches past compilation results and recalculates only the parts that have changed.
In the `RegisterAttributeGenerator` of [`GeneratorHelper.cs`](../../src/Kassyi.Generators.DependencyProperty/Generators/GeneratorHelper.cs), the pipeline is constructed as follows:

```csharp
combinedProvider
    .Combine(framework)
    .Combine(version)
    .SelectAndReportExceptions(prepareData, context, id) // O(N)
    .WhereNotNull()
    .SelectAndReportExceptions(getSourceCode, context, id) // O(K)
    .AddSource(context);
```

Assuming the cache hit ratio is $H$ ($0 \le H \le 1$), the actual total complexity $T$ flowing through this pipeline can be approximated as follows:

$$ T \approx (1 - H) \times O(S \times P \times (N + K)) $$

In an ideal state (only local changes due to typing), $H \approx 1$ for almost all files, resulting in $T \approx 0$.

### Worst-Case Scenario

What is the "heaviest (worst) operation" a developer might face?
It is when **widespread changes occur that cause the cache hit ratio $H = 0$**.

**Scenario:**
Suppose you modify the name, type, or attribute parameters (e.g., `DefaultValue`) of a `[DependencyProperty]` defined in a common class (like a Base class) that is widely used.

What happens then:
1. Roslyn determines that files depending on that class, or the entire compilation, are affected.
2. The cache is invalidated ($H = 0$) across all target files $S$.
3. For all properties $S \times P$, the $O(N)$ analysis and $O(K)$ source generation run again.

**Worst-Case Complexity:** **$O(S \times P \times (N + K))$**

In large solutions (where $S$ is in the thousands), when this worst-case scenario occurs, the IDE may freeze for several seconds.

---

## Ⅲ. Architectural Ingenuity to Prevent Performance Degradation

To prevent this worst-case complexity from occurring on every single keystroke, this generator is designed with the following strict rules:

1. **Prohibiting the inclusion of `ISymbol` or `SyntaxNode` in DTOs**
   - If Roslyn reference objects are included in the data model, the instance changes on every keystroke, causing `Equals` to return `false`.
   - This invalidates the caches of completely unrelated files, constantly triggering the worst-case complexity $O(S \times P)$, which leads to the "IDE freeze phenomenon".
2. **Perfect Implementation of `IEquatable` (`EquatableArray<T>`)**
   - By performing value-based comparisons for list data as well, it guarantees that "if it's semantically the same, use the cache", maintaining $H \approx 1$.
3. **Allocation-free code generation with `SourceWriter`**
   - Even if the worst case (full file regeneration) occurs, by making full use of `StringBuilder` pooling and `ref struct` (like `ClassScope`), it prevents secondary performance degradation (prolonged freezing) caused by GC spikes.
