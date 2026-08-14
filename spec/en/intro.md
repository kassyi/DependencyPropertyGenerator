# DependencyPropertyGenerator Specification (Introduction)

[English](./intro.md) | [日本語](../ja/intro.md)

This documentation contains the official specification, domain models, incremental pipeline architecture, code generation design, and performance optimization guidelines for **DependencyPropertyGenerator (`Kassyi.Generators.DependencyProperty`)**, a Roslyn Incremental Source Generator.

Use this documentation as the architectural source of truth when extending features, optimizing compilation performance, or refining Roslyn pipeline caching behavior.

## Specification Index

- [01. Foundation & Domain](./01_foundation_and_domain.md)
  - Purpose, modular design, target platforms, ubiquitous language, and structured DTO models.
- [02. Pipeline & Architecture](./02_pipeline_architecture.md)
  - Incremental generator pipeline, dataflow, extraction caching, and model equality strategies.
- [03. Generation & Optimization](./03_generation_and_optimization.md)
  - Zero-allocation code generation with `SourceWriter` (`ClassScope`), callback matching rules, Dos & Don'ts, and profiling.
- [04. Complexity Model of Incremental Generator](./04_mathematical_model.md)
  - Worst-case time/memory complexity analysis, caching scaling limits, and architectural mitigation.

