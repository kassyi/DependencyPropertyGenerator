# Chapter 01: Design Rationale & FAQ

This chapter outlines the architectural philosophy behind the zero-allocation v4 generator and answers frequently asked questions regarding API design, code synthesis, and integration with modern .NET features.

## 1. Design Philosophy

Writing `DependencyProperty` in XAML frameworks such as .NET MAUI, WPF, WinUI, and Avalonia is notoriously verbose. The primary goal of `DependencyPropertyGenerator` is to eliminate this boilerplate without degrading IDE responsiveness at scale.

To achieve high-throughput, zero-allocation generation, the v4 pipeline relies on the following aggressive architectural shifts:

### Flat Code Synthesis Without Indentation

The generator drops `NormalizeWhitespace()` and manual indentation entirely. It outputs flat, left-aligned code.

- **Performance win:** Eliminates thousands of whitespace string allocations on the hot path.
- **AI/LLM win:** Lightweight models such as Gemini Flash often hallucinate or misalign whitespace when predicting code. Flat output ensures deterministic, stable generation without brittle whitespace matching.

### Direct Token Streaming with Zero AST Mutation

We strictly avoid `SyntaxFactory` mutations for code synthesis. Whether generating class definitions or resolving dynamic `DefaultValueExpression` declarations such as converting `new()` to explicit types, we extract tokens from the parsed AST and stream them straight into a custom `SourceWriter`.

- **Zero Gen2 Allocations:** Bypassing AST construction completely prevents Gen2 GC spikes.

### `ref struct` Source Writers

Generation logic runs on stack-allocated `SourceWriter` and `ClassScope` wrappers, completely bypassing heap allocations for string assembly.

> [!NOTE]
> For detailed code specifications on token streaming, `SourceWriter` / `ClassScope` implementation conventions, and micro-benchmark metrics comparing AST mutation vs. token streaming, see **[05. Code Synthesis and Performance](./05_synthesis_and_performance.md)**.

## 2. Frequently Asked Questions

### Why support multi-platform XAML with a single attribute?

To enable **"Write Once, Run on Any XAML Platform"**. 
By isolating the data extraction phase into pure DTOs and delegating output to platform-specific strategies, a single `[DependencyProperty]` attribute can synthesize `DependencyProperty.Register` for WPF, `AvaloniaProperty.Register` for Avalonia, and `BindableProperty.Create` for MAUI. This drastically reduces `#if` boilerplate for OSS library authors and enterprise teams migrating between UI frameworks.

### What happens if Avalonia releases an official source generator? (e.g., Avalonia 12.2)

When Avalonia 12.2 introduces official source generator support, we will evaluate its adoption and community consensus. 
While official generators provide platform-native excellence, **this library will continue to maintain `AvaloniaFrameworkGenerator`** as long as there is value in a unified, multi-targeted `[DependencyProperty]` attribute shared across WPF, MAUI, and Avalonia codebases. Deprecation will only be considered if the community fully standardizes on the official generator and multi-target demand diminishes.

### Why use class-level attributes instead of field-level attributes like in MVVM Toolkit or C# 13 partial properties?

Class-level attributes provide a unified mental model for declaring both standard Dependency Properties and Attached Properties, which do not map cleanly to backing fields. However, C# 13 `partial property` support is natively implemented and works seamlessly alongside class-level attributes, utilizing the same underlying AST parsing logic.

### Does killing indentation with flat output ruin the Go To Definition or debugging experience?

No. Formatting generated code is the IDE/formatter's responsibility. It is inefficient to allocate megabytes of whitespace per keystroke on the generator hot path just for intermediate visual formatting.
If you need to read the generated file for debugging, you can use the format document feature in your IDE to instantly indent it. Furthermore, `#line` directives ensure stack traces and compiler errors map perfectly back to your original source files.

### How do you handle complex features such as Coerce, Validate, and FrameworkPropertyMetadataOptions?

Through convention-based `partial` methods. If you define `partial void CoerceIsActive(ref bool value)` or `partial bool ValidateIsActive(bool value)`, the generator detects them and automatically wires up the metadata callbacks. Framework-specific flags like `AffectsRender = true` or `BindsTwoWayByDefault` are simply declared via attribute arguments.

### Why is there still 2.22 MB allocated if the pipeline is zero-allocation?

The internal synthesis pipeline—AST token extraction, string assembly, and `SourceWriter` buffering—allocates exactly 0 Bytes. The remaining 2.22 MB represents the unavoidable overhead of Roslyn's API boundaries: Incremental Pipeline caching overhead, `GeneratorInitializationContext`, and the mandatory final `SourceText` string allocation when handing the generated code back to the compiler host. Our proprietary generation logic itself remains strictly zero-allocation.
For end-to-end benchmark measurements and reduction ratios, see **[Section VI. Performance Metrics in 05. Code Synthesis and Performance](./05_synthesis_and_performance.md#vi-performance-metrics)**.

### Does this break XAML Hot Reload, Live Preview, or NativeAOT?

No. The generator outputs standard, static C# code with explicit `DependencyProperty.Register` calls. There is absolutely zero runtime reflection or `Reflection.Emit` involved, making the output 100% NativeAOT and Trimming safe. Because it generates static fields and property wrappers at compile-time, it plays perfectly with XAML Hot Reload and Language Server Protocol, functioning exactly like hand-written code.
