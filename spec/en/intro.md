# DependencyPropertyGenerator Specification

[English](./intro.md) | [日本語](../ja/intro.md)

This documentation provides the official specification, domain models, incremental pipeline architecture, code generation design, and performance optimization guidelines for **DependencyPropertyGenerator (`Kassyi.Generators.DependencyProperty`)**, a Roslyn Incremental Source Generator.

Use this documentation as an architectural reference when extending features, optimizing compilation performance (such as build time, memory footprint, and IDE responsiveness), or refining Roslyn pipeline caching strategies.

## Specification Index

The following documents explain the system's internal architecture:

- **[01. Foundation and Domain](./01_foundation_and_domain.md)**
  Defines the project's purpose, modular design, and target platforms. It also covers the ubiquitous language and structured DTO models used throughout the codebase.
- **[02. Pipeline and Architecture](./02_pipeline_architecture.md)**
  Explains the Incremental Generator pipeline and dataflow. It details the extraction optimization techniques and model equality strategies required for efficient caching.
- **[03. Code Synthesis and Performance Optimization](./03_synthesis_and_performance.md)**
  Details the zero-allocation code synthesis engine (`SourceWriter` / `ClassScope`), aggressive performance optimization guidelines (such as direct AST traversal and LINQ elimination), and profiling methodologies.
- **[04. Complexity Model of Incremental Generator](./04_mathematical_model.md)**
  Analyzes worst-case time and memory complexities. It explains pipeline cache scaling limits and the architectural mitigations required to overcome them.
- **[05. Test Specification](./05_test_specification.md)**
  Defines the testing strategy, quality targets, combinatorial matrix parameters, test cases, and pass criteria.
- **[06. Framework-Specific Generation Mapping Specifications](./06_framework_strategies.md)**
  Details the differences in property registration APIs across UI frameworks and establishes the ground truth for autonomous agent extensions and bug fixes.
- **[07. Diagnostics Reference](./07_diagnostics_reference.md)**
  Provides a reference guide detailing the causes and solutions for diagnostic errors (`DPG0000` - `DPG9999`) emitted by the generator.
