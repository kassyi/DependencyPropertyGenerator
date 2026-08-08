# ソースジェネレーター パフォーマンス最適化レポート

このディレクトリには、フェーズごとに実施したベンチマークの測定結果と、最適化前後の比較（Diff）を保存します。

## パフォーマンス履歴

| フェーズ | 説明 | 主な変更内容 | WPF初回(ms) | WPF差分単体(ms) | メモリ(MB) |
|---|---|---|---|---|---|
| [Phase 0 (Baseline)](./Phase0_Baseline.md) | 最適化前ベースライン | 現状のコードベース測定 | 5.35 ms | 1.83 ms | 2.87 MB / 3.59 MB |
| [Phase 1](./Phase1_Optimization.md) | データモデル純粋化と事前面解き | `ClassData.Methods`削除、フラグ事前計算 | 4.89 ms (-8.7%) | 1.85 ms | 2.85 MB / 3.58 MB |
| [Phase 2](./Phase2_Optimization.md) | StaticConstructor パイプライン最適化 | `Combine`処理の最適化と`GroupBy`整理 | 5.23 ms | 2.06 ms | 2.85 MB / 3.57 MB |
| [Phase 3](./Phase3_Optimization.md) | 生成コードヘルパー文字列定数化 | `GenerateTypeByPlatform`等の直接リテラル化 | 5.09 ms | 1.85 ms | 2.85 MB / 3.57 MB |
| [Phase 4](./Phase4_Optimization.md) | 生成ヘルパーアロケーション極限削減 | `StringBuilder`型オプション・`stackalloc Span`化 | 5.14 ms | 1.77 ms | 2.85 MB / 3.58 MB |

## 測定の再現方法

```bash
dotnet run -c Release --project tests\Kassyi.Generators.DependencyProperty.Benchmarks
```
