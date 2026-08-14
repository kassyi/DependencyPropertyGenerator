# 03. 生成戦略と最適化 (Generation & Optimization)

[English](../en/03_generation_and_optimization.md) | [日本語](./03_generation_and_optimization.md) | [目次 (Intro)](./intro.md)

## Ⅰ. インターフェース仕様 (生成されるコードの構造)

ジェネレーターは、抽出したDTO（`DependencyPropertyData`等）をもとに、WPF/MAUI/Avalonia/Uno/WinUIなどのフレームワークに対応したC#コードをテキストとして出力します。
生成されるコードは、ユーザーが記述したクラスを拡張する形（`partial` クラス）で提供されます。

### 境界とコントラクト
- **入力 (User Code)**: `partial` 修飾子が付けられたクラス宣言と `[DependencyProperty]` などの属性。オプションとして `partial void On...Changed()` の宣言。
- **出力 (Generated Code)**: 
  - 依存関係プロパティの静的フィールド (`...Property`)
  - CLR プロパティラッパー (`get` / `set`)
  - プロパティ変更時のコールバックメソッドの実装 (`propertyChangedCallback`)
  - XMLドキュメントコメント

---

## Ⅱ. コード生成エンジン仕様 (`SourceWriter` & `ClassScope`)

生成コードの出力には、`Kassyi.Generators.Extensions` の `SourceWriter` を使用します。
本プロジェクトでは、コード生成時の定型ボイラープレートを排除し、かつゼロアロケーションで安全なインデント・スコープ管理を実現するため、以下の機構を採用しています。

### 1. 外殻スコープの単一化 (`writer.ClassScope(@class)`)
すべての生成ファイル共通の定型ボイラープレート（`#nullable enable` → `namespace` → `partial class`）を `ClassScope` ヘルパーにより1行でカプセル化します。

```csharp
// 外殻（#nullable enable, namespace, partial class）を1行で生成・スコープ管理
using var _ = writer.ClassScope(@class);

// このスコープ内にプロパティやメソッドの生成ロジックを記述
```

- **ゼロアロケーション**: `ref struct SourceWriterClassScope` を返すことで、ヒープアロケーションなしにスコープ終了時の閉じブレース `}` の出力を保証します。

### 2. ブロックヘッダーのスコープ直接渡し (`writer.Scope(...)`)
メソッドや静的コンストラクタのブロック開始時に、ヘッダー文字列を直接 `Scope` メソッドに渡します。

```csharp
using (writer.Scope($"static {@class.Name}()"))
{
    // 静的コンストラクタ内部の登録コード
}
```

---

## Ⅲ. プロパティ・コールバック解決仕様

### デフォルト値式 (`DefaultValueExpression`) の自動補完ルール
- `[DependencyProperty<T>("Name", DefaultValueExpression = "...")]` において、`DefaultValueExpression` に指定された文字列が `new(...)` または `new (...)`（C# 9.0+ の target-typed `new` 構文）で始まる場合、ジェネレーター抽出処理（`PrepareData`）時にプロパティの型 `T` の完全修飾型名（`global::...`）へと自動的に置換・展開します。
  - 入力例: `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
  - 展開後: `new global::MyNamespace.MyProfile(1.5, 48.0)`
- これにより、他の名前空間にある型をデフォルト値としてインスタンス化する際にも、属性の文字列内で手動で冗長な完全修飾名を書く必要がなくなり、コードの保守性と視認性が向上します。

### コールバックメソッド (`OnChanged` / `OnChanging`) の解決規則と制約

#### 1. シグネチャ照合ルールエンジン (`IMethodSignatureRule`)
コールバックシグネチャの照合は、`Rules/Signatures/` 内の個別ルール（`NoParametersRule`, `SingleParameterRule`, `DoubleParameterRule`, `TripleParameterRule`）によって判定されます。

```csharp
// ✅ 0引数 (NoParametersRule)
partial void OnTextChanged();

// ✅ 1引数 (SingleParameterRule: 新値 または EventArgs)
partial void OnTextChanged(string newValue);
partial void OnTextChanged(DependencyPropertyChangedEventArgs e);

// ✅ 2引数 (DoubleParameterRule: 旧値・新値 / sender・新値 / sender・EventArgs)
partial void OnTextChanged(string oldValue, string newValue);
partial void OnTextChanged(MyControl sender, string newValue);
partial void OnTextChanged(MyControl sender, DependencyPropertyChangedEventArgs e);

// ✅ 3引数 (TripleParameterRule: sender・旧値・新値)
partial void OnTextChanged(MyControl sender, string oldValue, string newValue);

// ❌ 4引数以上はサポート外 (渡せるデータが存在しないため無視される)
void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra);
```

#### 2. エラー判定と挙動の差異

```csharp
// ----------------------------------------------------------------------------
// 方式 A: [DependencyProperty] の OnChanged パラメータで明示指定する場合
// ----------------------------------------------------------------------------
[DependencyProperty<string>("Text", OnChanged = nameof(OnTextChanged))]
public partial class MyControl : UserControl
{
    // 🚨 未対応のシグネチャ（例: 4引数など）や未定義の場合:
    // ジェネレーターが #error DPG0001 を出力し、明示的にコンパイルエラー（ビルド停止）を報告する
    private void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra) { }
}

// ----------------------------------------------------------------------------
// 方式 B: 属性指定なしで partial void On...Changed() の自動一致に頼る場合
// ----------------------------------------------------------------------------
[DependencyProperty<string>("Text")]
public partial class MyControl : UserControl
{
    // ⚠️ 未対応のシグネチャ（例: 4引数や (DependencyObject, DependencyPropertyChangedEventArgs)）の場合:
    // 無関係なプライベートメソッドとして無視され、propertyChangedCallback: null が生成される。
    // 💡 サイレント無視を防ぐため、重要な処理は 方式 A (OnChanged = nameof(...)) ででの明示指定を推奨。
    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
}
```

---

## Ⅳ. パフォーマンス最適化ルール (Dos & Don'ts)

インクリメンタル・ソースジェネレーターのパフォーマンス（特にVisual StudioやRider等のIDE上でのタイピング時の応答速度）を極限まで高めるため、以下の最適化プラクティスを厳守します。

### 🟢 Dos (推奨事項)
- **`ForAttributeWithMetadataName` を使う**: 属性ベースで構文をフィルタリングするRoslyn 4.3+のAPIを使用し、対象外コードの変更によるジェネレーター起動を最小化します。
- **データ抽出は早期に行う**: `SyntaxNode` や `ISymbol` を受け取ったら、直ちにプリミティブな型や値レコードに変換してDTOに格納します。
- **`EquatableArray<T>` を使う**: コレクションを扱う場合は、必ず構造的等価性が保証される `EquatableArray<T>` でラップします。
- **ホットパスでのLINQ排除**: パイプライン抽出処理や内部ループでは LINQ (`.Select()`, `.Where()`, `.Any()`) を避け、インデックスベースの `for` ループを使用することで不要なイテレータ・アロケーションを完全排除します。
- **属性引数の辞書事前キャッシュ**: 属性の `NamedArguments` 解決では、探索ごとにLINQ検索するのではなく `Dictionary` 等へ事前キャッシュして $O(1)$ でアクセスします。

### 🔴 Don'ts (禁止事項)
- **❌ `ISymbol` や `SyntaxNode` をDTOに含めない**: 
  - これらを保持したまま `Select` を抜けると、メモリリークとキャッシュミスの二重障害を引き起こします。
- **❌ DTO内で `List<T>` や `T[]` を直接使わない**:
  - 参照比較となるため、中身が同一でもキャッシュが無効化されます。
- **❌ 文字列生成時の中間アロケーション（無駄なヒープ割り当て）**:
  - `string.Split()` や `string.Join()`、不要な `List<string>` の生成などはGCスパイクの原因となります。
  - 代わりに `SourceWriter`、`StringBuilder`、インデックススキャン、`stackalloc Span<char>` などを活用します。

---

## Ⅴ. プロファイリング手法

ジェネレーターのパフォーマンス・ボトルネックを調査する際は、以下の手法を用います。

1. **ビルドログ解析 (`.binlog`)**
   ```bash
   dotnet build -c Release -bl:msbuild.binlog
   ```
   生成された `msbuild.binlog` を [MSBuild Structured Log Viewer](https://msbuildlog.com/) で開き、Generatorの実行時間をミリ秒単位で確認します。

2. **ジェネレーターのベンチマーク**
   BenchmarkDotNet を使用し、`CSharpGeneratorDriver` に擬似的なソースコードを流し込んで、実行時間とメモリのアロケーション量 (Gen0/Gen1/Gen2, Allocated Bytes) を計測します。
