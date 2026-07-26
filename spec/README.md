# DependencyPropertyGenerator ドキュメント

このディレクトリは、C# Incremental Source Generator である **DependencyPropertyGenerator** の全体アーキテクチャ、ドメイン仕様、インクリメンタル・パイプライン構造、およびパフォーマンス最適化戦略をまとめたものです。

ソースジェネレーターの実行パフォーマンス（ビルド時間、メモリ消費）改善や新機能追加の際、システム全体を掌握し、Roslyn API との正しいやり取りやキャッシュ戦略を維持するために活用してください。

## インデックス

- [01. 基盤とドメインデータ (Foundation & Domain)](./01_foundation_and_domain.md)
  - プロジェクトの目的、対象プラットフォーム、ユビキタス言語、抽出データモデル
- [02. パイプラインとアーキテクチャ (Pipeline & Architecture)](./02_pipeline_architecture.md)
  - Incremental Generatorのアーキテクチャ、データフロー、等価性(キャッシュ)戦略
- [03. 生成戦略と最適化 (Generation & Optimization)](./03_generation_and_optimization.md)
  - コード出力仕様、ISymbol扱いのDos&Don'ts、プロファイリング手法
- [04. インクリメンタル・ジェネレーターの数理モデル (Mathematical Model)](./04_mathematical_model.md)
  - パイプラインとキャッシュ機構の集合論・関数ベースの数式表現
