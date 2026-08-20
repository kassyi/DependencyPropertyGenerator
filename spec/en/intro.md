# DependencyPropertyGenerator specification

This documentation provides the official specification, domain models, incremental pipeline architecture, code generation design, and performance optimization guidelines for **DependencyPropertyGenerator (`Kassyi.Generators.DependencyProperty`)**, a Roslyn Incremental Source Generator.

Use this documentation as an architectural reference when you extend features, optimize compilation performance (such as build time, memory footprint, and IDE responsiveness), or refine Roslyn pipeline caching strategies.

## Specification index

The following documents explain the system's internal architecture:

- **[01. Architectural principles and FAQ](./01_faq_and_rationale.md)**
  Explains the zero-allocation design philosophy, flat code synthesis, direct token streaming, and frequently asked questions.
- **[02. Foundation and domain](./02_foundation_and_domain.md)**
  Defines the project's purpose, modular design, and target platforms. It also covers the ubiquitous language and structured DTO models.
- **[03. Pipeline and architecture](./03_pipeline_architecture.md)**
  Details the Incremental Generator pipeline topology, internal class interactions, and data flow execution phases.
- **[04. Framework-specific generation mapping specifications](./04_framework_strategies.md)**
  Details the property registration API mappings across UI frameworks (WPF, WinUI, Uno, Avalonia, MAUI) and framework auto-detection cascade.
- **[05. Code synthesis and performance optimization](./05_synthesis_and_performance.md)**
  Details the zero-allocation code synthesis engine (`SourceWriter` / `ClassScope`), performance optimization guidelines (AST traversal, LINQ elimination), and benchmark metrics.
- **[06. Complexity model of Incremental Generator](./06_mathematical_model.md)**
  Analyzes worst-case time and memory complexities. It explains pipeline cache scaling limits and architectural mitigations.
- **[07. Test specification](./07_test_specification.md)**
  Defines the testing strategy, quality targets, combinatorial matrix parameters (576 cases), test cases, and pass criteria.
- **[08. Diagnostics reference](./08_diagnostics_reference.md)**
  Provides a reference guide detailing the causes and solutions for diagnostic errors (`DPG0000` through `DPG9999`) emitted by the generator.
