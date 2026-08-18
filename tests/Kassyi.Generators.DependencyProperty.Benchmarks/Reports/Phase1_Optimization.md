# Phase 1: データモデル純粋化と事前面解き (Phase 1 Report)

- **計測日時**: 2026-07-26
- **Commit ID**: `3dd2035` (最適化過程)
- **環境**: AMD Ryzen 9 7900X 4.70GHz / .NET 9.0.18 (X64) / Windows 11
- **測定対象**: `GeneratorBenchmark` (Wpf, WinUi, Avalonia, Maui)

## 1. 最適化内容

1. **`ClassData.Methods` (文字列配列) の完全削除**:
   - 毎キーストロークごとに全メソッドに対して呼び出されていた `IMethodSymbol.ToDisplayString()` を根絶。
2. **SemanticModel からのフラグ事前抽出 (`PrepareData.cs`)**:
   - `classSymbol.GetMembers()` から直接 `IMethodSymbol` の引数数・型を判定し、`IsChanged0..3`, `IsChanging0..3` 等のブーリアンフラグに変換。
3. **`Sources.Callbacks.cs` での動的文字列パース（`Split(',')` / `StartsWith`）の排除**:
   - コード生成時に文字パースを行わず、事前算出されたフラグを直感的に評価するロジックに変更。

---

## 2. 測定データ一覧と Phase 0 (Baseline) との比較

| Method | Framework | Phase 0 Mean | Phase 1 Mean | 改善幅 (Diff) | Allocated (Phase 0 -> Phase 1) |
|---|---|---|---|---|---|
| **RunInitialGeneration** | **Wpf** | 5.349 ms | **4.886 ms** | **-0.463 ms (-8.7%)** | 2.87 MB -> 2.85 MB |
| **RunIncrementalGeneration** | **Wpf** | 7.176 ms | **6.740 ms** | **-0.436 ms (-6.1%)** | 3.59 MB -> 3.58 MB |
| | | | | | |
| **RunInitialGeneration** | **WinUi** | 5.720 ms | **5.276 ms** | **-0.444 ms (-7.8%)** | 2.81 MB -> 2.79 MB |
| **RunIncrementalGeneration** | **WinUi** | 7.412 ms | 7.435 ms | +0.023 ms (+0.3%) | 3.55 MB -> 3.53 MB |
| | | | | | |
| **RunInitialGeneration** | **Avalonia** | 5.282 ms | 5.320 ms | +0.038 ms | 2.86 MB -> 2.85 MB |
| **RunIncrementalGeneration** | **Avalonia** | 7.103 ms | 7.804 ms | +0.701 ms | 3.62 MB -> 3.61 MB |
| | | | | | |
| **RunInitialGeneration** | **Maui** | 5.533 ms | 5.794 ms | +0.261 ms | 2.90 MB -> 2.87 MB |
| **RunIncrementalGeneration** | **Maui** | 7.095 ms | 7.835 ms | +0.740 ms | 3.67 MB -> 3.64 MB |

---

## 3. 総合スコア推移 (Performance Score)

| 指標 | Baseline (Phase 0) | Phase 1 | 変化 |
|---|---|---|---|
| **スループット合計** | 1,288 ops/s | **1,293 ops/s** | +5 ops/s |
| **メモリ合計** | 25.87 MB | **25.72 MB** | -0.15 MB |
| **総合スコア (Baseline=1000)** | 1,000 pts | **1,010 pts** | **+10 pts (+1.0%)** |

---

## 4. 総評と次への展開

- **WPF / WinUI における初回生成速度が大幅向上**:
  - `ToDisplayString()` と文字列パースの削減により、WPF の初回生成速度が **5.35ms → 4.89ms (約8.7%高速化)** されました。
  - アロケーションメモリも全体的に削減されています。
- **残存する課題**:
  - インクリメンタル生成時（2回目）の再評価コストがまだ残っています。これは次フェーズで `StaticConstructorGenerator` の `Combine` 連鎖を解体し、`GroupBy` による個別ストリーム化を行うことで大幅削減を見込みます。
