# DependencyPropertyGenerator Specifications

Welcome to the architectural specifications and domain documentation for **DependencyPropertyGenerator (`Kassyi.Generators.DependencyProperty`)**.

Please select your preferred language:

- 🇺🇸 **[English Documentation (`/en/intro.md`)](./en/intro.md)**
  - Full technical architecture, incremental pipeline design, `SourceWriter` code generation engine, and zero-allocation optimization guides in English.
- 🇯🇵 **[日本語ドキュメント (`/ja/intro.md`)](./ja/intro.md)**
  - 全体アーキテクチャ、インクリメンタル・パイプライン構造、`SourceWriter` によるコード生成仕様、およびゼロアロケーション最適化戦略（日本語版）。

---

## Document Index / ドキュメント構成

| Chapter | English | 日本語 | Description |
|---|---|---|---|
| **Intro** | [Introduction](./en/intro.md) | [概要](./ja/intro.md) | Overview and high-level structure |
| **01** | [Foundation & Domain](./en/01_foundation_and_domain.md) | [基盤とドメイン](./ja/01_foundation_and_domain.md) | Modular structure, ubiquitous language & DTO models |
| **02** | [Pipeline & Architecture](./en/02_pipeline_architecture.md) | [パイプライン構造](./ja/02_pipeline_architecture.md) | Incremental generator pipeline & caching strategy |
| **03** | [Generation & Optimization](./en/03_generation_and_optimization.md) | [生成戦略と最適化](./ja/03_generation_and_optimization.md) | `SourceWriter` (`ClassScope`), callbacks & best practices |
| **04** | [Mathematical Model](./en/04_mathematical_model.md) | [数理モデル](./ja/04_mathematical_model.md) | Set-theoretic & pure functional compiler model |
