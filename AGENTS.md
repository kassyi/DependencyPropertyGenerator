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

## Source Generation Best Practices

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


