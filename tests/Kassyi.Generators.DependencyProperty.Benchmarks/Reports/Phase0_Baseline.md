# Phase 0: 改善前ベースライン (Baseline Performance Report)

- **計測日時**: 2026-07-26
- **Commit ID**: `8ee617a` (ベンチマーク導入時・最適化前)
- **環境**: AMD Ryzen 9 7900X 4.70GHz / .NET 9.0.18 (X64) / Windows 11
- **測定対象**: `GeneratorBenchmark` (Wpf, WinUi, Avalonia, Maui)

## 1. 測定データ一覧

| Method | Framework | Mean (ms) | Error (ms) | StdDev (ms) | Gen0 (/1000op) | Gen1 (/1000op) | Gen2 (/1000op) | Allocated (MB) |
|---|---|---|---|---|---|---|---|---|
| **RunInitialGeneration** | Wpf | 5.349 | 0.1070 | 0.1983 | 156.2500 | 62.5000 | - | 2.87 |
| **RunIncrementalGeneration** | Wpf | 7.176 | 0.1030 | 0.0963 | 234.3750 | 85.9375 | 15.6250 | 3.59 |
| | | | | | | | | |
| **RunInitialGeneration** | WinUi | 5.720 | 0.1115 | 0.2837 | 179.6875 | 62.5000 | 7.8125 | 2.81 |
| **RunIncrementalGeneration** | WinUi | 7.412 | 0.1478 | 0.3681 | 226.5625 | 78.1250 | 7.8125 | 3.55 |
| | | | | | | | | |
| **RunInitialGeneration** | Avalonia | 5.282 | 0.1043 | 0.1241 | 179.6875 | 62.5000 | 7.8125 | 2.86 |
| **RunIncrementalGeneration** | Avalonia | 7.103 | 0.1169 | 0.1093 | 234.3750 | 78.1250 | 7.8125 | 3.62 |
| | | | | | | | | |
| **RunInitialGeneration** | Maui | 5.533 | 0.1103 | 0.1582 | 187.5000 | 70.3125 | 7.8125 | 2.90 |
| **RunIncrementalGeneration** | Maui | 7.095 | 0.1397 | 0.1609 | 234.3750 | 85.9375 | 7.8125 | 3.67 |

---

## 2. 総合スコア (Performance Score)
 
- **スループット合計**: **1,288 ops/sec**
- **割り当てメモリ合計**: **25.87 MB**
- **総合スコア**: **1,000 pts** (Baseline 基準値)

---

## 3. 考察とボトルネック指標

- **初回生成コスト (Initial)**: ~5.3 ms 〜 5.7 ms, ~2.8 MB
- **インクリメンタル生成単体コスト (Delta)**: `RunIncrementalGeneration` - `RunInitialGeneration` = **~1.8 ms**, 追加メモリ **~0.7 MB (700 KB)**
- **今後の最適化目標**:
  - `IEquatable` 比較による早期リターンで差分評価時間を短縮（目標: インクリメンタルコスト 1ms 未満）
  - 無駄な文字列結合や `Select` 内アロケーションの削減（目標: アロケーション数・容量の削減）

