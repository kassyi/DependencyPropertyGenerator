# 03. 生成戦略と最適化

[English](../en/03_generation_and_optimization.md) | [日本語](./03_generation_and_optimization.md) | [目次 (Intro)](./intro.md)

## Ⅰ. インターフェース仕様と生成コードの構造

ジェネレーターは、抽出したDTO（`DependencyPropertyData`など）をもとに、WPF、MAUI、Avalonia、Uno、WinUIなどのフレームワークに対応したC#コードをテキストとして出力します。生成するコードは、ユーザーが記述したクラスを拡張する形（`partial` クラス）で提供します。

### 境界とコントラクト
**入力 (User Code)** は、`partial` 修飾子を付けたクラス宣言と `[DependencyProperty]` などの属性です。オプションとして `partial void On...Changed()` の宣言を受け付けます。
**出力 (Generated Code)** は、以下の要素を含みます。
- 依存関係プロパティの静的フィールド (`...Property`)
- CLR プロパティラッパー (`get` / `set`)
- プロパティ変更時のコールバックメソッドの実装 (`propertyChangedCallback`)
- XMLドキュメントコメント

---

## Ⅱ. コード生成エンジン仕様 (`SourceWriter` と `ClassScope`)

生成コードの出力には、`Kassyi.Generators.Extensions` の `SourceWriter` を使用します。本プロジェクトでは、コード生成時の定型ボイラープレートを排除し、かつゼロアロケーションで安全なインデント・スコープ管理を実現するため、以下の機構を採用しています。

### 1. 外殻スコープの単一化 (`writer.ClassScope(@class)`)
すべての生成ファイルに共通する定型のボイラープレート（`#nullable enable` → `namespace` → `partial class`）を、`ClassScope` ヘルパーにより1行でカプセル化します。

```csharp
// 外殻（#nullable enable, namespace, partial class）を1行で生成しスコープを管理する
using var _ = writer.ClassScope(@class);

// このスコープ内にプロパティやメソッドの生成ロジックを記述する
```

この手法はゼロアロケーションで機能します。`ref struct SourceWriterClassScope` を返すことで、ヒープアロケーションを発生させずに、スコープ終了時の閉じブレース `}` の出力を保証します。

### 2. ブロックヘッダーのスコープ直接渡し (`writer.Scope(...)`)
メソッドや静的コンストラクタのブロック開始時に、ヘッダー文字列を直接 `Scope` メソッドに渡します。

```csharp
using (writer.Scope($"static {@class.Name}()"))
{
    // 静的コンストラクタ内部の登録コード
}
```

---

## Ⅲ. プロパティとコールバックの解決仕様

### デフォルト値式 (`DefaultValueExpression`) の自動補完ルール
`[DependencyProperty<T>("Name", DefaultValueExpression = "...")]` において、`DefaultValueExpression` に指定した文字列が `new(...)` または `new (...)`（C# 9.0+ の target-typed `new` 構文）で始まる場合、ジェネレーター抽出処理（`PrepareData`）時にプロパティの型 `T` の完全修飾型名（`global::...`）へと自動的に置換・展開します。

- 入力例: `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
- 展開後: `new global::MyNamespace.MyProfile(1.5, 48.0)`

これにより、他の名前空間にある型をデフォルト値としてインスタンス化する際にも、属性の文字列内で手動で冗長な完全修飾名を書く必要がなくなり、コードの保守性と視認性が向上します。

### コールバックメソッド (`OnChanged` / `OnChanging`) の解決規則と制約

#### 1. シグネチャ照合ルールエンジン (`IMethodSignatureRule`)
コールバックシグネチャの照合は、`Rules/Signatures/` 内の個別ルール（`NoParametersRule`, `SingleParameterRule`, `DoubleParameterRule`, `TripleParameterRule`）によって判定します。

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
    // 🚨 未対応のシグネチャ（例: 4引数など）や未定義の場合
    // ジェネレーターが #error DPG0001 を出力し、明示的にコンパイルエラー（ビルド停止）を報告する。
    private void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra) { }
}

// ----------------------------------------------------------------------------
// 方式 B: 属性指定なしで partial void On...Changed() の自動一致に頼る場合
// ----------------------------------------------------------------------------
[DependencyProperty<string>("Text")]
public partial class MyControl : UserControl
{
    // 🚨 未対応のシグネチャ（例: 4引数や (DependencyObject, DependencyPropertyChangedEventArgs)）の場合
    // 以前は無関係なメソッドとしてサイレントに無視されていましたが、現在はジェネレーターが #error DPG0007 を出力し、
    // 明示的にコンパイルエラー（ビルド停止）を報告してサイレントバグを防止します。
    private void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
}
```

#### 3. コールバックシグネチャ不一致エラー (DPG0007) のトラブルシューティング (Agentic Ground Truth)

メソッド名が `On...Changed` に一致しているにもかかわらず、パラメータのシグネチャが未対応の場合、ジェネレーターは `DPG0007` エラーを出力してビルドを停止させ、イベントがサイレントに無視されるバグ（`propertyChangedCallback: null` の生成）を未然に防ぎます。

この問題の最も一般的な原因は、WPF開発者が習慣的に `private void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)` というWPF標準のコールバックシグネチャを手動で定義してしまうことにあります。しかし本ジェネレーターのルールエンジンは、より強固な型安全性を保証するため、第1引数に汎用的な `DependencyObject` を取るメソッドを意図的にサポート対象外としています。

このエラーを解決するには、メソッドの第1引数を `DependencyObject` ではなく、プロパティを定義しているクラス自身の型（例えば `MyControl sender`）に変更してください。また、ジェネレーターが背後で自動的に static なプロキシメソッドを生成して結線を行うため、ユーザーコード側で定義するコールバックは `static` メソッドではなく、通常のインスタンスメソッドとして定義する必要があります。

---

## Ⅳ. パフォーマンス最適化ルール

インクリメンタル・ソースジェネレーターのパフォーマンス（特にVisual StudioやRiderなどのIDE上でのタイピング時の応答速度）を極限まで高めるため、以下の最適化プラクティスを厳守します。

### ベンチマーク実証済みの原則
- **文字列パースよりASTノード直接走査**: 式の解析において、一度抽出した文字列を再度パース (`SyntaxFactory.ParseExpression()`) する手法に比べ、`ExpressionSyntax` ノードを直接AST走査する手法は、再トークナイズや中間構文木の生成アロケーションを完全に回避できるため大幅に高速かつ省メモリです。ジェネレーターのホットパスでの文字列からの再パースは行わないでください。
- **SyntaxFactoryよりSourceWriterによるコード生成**: ソース生成のホットパスでは、`SyntaxFactory.NormalizeWhitespace().ToFullString()` による構文木構築よりも、[`SourceWriter`](../../src/Kassyi.Generators.Extensions/SourceWriter.cs) (カスタム補間文字列ハンドラー) による直接出力が推奨されます（※ユニットテストや非ホットパスでの構文解析・検証用途では `SyntaxFactory` の使用も許容されます）。

### Dos (推奨事項)
- **`ForAttributeWithMetadataName` の使用**: 属性ベースで構文をフィルタリングするRoslyn 4.3以降のAPIを使用し、対象外コードの変更によるジェネレーターの起動を最小化します。
- **データ抽出の早期実行**: `SyntaxNode` や `ISymbol` を受け取ったら、直ちにプリミティブな型や値レコードに変換してDTOに格納します。
- **`EquatableArray<T>` の使用**: コレクションを扱う場合は、必ず構造的等価性が保証される `EquatableArray<T>` でラップします。
- **ホットパスでのLINQ排除**: パイプライン抽出処理や内部ループでは LINQ (`.Select()`, `.Where()`, `.Any()`) を避け、インデックスベースの `for` ループを使用することで不要なイテレータ・アロケーションを完全に排除します。
- **属性引数の辞書事前キャッシュ**: 属性の `NamedArguments` 解決では、探索ごとにLINQ検索するのではなく、`Dictionary` などへ事前にキャッシュして $O(1)$ でアクセスします。

### Don'ts (禁止事項)
- **`ISymbol` や `SyntaxNode` をDTOに含めない**: これらを保持したまま `Select` を抜けると、メモリリークとキャッシュミスの二重障害を引き起こします。
- **DTO内で `List<T>` や `T[]` を直接使わない**: 参照比較となるため、中身が同一でもキャッシュが無効化されます。
- **文字列生成時の中間アロケーション（無駄なヒープ割り当て）**: `string.Split()` や `string.Join()`、不要な `List<string>` の生成などはGCスパイクの原因となります。代わりに `SourceWriter`、`StringBuilder`、インデックススキャン、`stackalloc Span<char>` などを活用します。

---

## Ⅴ. プロファイリング手法

ジェネレーターのパフォーマンスやボトルネックを調査する際は、以下の手法を用います。

1. **ビルドログ解析 (`.binlog`)**
   ```bash
   dotnet build -c Release -bl:msbuild.binlog
   ```
   生成した `msbuild.binlog` を MSBuild Structured Log Viewer で開き、Generatorの実行時間をミリ秒単位で確認します。

2. **ジェネレーターのベンチマーク**
   BenchmarkDotNet を使用し、`CSharpGeneratorDriver` に擬似的なソースコードを流し込んで、実行時間とメモリのアロケーション量 (Gen0/Gen1/Gen2, Allocated Bytes) を計測します。
