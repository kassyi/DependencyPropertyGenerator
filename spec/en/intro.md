# DependencyPropertyGenerator Specification

[English](./intro.md) | [日本語](../ja/intro.md)

This documentation provides the official specification, domain models, incremental pipeline architecture, code generation design, and performance optimization guidelines for **DependencyPropertyGenerator (`Kassyi.Generators.DependencyProperty`)**, a Roslyn Incremental Source Generator.

Use this documentation as an architectural reference when extending features, optimizing compilation performance (build time, memory footprint, IDE responsiveness), or refining Roslyn pipeline caching strategies.

## Specification Index

The following documents explain the internal workings of the system.

- **[01. Foundation and Domain](./01_foundation_and_domain.md)**
  Defines the project's purpose, modular design, and target platforms. It also covers the ubiquitous language and structured DTO models used throughout the codebase.
- **[02. Pipeline and Architecture](./02_pipeline_architecture.md)**
  Explains the Incremental Generator pipeline and dataflow. It details extraction optimization techniques and model equality strategies necessary for efficient caching.
- **[03. Code Synthesis and Performance Optimization](./03_synthesis_and_performance.md)**
  Details the zero-allocation code synthesis engine (`SourceWriter` / `ClassScope`), extreme performance optimization guidelines (AST direct traversal, LINQ elimination), and profiling methodologies.
- **[04. Complexity Model of Incremental Generator](./04_mathematical_model.md)**
  Analyzes worst-case time and memory complexities. It explains pipeline cache scaling limits and the architectural mitigations required to overcome them.
- **[05. Test Specification](./05_test_specification.md)**
  Defines the testing strategy, quality targets, combinatorial matrix parameters, test cases, and pass criteria for the project.
- **[06. Framework Strategies](./06_framework_strategies.md)**
  Defines the platform API mapping differences among UI frameworks and provides troubleshooting guidelines (Agentic Ground Truth) for autonomous agents.
