# Chapter 01: Design Rationale & FAQ

This chapter outlines the architectural philosophy behind the zero-allocation v4 generator and answers frequently asked questions regarding API design, code synthesis, and integration with modern .NET features.

## 1. Design Philosophy

Writing `DependencyProperty` in XAML frameworks (.NET MAUI, WPF, WinUI, Avalonia) is notoriously verbose. The primary goal of `DependencyPropertyGenerator` is to eliminate this boilerplate without degrading IDE responsiveness at scale.

To achieve high-throughput, zero-allocation generation, the v4 pipeline relies on the following aggressive architectural shifts:

### Flat Code Synthesis (No Indentation)
The generator drops `NormalizeWhitespace()` and manual indentation entirely. It outputs flat, left-aligned code.
- **Performance win:** Eliminates thousands of whitespace string allocations on the hot path.
- **AI/LLM win:** Lightweight models (like Gemini Flash) often hallucinate or misalign whitespace when predicting code. Flat output ensures deterministic, stable generation without brittle whitespace matching.

### Direct Token Streaming (Zero AST Mutation)
We strictly avoid `SyntaxFactory` mutations for code synthesis. Whether generating class definitions or resolving dynamic `DefaultValueExpression` declarations (e.g., converting `new()` to explicit types), we extract tokens from the parsed AST and stream them straight into a custom `SourceWriter`.
- **Zero Gen2 Allocations:** Bypassing AST construction completely prevents Gen2 GC spikes.

### `ref struct` Source Writers
Generation logic runs on stack-allocated `SourceWriter` and `ClassScope` wrappers, completely bypassing heap allocations for string assembly.

## 2. Frequently Asked Questions (FAQ)

### Why class-level attributes instead of field-level (like MVVM Toolkit) or C# 13 partial properties?
Class-level attributes provide a unified mental model for declaring both standard Dependency Properties and Attached Properties (which do not map cleanly to backing fields). However, C# 13 `partial property` support is natively implemented and works seamlessly alongside class-level attributes, utilizing the same underlying AST parsing logic.

### Does flat output (killing indentation) ruin the "Go To Definition" or debugging experience?
No. Formatting generated code is the IDE/formatter's responsibility. It is inefficient to allocate megabytes of whitespace per keystroke on the generator hot path just for intermediate visual formatting. 
If you need to read the generated file for debugging, you can use **Format Document (Ctrl+K, Ctrl+D)** in your IDE to instantly indent it. Furthermore, `#line` directives ensure stack traces and compiler errors map perfectly back to your original source files.

### How do you handle complex DP features (Coerce, Validate, FrameworkPropertyMetadataOptions)?
Through convention-based `partial` methods. If you define `partial void CoerceIsActive(ref bool value)` or `partial bool ValidateIsActive(bool value)`, the generator detects them and automatically wires up the metadata callbacks. Framework-specific flags like `AffectsRender = true` or `BindsTwoWayByDefault` are simply declared via attribute arguments.

### Why is there still 2.22 MB allocated if the pipeline is "zero-allocation"?
The internal synthesis pipeline—AST token extraction, string assembly, and `SourceWriter` buffering—allocates exactly 0 Bytes. The remaining 2.22 MB represents the unavoidable overhead of Roslyn's API boundaries: Incremental Pipeline caching overhead, `GeneratorInitializationContext`, and the mandatory final `SourceText` string allocation when handing the generated code back to the compiler host. Our proprietary generation logic itself remains strictly zero-allocation.

### Does this break XAML Hot Reload, Live Preview, or NativeAOT?
No. The generator outputs standard, static C# code (explicit `DependencyProperty.Register` calls). There is absolutely zero runtime reflection or `Reflection.Emit` involved, making the output 100% NativeAOT and Trimming safe. Because it generates static fields and property wrappers at compile-time, it plays perfectly with XAML Hot Reload and LSP, functioning exactly like hand-written code.

