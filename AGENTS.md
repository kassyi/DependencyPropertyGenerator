# DependencyPropertyGenerator Agent Context

This file gives Codex and other agents enough repository context to work safely and to support later portfolio analysis.

<!-- BEGIN GENERATED PORTFOLIO CONTEXT -->

## Generated Repository Context

This section was generated from the GitHub repository inventory and local checkout to support future Codex work and portfolio analysis.

### Repository Metadata

- Remote: https://github.com/kassyi/DependencyPropertyGenerator
- Visibility: public
- Type: original; active
- Primary language: C#
- Topics: csharp, dependency-property, dotnet, dp, generator, net5, net6, source-generator, incrementral-generator, avalonia, avaloniaui, routed-event, uno, uno-platform, uwp, winui, wpf, csharp-sourcegenerator, maui
- Last pushed: 2026-06-01T06:25:40Z
- Local path: /Users/kassyi/GitHub/kassyi/DependencyPropertyGenerator
- Local note: standard checkout
- Classification: Public developer tooling/library

### Working Summary

Dependency property, routed event and weak event source generator for WPF/UWP/WinUI/Uno/Avalonia/MAUI platforms.

### Detected Structure

- Top-level items: `.github/`, `assets/`, `src/`, `.gitignore`, `DependencyPropertyGenerator.sln`, `DependencyPropertyGenerator.sln.DotSettings`, `global.json`, `LICENSE`, `README.md`
- Sampled file count: 1372
- Common extensions: .cs (1134), .txt (222), .props (4), .csproj (4), [no extension] (2), .sln (1), .dotsettings (1), .md (1)

### Manifests And Commands

- Kassyi.Generators.DependencyProperty.sln
- .github/workflows

Suggested commands:

- dotnet build Kassyi.Generators.DependencyProperty.sln
- dotnet test

Testing signal:

- 2 .NET test project(s) detected

### Portfolio Signals

- Skills: .NET, Roslyn source generators, XAML UI frameworks, NuGet packaging, GitHub Actions, csharp, dependency-property, dotnet, dp, generator, net5, net6, source-generator, incrementral-generator, avalonia, avaloniaui, routed-event, uno
- Portfolio angle: Strong signal for advanced .NET metaprogramming, build-time automation, and API ergonomics.

### Agent Notes

- Prefer README and manifest instructions over generated assumptions when they disagree.
- Keep generated context current when build tooling, test commands, or project scope changes.
- Review private or client-specific details before copying portfolio claims into public material.

<!-- END GENERATED PORTFOLIO CONTEXT -->

## 1. CRITICAL GUARDRAILS & PROHIBITED ACTIONS

These rules are absolute. Any violation will directly induce runtime failures, Git history pollution, or development workflow disruption.

### 1.1. Absolute Ban on Unauthorized Git Commits / Pushes

- **DO NOT execute `git commit` or `git push` automatically.** It is strictly forbidden to commit or push code modifications on your own, even if you are certain the task is completed.
- **You do not have the final state authority.** A single code modification rarely resolves bugs completely without human incremental testing.
- **The user controls the workflow.** Your role is strictly to suggest modifications, stage them (`git add`) if requested, and prepare the commit message. You must always wait for the explicit `/commit` command or approval from the user before finalizing a state.

### 1.2. Absolute Ban on Reflection

- **NEVER use Reflection under any circumstances.** Usage of `System.Reflection`, dynamic type inspection, accessing private/protected members via reflection, dynamic invocation, or runtime metadata reading is strictly forbidden.
- **Alternative Approaches:** All dynamic behavior, type inspection, and code synthesis must be performed at compile-time via Roslyn Source Generators (`IIncrementalGenerator`) and syntax/symbol analysis.

### 1.3. No Empty Catch Blocks (Exception Swallowing)

- **NEVER suppress or swallow exceptions** with an empty `catch (Exception) { }` block.
- **Diagnostics:** Report diagnostic errors via Roslyn's `SourceProductionContext.ReportDiagnostic` with proper diagnostic descriptors (`DPG0000` - `DPG9999`) or rethrow appropriately.

---

## 2. C# DEVELOPMENT & CODE STYLE CHARTER

You must conform to these modern .NET guidelines, matching the project's `.editorconfig` exactly to prevent ReSharper/Roslyn diagnostic noise (such as `IDE1006`).

### 2.1. Strict Naming Conventions

- **Types:** Classes, Structs, Interfaces, Enums, Records, and Namespaces must use `PascalCase`.
  - Interfaces must start with an `I` prefix (e.g., `IMemberData`).
- **Members:** Properties, Methods, and Events must use `PascalCase`.
- **Variables:** Local variables and method parameters must use `camelCase`.
- **Constants & Static Readonly Fields:** Must use `PascalCase` even for local constants (e.g., `DefaultNamespace`, `MaxCachedItems`). Old-school uppercase with underscores (`ALL_UPPER`) is strictly forbidden.
- **Fields Prefixing:**
  - **Private Instance Fields:** Use `_camelCase` (e.g., `_writer`, `_builder`).
  - **Non-Private Instance Fields (including protected):** Use `PascalCase`. Prefer exposing them via properties.
  - **Non-Public Static Fields:** Use `s_camelCase` (e.g., `s_emptyArray`).
  - **Thread Static Fields:** Use `t_camelCase`.
- **Acronyms & Abbreviations:** For acronyms of 3 or more characters, use PascalCase/camelCase conventions rather than full capitalization (e.g., use `WpfGenerator` instead of `WPFGenerator`, `DpData` instead of `DPData`).

### 2.2. Modern C# Syntax & Zero-Allocation Preferences

- **Primary Constructors:** Use primary constructors where applicable to eliminate boilerplate.
- **Method Groups:** Convert lambdas into concise method groups where applicable (e.g., `x => MyMethod(x)` should be simplified to `MyMethod`).
- **UTF-8 String Literals:** In performance-critical sections and tests, prefer UTF-8 string literals (`"..."u8`) to avoid runtime encoding allocations.
- **ValueTask & Ref Structs:** Use `readonly ref struct` for zero-allocation scope managers (like `SourceWriterClassScope`).

---

## 3. Source Generation Best Practices

This is the definitive best practice designed specifically for this project's characteristics (Roslyn Source Generator + ultra-low allocation requirements + AI-driven maintainability).

### Core Principle: Outer Envelope (`ClassScope`) Helper + Scope with Header (`writer.Scope(...)`)

Encapsulate the repetitive 3-step boilerplate (`#nullable enable` → `namespace` → `partial class`) common across all generator files into **a single line**.

---

### Concrete Rules for Code Generation (AI Directives)

#### Rule 1: Always Use `writer.ClassScope(@class)` for Outer Envelope
Do NOT manually write `#nullable enable`, `namespace`, or `partial class` boilerplate in individual generator files.

- ❌ **DO NOT** write:
```csharp
writer.AppendLine("#nullable enable");
writer.AppendLine($"namespace {@class.Namespace}");
using (writer.Scope())
{
    writer.AppendLine($"{SourceGenerationHelper.GenerateModifiers(@class)}partial class {@class.Name}");
    using (writer.Scope())
    {
        // Core generation logic...
    }
}
```

- ✨ **DO** write:
```csharp
// Outer envelope (namespace & class) in just 1 line!
using var _ = writer.ClassScope(@class);
```

#### Rule 2: Pass Block Headers Directly to `writer.Scope(...)`
Do NOT manually write a line before opening a scope. Pass method or block headers directly into `writer.Scope(...)`.

- ❌ **DO NOT** write:
```csharp
writer.AppendLine($"static {@class.Name}()");
using (writer.Scope())
{
    // ...
}
```

- ✨ **DO** write:
```csharp
using (writer.Scope($"static {@class.Name}()"))
{
    // ...
}
```

---

### Why is this the best practice for this project?

1. **Eliminates the "using storm" and repetitive boilerplate simultaneously**
    - Centralizes the boilerplate (`#nullable enable`, `namespace`, `partial class`) present in dozens of generator template files, removing redundant code across the project.
2. **Zero-allocation implementation for `ClassScope` helper (Ultra-fast)**
    - Can be implemented efficiently by adding a `ref struct` within `SourceWriter`.
3. **Prevents AI assistants (e.g., Flash model) from making structural errors**
    - AI models generating source template code no longer need to handle outer scoping rules—instructing them to put `using var _ = writer.ClassScope(@class);` on the first line is sufficient.

```csharp
// Outer envelope helper method example in SourceWriter.cs
public SourceWriterClassScope ClassScope(ClassData @class)
{
    AppendLine("#nullable enable");
    AppendLine();
    AppendLine($"namespace {@class.Namespace}");
    AppendLine("{");
    AppendLine($"{SourceGenerationHelper.GenerateModifiers(@class)}partial class {@class.Name}");
    AppendLine("{");
    return new SourceWriterClassScope(this);
}

// Scope helper that outputs two closing braces '}' on Dispose
public readonly ref struct SourceWriterClassScope : IDisposable
{
    private readonly SourceWriter _writer;
    public SourceWriterClassScope(SourceWriter writer) => _writer = writer;
    public void Dispose()
    {
        _writer.AppendLine("}");
        _writer.AppendLine("}");
    }
}
```

---

## 4. OUTPUT VALIDATION & RESPONSE STYLE RULE

Before presenting any code modification, refactored files, or responding to the user:

### 4.1. Static Code Analysis Validation

1. **Mental Check:** Validate your generated code against this guidelines document.
2. **Diagnostic Pre-emption:** Ensure your output does not trigger any warnings or errors from Roslyn Analyzers or ReSharper. Leave the workspace clean and compliant.

### 4.2. Tone & Communication Rules

- **Language:** Conduct all explanations, walkthroughs, Q&As, and thinking processes in Japanese.
- **Professional Demeanor:** Keep your tone quiet, highly professional, concise, and dry—like a seasoned senior backend engineer. Do not adopt an overly optimistic or apologetic persona. Focus entirely on logic, architecture, and safety.


