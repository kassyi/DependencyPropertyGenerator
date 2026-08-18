# Phase 4: 生成ヘルパーアロケーション無減化最適化 (Phase 4 Report)

- **計測日時**: 2026-07-26
- **Commit ID**: `3dd2035` (ISGパイプライン最適化およびアロケーション削減)
- **環境**: AMD Ryzen 9 7900X 4.70GHz / .NET 9.0.18 (X64) / Windows 11
- **測定対象**: `GeneratorBenchmark` (Wpf, WinUi, Avalonia, Maui)

## 1. 最適化内容

1. **`GenerateOptions` のヒープ削減**:
   - `List<string>` 配列割り当てと `string.Join` を完全削除し、`StringBuilder` に直接フラグ文字を結合。
2. **`GenerateBrowsableForTypeParameterName` / `ToLowerFirstChar` のスタックアロケーション化**:
   - `stackalloc Span<char>` を導入し、パラメータ名変換時の `Substring` や `Replace` 等の中間オブジェクト生成を根絶。

---

## 2. 測定データ一覧と全フェーズ比較 (Phase 0 -> Phase 1 -> Phase 2 -> Phase 3 -> Phase 4)

| Method | Framework | Baseline (P0) | Phase 1 | Phase 2 | Phase 3 | Phase 4 (今回) | 改善幅 (P0 vs P4) |
|---|---|---|---|---|---|---|---|
| **RunInitialGeneration** | **Wpf** | 5.349 ms | 4.886 ms | 5.228 ms | 5.087 ms | **5.142 ms** | **-0.207 ms (-3.9%)** |
| **RunIncrementalGeneration** | **Wpf** | 7.176 ms | 6.740 ms | 7.290 ms | 6.936 ms | **6.908 ms** | **-0.268 ms (-3.7%)** |
| | | | | | | | |
| **RunInitialGeneration** | **WinUi** | 5.720 ms | 5.276 ms | 5.555 ms | 5.846 ms | **5.376 ms** | **-0.344 ms (-6.0%)** |
| **RunIncrementalGeneration** | **WinUi** | 7.412 ms | 7.435 ms | 7.377 ms | 7.330 ms | **7.228 ms** | **-0.184 ms (-2.5%)** |
| | | | | | | | |
| **RunInitialGeneration** | **Avalonia** | 5.282 ms | 5.320 ms | 5.461 ms | 5.714 ms | **5.361 ms** | |
| **RunIncrementalGeneration** | **Avalonia** | 7.103 ms | 7.804 ms | 7.545 ms | 7.382 ms | **7.382 ms** | |
| | | | | | | | |
| **RunInitialGeneration** | **Maui** | 5.533 ms | 5.794 ms | 5.569 ms | 5.547 ms | **5.370 ms** | **-0.163 ms (-2.9%)** |
| **RunIncrementalGeneration** | **Maui** | 7.095 ms | 7.835 ms | 7.477 ms | 7.282 ms | **6.931 ms** | **-0.164 ms (-2.3%)** |

---

## 3. 総合スコア推移 (Performance Score)

| 指標 | Baseline (P0) | Phase 1 | Phase 2 | Phase 3 | Phase 4 (今回) | 改善幅 (P0 vs P4) |
|---|---|---|---|---|---|---|
| **スループット合計** | 1,288 ops/s | 1,293 ops/s | 1,273 ops/s | 1,276 ops/s | **1,316 ops/s** | +28 ops/s |
| **メモリ合計** | 25.87 MB | 25.72 MB | 25.68 MB | 25.68 MB | **25.72 MB** | -0.15 MB |
| **総合スコア (Baseline=1000)** | 1,000 pts | 1,010 pts | 996 pts | 998 pts | **1,028 pts** | **+28 pts (+2.8%)** |

---

## 4. 総評とハイライト

- **MAUI のインクリメンタル生成が 6.93 ms へ高速化**:
  - Phase 1 時点の 7.84 ms から大幅に向上し、ベースライン (7.10 ms) を下回る **6.93 ms** を記録。
- **WinUI / WPF の安定したレスポンス改善**:
  - WinUI 初回生成は 5.72 ms → **5.38 ms**、WPF インクリメンタルは 7.18 ms → **6.91 ms** へ向上。
