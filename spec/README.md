# DependencyPropertyGenerator Specifications

Welcome to the architectural specifications and domain documentation for **DependencyPropertyGenerator (`Kassyi.Generators.DependencyProperty`)**.

Please select your preferred language:

- 🇺🇸 **[English Documentation (`/en/intro.md`)](./en/intro.md)**
  Comprehensive guides covering technical architecture, incremental pipeline design, the `SourceWriter` code generation engine, and zero-allocation optimization strategies.
- 🇯🇵 **[日本語ドキュメント (`/ja/intro.md`)](./ja/intro.md)**
  全体アーキテクチャ、インクリメンタル・パイプライン構造、`SourceWriter` によるコード生成仕様、およびゼロアロケーション最適化戦略を解説した日本語版ドキュメントです。

---

## Document Index / ドキュメント構成

| Chapter | English | 日本語 | Description |
|---|---|---|---|
| **Intro** | [Introduction](./en/intro.md) | [概要](./ja/intro.md) | Overview and high-level structure |
| **01** | [Foundation & Domain](./en/01_foundation_and_domain.md) | [基盤とドメイン](./ja/01_foundation_and_domain.md) | Modular structure, ubiquitous language & DTO models |
| **02** | [Pipeline & Architecture](./en/02_pipeline_architecture.md) | [パイプライン構造](./ja/02_pipeline_architecture.md) | Incremental generator pipeline & caching strategy |
| **03** | [Code Synthesis & Performance](./en/03_synthesis_and_performance.md) | [コード生成とパフォーマンス最適化](./ja/03_synthesis_and_performance.md) | `SourceWriter` (`ClassScope`), zero-allocation synthesis & profiling |
| **04** | [Complexity Model](./en/04_mathematical_model.md) | [計算量モデル](./ja/04_mathematical_model.md) | Worst-case complexity analysis & pipeline scaling limits |
| **05** | [Test Specification](./en/05_test_specification.md) | [テスト仕様書](./ja/05_test_specification.md) | Test architecture, combinatorial matrix, language features & diagnostics |
| **06** | [Framework Strategies](./en/06_framework_strategies.md) | [フレームワーク別生成マッピング仕様](./ja/06_framework_strategies.md) | Platform API mapping & generator extension guidelines |
