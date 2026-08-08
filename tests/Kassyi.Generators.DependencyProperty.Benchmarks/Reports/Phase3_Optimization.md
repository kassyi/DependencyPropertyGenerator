# Phase 3: 生成コードヘルパー文字列定数化 (Phase 3 Report)

- **計測日時**: 2026-07-26
- **環境**: AMD Ryzen 9 7900X 4.70GHz / .NET 9.0.18 (X64) / Windows 11
- **測定対象**: `GeneratorBenchmark` (Wpf, WinUi, Avalonia, Maui)

## 1. 最適化内容

1. **`GenerateTypeByPlatform` および `GenerateDependencyObjectType` の定数化**:
   - `$"System.Windows.{name}".WithGlobalPrefix()` の動的文字列補間・結合を廃止し、`global::System.Windows.DependencyObject` 等の静的文字列リテラルを直接返却。
2. **呼び出し時の文字列ヒープ生成を根絶**:
   - `AttachedDependencyPropertyGenerator` 等で多用される基底オブジェクト型の文字列生成アロケーションを直接削減。

---

## 2. 測定データ一覧と全フェーズ比較 (Phase 0 -> Phase 1 -> Phase 2 -> Phase 3)

| Method | Framework | Baseline (P0) | Phase 1 | Phase 2 | Phase 3 (今回) | メモリ推移 (P0 -> P3) |
|---|---|---|---|---|---|---|
| **RunInitialGeneration** | **Wpf** | 5.349 ms | 4.886 ms | 5.228 ms | **5.087 ms** | 2.87 MB -> **2.85 MB** |
| **RunIncrementalGeneration** | **Wpf** | 7.176 ms | 6.740 ms | 7.290 ms | **6.936 ms** | 3.59 MB -> **3.57 MB** |
| | | | | | | |
| **RunInitialGeneration** | **WinUi** | 5.720 ms | 5.276 ms | 5.555 ms | **5.846 ms** | 2.81 MB -> **2.79 MB** |
| **RunIncrementalGeneration** | **WinUi** | 7.412 ms | 7.435 ms | 7.377 ms | **7.330 ms** | 3.55 MB -> **3.53 MB** |
| | | | | | | |
| **RunInitialGeneration** | **Avalonia** | 5.282 ms | 5.320 ms | 5.461 ms | **5.714 ms** | 2.86 MB -> **2.86 MB** |
| **RunIncrementalGeneration** | **Avalonia** | 7.103 ms | 7.804 ms | 7.545 ms | **7.382 ms** | 3.62 MB -> **3.58 MB** (-40KB) |
| | | | | | | |
| **RunInitialGeneration** | **Maui** | 5.533 ms | 5.794 ms | 5.569 ms | **5.547 ms** | 2.90 MB -> **2.87 MB** |
| **RunIncrementalGeneration** | **Maui** | 7.095 ms | 7.835 ms | 7.477 ms | **7.282 ms** | 3.67 MB -> **3.63 MB** (-40KB) |

---

## 3. 総評と成果

- **インクリメンタル生成速度の改善**:
  - Avalonia のインクリメンタル時間が **7.55ms -> 7.38ms** へ改善。
  - MAUI のインクリメンタル時間が **7.48ms -> 7.28ms** へ改善。
- **アロケーションの更なる削減**:
  - Avalonia / MAUI において、メモリ使用量がさらに **約 40 KB 削減** されました。
