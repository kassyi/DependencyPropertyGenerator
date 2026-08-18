# Kassyi.Generators.DependencyProperty

[English](README.md) | [日本語](README.ja.md)

[![Nuget package](https://img.shields.io/nuget/vpre/Kassyi.Generators.DependencyProperty)](https://www.nuget.org/packages/Kassyi.Generators.DependencyProperty/)
[![CI/CD](https://github.com/kassyi/DependencyPropertyGenerator/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/kassyi/DependencyPropertyGenerator/actions/workflows/main.yml)
[![License: MIT](https://img.shields.io/github/license/kassyi/DependencyPropertyGenerator)](https://github.com/kassyi/DependencyPropertyGenerator/blob/main/LICENSE)
[![Specifications](https://img.shields.io/badge/docs-specifications-blue.svg)](./spec/ja/intro.md)
[![Performance](https://img.shields.io/badge/performance-+30%25_faster-brightgreen.svg)](#完全なゼロアロケーションとideタイピング遅延の排除)
[![Zero Gen2 GC](https://img.shields.io/badge/Gen2_GC-zero_alloc-blue.svg)](#完全なゼロアロケーションとideタイピング遅延の排除)

完全なゼロアロケーションと極限の最適化を追求したDependency Property ソースジェネレーター。.NETランタイム級の大規模コードベースにおいて、マイクロ秒オーダーのスループットを実現。WPF、UWP、WinUI、Uno、Avalonia、MAUI をサポート。

## 本 Fork の特徴と改善点

本プロジェクトは、[`HavenDV/DependencyPropertyGenerator`](https://github.com/HavenDV/DependencyPropertyGenerator) をベースに独自に保守・極限まで最適化した Fork です。元リポジトリにおける深刻なサイレントバグ（[HavenDV#165](https://github.com/HavenDV/DependencyPropertyGenerator/issues/165): `OnChanged` コールバックの引数シグネチャ不一致時に警告なく `propertyChangedCallback: null` が生成されイベントが不発になる問題）を根絶し、厳格な型安全性を強制するとともに、**最大 30% の生成速度向上（総合スループットスコア +62.4% 向上）と Gen2 ガベージコレクション（GC）アロケーション完全ゼロ** を達成するようにアーキテクチャを再設計しました。

### 完全なゼロアロケーションとIDEタイピング遅延の排除

Incremental Source Generator は、開発者がコードを入力するたびにバックグラウンドで連続実行されます。従来のジェネレーターにおける重いメモリアロケーションは Gen2 GC を頻繁に引き起こし、Visual Studio や JetBrains Rider などの IDE でタイピング時に目に見える引っかかり（遅延）を発生させていました。

本ジェネレーターは、最大スループットを達成するためにパイプラインを根本から再構築しています：

- **Gen2 GC による停止時間をゼロ化**: メモリアロケーションを約 22% 削減（1クラスあたり約 650 KB の節約）。Gen2 GC の発生を完全に排除。
- **最適化されたパイプライン**: 重い構文木の再構築処理（`NormalizeWhitespace()`）を排除し、直接的なゼロアロケーション・ストリーミング生成へと置換。
- **ゼロアロケーションスコープ**: 中間文字列の生成を防ぐカスタム `ref struct` スコープハンドラー（`ClassScope` / `SourceWriter`）を実装。
- **宣言的ルールエンジン**: 実行時の文字列パースからコンパイル時のセマンティックフラグ解析へと移行。

<details>
<summary><b>ベンチマーク測定結果 (AMD Ryzen 9 7900X / .NET 9 - Phase 5)</b></summary>

| 測定項目                    | Upstream (元リポジトリ) | 本 Fork (Phase 5)           | 改善率                          |
| :-------------------------- | :---------------------- | :-------------------------- | :------------------------------ |
| **初回生成 (WPF)**          | 5.349 ms (2.87 MB)      | **3.729 ms (2.22 MB)**      | **-30.3% 時間 / -22.6% メモリ** |
| **差分生成 (WPF)**          | 7.176 ms (3.59 MB)      | **5.663 ms (2.93 MB)**      | **-21.1% 時間 / -18.4% メモリ** |
| **初回生成 (WinUI)**        | 5.720 ms (2.81 MB)      | **4.192 ms (2.21 MB)**      | **-26.7% 時間 / -21.4% メモリ** |
| **差分生成 (WinUI)**        | 7.412 ms (3.55 MB)      | **5.847 ms (2.94 MB)**      | **-21.1% 時間 / -17.2% メモリ** |
| **初回生成 (Avalonia)**     | 5.282 ms (2.86 MB)      | **4.137 ms (2.25 MB)**      | **-21.7% 時間 / -21.3% メモリ** |
| **差分生成 (Avalonia)**     | 7.103 ms (3.62 MB)      | **5.665 ms (3.01 MB)**      | **-20.2% time / -16.9% memory** |
| **初回生成 (MAUI)**         | 5.533 ms (2.90 MB)      | **4.147 ms (2.26 MB)**      | **-25.0% 時間 / -22.1% メモリ** |
| **差分生成 (MAUI)**         | 7.095 ms (3.67 MB)      | **5.843 ms (3.02 MB)**      | **-17.6% 時間 / -17.7% メモリ** |
| **総合スループットスコア**  | 1,000 pts (1,288 ops/s) | **1,624 pts (1,685 ops/s)** | **+62.4% スコア向上** 🚀        |
| **GC Gen2 発生回数 (初回)** | 7.8–15.6 / 1k ops       | **0.0000 / 1k ops**         | **100% 根絶 (完全 0 回)**       |

</details>

### 主な不具合修正と機能強化

- **厳格な型安全性の強制 (`#error DPG0001` / `DPG0007`)**: 元のジェネレーターでは、`OnChanged` コールバックのシグネチャが不一致の場合にエラーを出さず静かに無視（`propertyChangedCallback: null` を生成）してイベントが発火しなくなるサイレントバグ（[HavenDV#165](https://github.com/HavenDV/DependencyPropertyGenerator/issues/165)）が存在しました。本 Fork ではシグネチャの不一致をコンパイル時エラーとして即座に通知します。
- **Target-Typed `new(...)` 式の自動展開**: C# 9.0 以降の `DefaultValueExpression = "new(...)"` をシームレスにサポート。冗長な型名の指定なしに完全修飾されたコンストラクタ呼び出しへと自動展開します。
- **モジュラーアーキテクチャ**: モノリシックなロジックをフレームワークごとのストラテジー（WPF、WinUI、Avalonia 等）に分割し、保守性とテスト容易性を大幅に向上。
- **パッケージ名称の刷新**: 名前空間の衝突を防ぎクリーンに利用できるよう `Kassyi.Generators.DependencyProperty` として公開。

## インストール

.NET CLI または NuGet パッケージ マネージャーを使用してインストールします：

```bash
dotnet add package Kassyi.Generators.DependencyProperty
```

または、`.csproj` に直接パッケージ参照を追加します：

```xml
<PackageReference Include="Kassyi.Generators.DependencyProperty" Version="0.1.0" PrivateAssets="all" />
```

## クイックスタート

ジェネリック属性を使用してプロパティを宣言するだけで、ボイラープレートコードが自動的に生成されます。

```csharp
using DependencyPropertyGenerator;
using System.Windows.Controls;

#nullable enable

namespace MyApp.Controls;

[DependencyProperty<bool>("IsSpinning", DefaultValue = true, Category = "Category", Description = "Description")]
public partial class MyControl : UserControl
{
    // ジェネレーターによって自動呼出されるコールバック（任意）
    partial void OnIsSpinningChanged(bool oldValue, bool newValue)
    {
    }
}

[AttachedDependencyProperty<object, TreeView>("SelectedItem", DefaultBindingMode = DefaultBindingMode.TwoWay)]
public static partial class TreeViewExtensions
{
    // ジェネレーターによって自動呼出されるコールバック（任意）
    static partial void OnSelectedItemChanged(TreeView sender, object? oldValue, object? newValue)
    {
    }
}
```

<details>
<summary><b>生成されるコードを確認する</b></summary>

```csharp
// HintName: MyControl.Properties.IsSpinning.generated.cs
#nullable enable

namespace MyApp.Controls
{
    public partial class MyControl
    {
        public static readonly global::System.Windows.DependencyProperty IsSpinningProperty =
            global::System.Windows.DependencyProperty.Register(
                name: "IsSpinning",
                propertyType: typeof(bool),
                ownerType: typeof(global::MyApp.Controls.MyControl),
                typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
                    defaultValue: (bool)true,
                    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
                    propertyChangedCallback: static (sender, args) =>
                    {
                        ((global::MyApp.Controls.MyControl)sender).OnIsSpinningChanged(
                            (bool)args.OldValue,
                            (bool)args.NewValue);
                    }));

        [global::System.ComponentModel.Category("Category")]
        [global::System.ComponentModel.Description("Description")]
        public bool IsSpinning
        {
            get => (bool)GetValue(IsSpinningProperty);
            set => SetValue(IsSpinningProperty, value);
        }

        partial void OnIsSpinningChanged();
        partial void OnIsSpinningChanged(bool newValue);
        partial void OnIsSpinningChanged(bool oldValue, bool newValue);
    }
}
```

_(※ 簡潔にするため一部省略しています)_

</details>

## 高度な機能

### デフォルト値における Target-Typed `new(...)` 式

C# 9.0 以降の `new()` 式を利用してクリーンにデフォルト値を定義できます。ジェネレーターはこれを完全修飾型へと安全に展開します。

```csharp
public readonly record struct Data(int Value);

// プロパティの型に合わせて自動展開 (new global::MyNamespace.Data(42))
[AttachedDependencyProperty<Data, TreeView>("SelectedItem", DefaultValueExpression = "new(42)")]

// デフォルトコンストラクタもサポート:
[AttachedDependencyProperty<Data, TreeView>("SelectedItem", DefaultValueExpression = "new()")]
```

### イベントバインディング

UI イベントに連動したプロパティを自動管理できます。

```csharp
[AttachedDependencyProperty<object, Grid>("BindEventProperty", BindEvent = nameof(Grid.MouseWheel), DefaultValueExpression = "new()")]
public static partial class GridExtensions
{
    private static void OnBindEventPropertyChanged_MouseWheel(object? sender, System.Windows.Input.MouseWheelEventArgs args)
    {
        // マウスホイールイベントの処理
    }
}
```

プロパティの値が変更された際、旧値の購読解除と新値の購読（`sender.MouseWheel += ...`）が自動的に行われます。

### XML ドキュメントコメント

XML ドキュメントを生成する最も簡単な方法は `Description` プロパティの指定です：

```csharp
[DependencyProperty<bool>("IsSpinning", Description = "要素が回転中であるかを示します。")]
```

これにより `[Description]` 属性が付与されると同時に、生成される XML ドキュメントコメントにもテキストが直接埋め込まれます。未加工の XML を指定したい場合は `XmlDocumentation` や `PropertyXmlDocumentation` を使用します。

<details>
<summary><b>生成される XML ドキュメントコードを確認</b></summary>

```csharp
/// <summary>
/// Identifies the <see cref="IsSpinning"/> dependency property.<br/>
/// Default value: default(bool)
/// </summary>
public static readonly global::System.Windows.DependencyProperty IsSpinningProperty =
    global::System.Windows.DependencyProperty.Register(...);

/// <summary>
/// 要素が回転中であるかを示します。<br/>
/// Default value: default(bool)
/// </summary>
[global::System.ComponentModel.Description("要素が回転中であるかを示します。")]
public bool IsSpinning
{
    get => (bool)GetValue(IsSpinningProperty);
    set => SetValue(IsSpinningProperty, value);
}
```

</details>

### プラットフォームの手動設定

自動検出がうまく機能しない環境（マルチプラットフォーム構成や特殊なビルド時）では、`.csproj` 内で明示的にターゲットフレームワークを指定できます：

```xml
<PropertyGroup>
  <DefineConstants>$(DefineConstants);HAS_WPF</DefineConstants>
  <!-- <DefineConstants>$(DefineConstants);HAS_UNO</DefineConstants> -->
  <!-- <DefineConstants>$(DefineConstants);HAS_UNO_WINUI</DefineConstants> -->
  <!-- <DefineConstants>$(DefineConstants);HAS_AVALONIA</DefineConstants> -->
</PropertyGroup>
```

### UWP / WinUI / Uno におけるメタデータ登録

UWP、WinUI、Uno では、ジェネレーターが `RegisterPropertyChangedCallbacks()` メソッドを出力します。プロパティ変更通知コールバックを正しく動作させるため、コントロールのコンストラクタ内でこのメソッドを手動で呼び出してください。

## 前提条件

> [!IMPORTANT]
> **C# 言語バージョンの設定 (`LangVersion`)**
> ジェネリック属性（`[DependencyProperty<T>]` や `[RoutedEvent<T>]`）を使用する場合、`.csproj` の `LangVersion` を **`11.0` 以上**（または `preview` / `latest`）に設定してください：
>
> ```xml
> <PropertyGroup>
>   <LangVersion>11.0</LangVersion> <!-- または preview / latest -->
> </PropertyGroup>
> ```
>
> ※ 非ジェネリック属性（`[DependencyProperty("Name", typeof(Type))]`）を使用する場合は、従来の言語バージョン（C# 8.0+）でも動作します。

## アーキテクチャと公式仕様書

パイプライン内部構造、ゼロアロケーション設計、計算量モデル、フレームワーク別生成仕様についての詳細は、仕様書をご覧ください：

- **[仕様書概要・インデックス (spec/ja/intro.md)](./spec/ja/intro.md)**
    - **[01. 基盤とドメインデータ](./spec/ja/01_foundation_and_domain.md)**: DTO構造と対象プラットフォーム
    - **[02. パイプラインとアーキテクチャ](./spec/ja/02_pipeline_architecture.md)**: 差分検出・等価性キャッシュ戦略
    - **[03. コード生成とパフォーマンス最適化](./spec/ja/03_synthesis_and_performance.md)**: `SourceWriter`、コールバック照合、最適化ガイドライン
    - **[04. 計算量モデル](./spec/ja/04_mathematical_model.md)**: 最悪時間・メモリ計算量の数理分析
    - **[05. テスト仕様書](./spec/ja/05_test_specification.md)**: 直交表マトリクスと品質保証基準
    - **[06. フレームワーク別生成仕様](./spec/ja/06_framework_strategies.md)**: プラットフォーム固有APIマッピング
    - **[07. 診断エラーコード一覧](./spec/ja/07_diagnostics_reference.md)**: `DPG0000`〜`DPG9999` の原因と解決策

## サポートとフィードバック

- **不具合報告・Issue**: [GitHub Issues](https://github.com/kassyi/DependencyPropertyGenerator/issues)
- **ディスカッション・アイデア**: [GitHub Discussions](https://github.com/kassyi/DependencyPropertyGenerator/discussions)
