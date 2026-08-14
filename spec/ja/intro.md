# DependencyPropertyGenerator 仕様書概要 (Introduction)

[English](../en/intro.md) | [日本語](./intro.md)

このドキュメント群は、C# Incremental Source Generator である **DependencyPropertyGenerator (`Kassyi.Generators.DependencyProperty`)** の全体アーキテクチャ、ドメイン仕様、インクリメンタル・パイプライン構造、コード生成エンジン、およびパフォーマンス最適化戦略をまとめた公式仕様書です。

ソースジェネレーターの実行パフォーマンス（ビルド時間、メモリ消費、IDE応答速度）改善や新機能追加の際、システム全体を掌握し、Roslyn API との正しいやり取りやキャッシュ戦略を維持するために活用してください。

## インデックス

- [01. 基盤とドメインデータ (Foundation & Domain)](./01_foundation_and_domain.md)
  - プロジェクトの目的、モジュール構成、対象プラットフォーム、ユビキタス言語、構造化データモデル（DTO）
- [02. パイプラインとアーキテクチャ (Pipeline & Architecture)](./02_pipeline_architecture.md)
  - Incremental Generatorのアーキテクチャ、データフロー、抽出最適化、等価性(キャッシュ)戦略
- [03. 生成戦略と最適化 (Generation & Optimization)](./03_generation_and_optimization.md)
  - `SourceWriter` によるゼロアロケーション出力 (`ClassScope`)、コールバック解決ルール、Dos & Don'ts、プロファイリング手法
- [04. インクリメンタル・ジェネレーターの計算量モデル (Complexity Model)](./04_mathematical_model.md)
  - 最悪時間・メモリ計算量の数理分析、パイプラインのキャッシュスケーリング限界、および設計的対策

