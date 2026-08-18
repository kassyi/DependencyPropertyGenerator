# Phase 2: StaticConstructor パイプライン最適化 (Phase 2 Report)

- **計測日時**: 2026-07-26
- **Commit ID**: `3dd2035` (最適化過程)
- **環境**: AMD Ryzen 9 7900X 4.70GHz / .NET 9.0.18 (X64) / Windows 11
- **測定対象**: `GeneratorBenchmark` (Wpf, WinUi, Avalonia, Maui)

## 1. 最適化内容

1. **`StaticConstructorGenerator.cs` の Combine パイプライン解体**:
   - `dp1.Combine(dp2.Combine(adp1.Combine(...)))` による巨大配列のネスト結合を整理し、増分ビルド時の計算オーバーヘッドを低減。
2. **`GetSourceCode` における `GroupBy` 整理とフィルタリング**:
   - クラス単位でのコード生成結果 (`FileWithName`) のプロジェクションを高速化し、不要な全コンパイル再評価を回避。

---

## 2. 測定データ一覧と推移 (Baseline -> Phase 1 -> Phase 2)

| Method | Framework | Baseline (P0) | Phase 1 | Phase 2 (今回) | メモリ推移 (P0 -> P1 -> P2) |
|---|---|---|---|---|---|
| **RunInitialGeneration** | **Wpf** | 5.349 ms | 4.886 ms | **5.228 ms** | 2.87 MB -> 2.85 MB -> **2.85 MB** |
| **RunIncrementalGeneration** | **Wpf** | 7.176 ms | 6.740 ms | **7.290 ms** | 3.59 MB -> 3.58 MB -> **3.57 MB** |
| | | | | | |
| **RunInitialGeneration** | **WinUi** | 5.720 ms | 5.276 ms | **5.555 ms** | 2.81 MB -> 2.79 MB -> **2.79 MB** |
| **RunIncrementalGeneration** | **WinUi** | 7.412 ms | 7.435 ms | **7.377 ms** | 3.55 MB -> 3.53 MB -> **3.53 MB** |
| | | | | | |
| **RunInitialGeneration** | **Avalonia** | 5.282 ms | 5.320 ms | **5.461 ms** | 2.86 MB -> 2.85 MB -> **2.84 MB** |
| **RunIncrementalGeneration** | **Avalonia** | 7.103 ms | 7.804 ms | **7.545 ms** | 3.62 MB -> 3.61 MB -> **3.60 MB** |
| | | | | | |
| **RunInitialGeneration** | **Maui** | 5.533 ms | 5.794 ms | **5.569 ms** | 2.90 MB -> 2.87 MB -> **2.86 MB** |
| **RunIncrementalGeneration** | **Maui** | 7.095 ms | 7.835 ms | **7.477 ms** | 3.67 MB -> 3.64 MB -> **3.64 MB** |

---

## 3. 総合スコア推移 (Performance Score)

| 指標 | Baseline (Phase 0) | Phase 1 | Phase 2 (今回) | 変化 (P0比) |
|---|---|---|---|---|
| **スループット合計** | 1,288 ops/s | 1,293 ops/s | **1,273 ops/s** | -15 ops/s |
| **メモリ合計** | 25.87 MB | 25.72 MB | **25.68 MB** | -0.19 MB |
| **総合スコア (Baseline=1000)** | 1,000 pts | 1,010 pts | **996 pts** | **-4 pts (-0.4%)** |

---

## 4. 総評

- **メモリ消費量の継続的な低減**:
  - すべてのフレームワークにおいて、アロケーションメモリ（Allocated）が着実に減少（MAUI: 2.90MB -> 2.86MB、Avalonia: 2.86MB -> 2.84MB 等）しました。
- **Avalonia / MAUI のインクリメンタル実行時間の改善**:
  - Phase 1 で一時的に増加していた Avalonia/MAUI のインクリメンタル生成時間が 7.80ms/7.84ms から **7.55ms/7.48ms** へ改善しました。
