# Phase 5: v1.0 全体リファクタリング・アーキテクチャ刷新 (Phase 5 Report)

- **計測日時**: 2026-08-18
- **Commit ID**: `a685623` (v1.0 全体リファクタリング後)
- **環境**: AMD Ryzen 9 7900X 4.70GHz / .NET 9.0.19 (X64) / Windows 11
- **測定対象**: `GeneratorBenchmark` (Wpf, WinUi, Avalonia, Maui)

## 1. 主な変更・リファクタリング内容

1. **`ClassScope` / `SourceWriter` 構造最適化とインデントの完全廃止**:
   - `using var _ = writer.ClassScope(@class);` 導入によるスコープ管理の一元化とボイラープレート削減。
   - 生成コードの見た目（インデント等）を整形する処理や `NormalizeWhitespace()` の呼び出しを完全に放棄し、全行を左揃え（フラット）で直接出力する設計に変更。これにより文字列結合とフォーマット時のアロケーションを根絶。
2. **Roslyn パイプライン・シンボル処理の徹底最適化**:
   - SyntaxValueProvider 拡張・不要な構文解析ツリーの走査削減とアロケーション抑制。
3. **文字列・拡張メソッドの最適化**:
   - `StringExtensions` などの文字列操作のゼロアロケーション化 / Span 化。
4. **警告・診断機構の整理**:
   - `Cs0436Suppressor` / 警告処理の効率化。

---

## 2. 測定データ一覧と全フェーズ比較 (Phase 0 -> Phase 1 -> Phase 2 -> Phase 3 -> Phase 4 -> Phase 5)

| Method | Framework | Baseline (P0) | Phase 1 | Phase 2 | Phase 3 | Phase 4 | Phase 5 (今回) | 改善幅 (P0 vs P5) |
|---|---|---|---|---|---|---|---|---|
| **RunInitialGeneration** | **Wpf** | 5.349 ms | 4.886 ms | 5.228 ms | 5.087 ms | 5.142 ms | **3.729 ms** | **-1.620 ms (-30.3%)** |
| **RunIncrementalGeneration** | **Wpf** | 7.176 ms | 6.740 ms | 7.290 ms | 6.936 ms | 6.908 ms | **5.663 ms** | **-1.513 ms (-21.1%)** |
| | | | | | | | | |
| **RunInitialGeneration** | **WinUi** | 5.720 ms | 5.276 ms | 5.555 ms | 5.846 ms | 5.376 ms | **4.192 ms** | **-1.528 ms (-26.7%)** |
| **RunIncrementalGeneration** | **WinUi** | 7.412 ms | 7.435 ms | 7.377 ms | 7.330 ms | 7.228 ms | **5.847 ms** | **-1.565 ms (-21.1%)** |
| | | | | | | | | |
| **RunInitialGeneration** | **Avalonia** | 5.282 ms | 5.320 ms | 5.461 ms | 5.714 ms | 5.361 ms | **4.137 ms** | **-1.145 ms (-21.7%)** |
| **RunIncrementalGeneration** | **Avalonia** | 7.103 ms | 7.804 ms | 7.545 ms | 7.382 ms | 7.382 ms | **5.665 ms** | **-1.438 ms (-20.2%)** |
| | | | | | | | | |
| **RunInitialGeneration** | **Maui** | 5.533 ms | 5.794 ms | 5.569 ms | 5.547 ms | 5.370 ms | **4.147 ms** | **-1.386 ms (-25.0%)** |
| **RunIncrementalGeneration** | **Maui** | 7.095 ms | 7.835 ms | 7.477 ms | 7.282 ms | 6.931 ms | **5.843 ms** | **-1.252 ms (-17.6%)** |

---

## 3. アロケーションメモリ比較 (Allocated Memory)

| Method | Framework | Baseline (P0) | Phase 4 | Phase 5 (今回) | メモリ削減幅 (P0 vs P5) |
|---|---|---|---|---|---|
| **RunInitialGeneration** | **Wpf** | 2.87 MB | 2.85 MB | **2.22 MB** | **-0.65 MB (-22.6%)** |
| **RunIncrementalGeneration** | **Wpf** | 3.59 MB | 3.58 MB | **2.93 MB** | **-0.66 MB (-18.4%)** |
| | | | | | |
| **RunInitialGeneration** | **WinUi** | 2.81 MB | 2.85 MB | **2.21 MB** | **-0.60 MB (-21.4%)** |
| **RunIncrementalGeneration** | **WinUi** | 3.55 MB | 3.58 MB | **2.94 MB** | **-0.61 MB (-17.2%)** |
| | | | | | |
| **RunInitialGeneration** | **Avalonia** | 2.86 MB | 2.85 MB | **2.25 MB** | **-0.61 MB (-21.3%)** |
| **RunIncrementalGeneration** | **Avalonia** | 3.62 MB | 3.58 MB | **3.01 MB** | **-0.61 MB (-16.9%)** |
| | | | | | |
| **RunInitialGeneration** | **Maui** | 2.90 MB | 2.85 MB | **2.26 MB** | **-0.64 MB (-22.1%)** |
| **RunIncrementalGeneration** | **Maui** | 3.67 MB | 3.58 MB | **3.02 MB** | **-0.65 MB (-17.7%)** |

---

## 4. 総合スコア推移 (Performance Score)

| 指標 | Baseline (P0) | Phase 1 | Phase 2 | Phase 3 | Phase 4 | Phase 5 (今回) | 改善幅 (P0 vs P5) |
|---|---|---|---|---|---|---|---|
| **スループット合計** | 1,288 ops/s | 1,293 ops/s | 1,273 ops/s | 1,276 ops/s | 1,316 ops/s | **1,685 ops/s** | **+397 ops/s** |
| **メモリ合計** | 25.87 MB | 25.72 MB | 25.68 MB | 25.68 MB | 25.72 MB | **20.84 MB** | **-5.03 MB** |
| **総合スコア (Baseline=1000)** | 1,000 pts | 1,010 pts | 996 pts | 998 pts | 1,028 pts | **1,624 pts** | **+624 pts (+62.4%)** 🚀 |

---

## 5. 詳細測定値 (BenchmarkDotNet Output)

| Method | Framework | Mean | Error | StdDev | Gen0 (/1000op) | Gen1 (/1000op) | Gen2 (/1000op) | Allocated |
|---|---|---|---|---|---|---|---|---|
| **RunInitialGeneration** | Wpf | 3.729 ms | 0.0480 ms | 0.0426 ms | 125.0000 | 31.2500 | - | 2.22 MB |
| **RunIncrementalGeneration** | Wpf | 5.663 ms | 0.1094 ms | 0.0970 ms | 187.5000 | 62.5000 | 7.8125 | 2.93 MB |
| **RunInitialGeneration** | WinUi | 4.192 ms | 0.0827 ms | 0.1076 ms | 140.6250 | 54.6875 | 7.8125 | 2.21 MB |
| **RunIncrementalGeneration** | WinUi | 5.847 ms | 0.0868 ms | 0.0769 ms | 187.5000 | 62.5000 | 7.8125 | 2.94 MB |
| **RunInitialGeneration** | Avalonia | 4.137 ms | 0.0515 ms | 0.0457 ms | 148.4375 | 54.6875 | 7.8125 | 2.25 MB |
| **RunIncrementalGeneration** | Avalonia | 5.665 ms | 0.0376 ms | 0.0314 ms | 195.3125 | 62.5000 | 7.8125 | 3.01 MB |
| **RunInitialGeneration** | Maui | 4.147 ms | 0.0819 ms | 0.1275 ms | 140.6250 | 46.8750 | - | 2.26 MB |
| **RunIncrementalGeneration** | Maui | 5.843 ms | 0.1167 ms | 0.1711 ms | 195.3125 | 62.5000 | 7.8125 | 3.02 MB |

---

## 6. 総評とハイライト

- **全フレームワークで初回生成 3ms〜4ms 台へ突入 (20%〜30% の高速化)**:
  - WPF の初回生成は **3.73 ms** (Baseline 比 -30.3%) を記録し、大幅な速度向上を達成。
- **インクリメンタル生成も 5ms 台へ高速化 (約 20% 高速化)**:
  - WPF 5.66 ms, Avalonia 5.67 ms, WinUI 5.85 ms, MAUI 5.84 ms と、全プラットフォームで 5ms 台の軽快な動作を実現。
- **メモリ割り当て (Allocated Memory) が約 20% 削減**:
  - 初回生成: 2.8〜2.9 MB → **2.2 MB 台** (~22% 削減)
  - インクリメンタル生成: 3.6〜3.7 MB → **2.9〜3.0 MB 台** (~18% 削減)
- **GC 負荷（Gen0/Gen1 コレクション回数）も顕著に低下。**

### 最大のボトルネック解消要因：インデントと後処理フォーマットの完全放棄
Phase 4 から Phase 5 にかけての驚異的なスコア向上（+60%超）の最大の要因は、**「コードの見た目（インデント等）を整形する処理を完全に捨て去ったこと」**です。
- 従来は、綺麗に字下げされたコードを出力するために、インデント用の空白文字列の割り当てや、Roslyn の `NormalizeWhitespace()` 等の重い処理を必要としていました。
- コンパイラにとってインデントの有無は解析に影響しないという割り切りから、インデント管理を完全に廃止し、すべて左詰め（フラット）で直接 `StringBuilder` に流し込むアーキテクチャ（`ClassScope` / `SourceWriter`）へと移行しました。
- これにより、無数の細かな空白文字列のヒープアロケーションと、重い構文木パース＆フォーマット処理が**根絶**され、CPU 時間とメモリ消費量の両面において飛躍的なパフォーマンス向上をもたらしました。
