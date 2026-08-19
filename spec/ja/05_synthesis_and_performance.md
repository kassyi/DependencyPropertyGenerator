# 05. コード生成とパフォーマンス最適化

[English](../en/05_synthesis_and_performance.md) | [日本語](./05_synthesis_and_performance.md) | [目次 (Intro)](./intro.md)

## Ⅰ. インターフェース仕様と生成コードの構造

ジェネレーターは、抽出された Data Transfer Object（`DependencyPropertyData` 等）を入力とし、対象フレームワーク（WPF、MAUI、Avalonia、Uno、WinUI）に最適化されたC#ソースコードを生成する。このコードは、ユーザーが宣言した `partial` クラスを拡張する形で提供される。

### 境界とコントラクト

**入力制約 (User Code)**
ジェネレーターは以下の条件を満たすコードを処理対象とする。

- `partial` 修飾子が付与されたクラス宣言。
- `[DependencyProperty]` または関連する属性が付与されたクラス。
- 任意のフック宣言（`partial void On...Changed()`）。

**出力成果物 (Generated Code)**
生成されるコードは以下の構造要素で構成される。

- 依存関係プロパティの静的フィールド（通常は `...Property` サフィックスを伴う）。
- `get` および `set` アクセサを実装する CLR プロパティラッパー。
- `propertyChangedCallback` にバインドされるプロパティ変更コールバックの実装。
- 包括的な XML ドキュメントコメント。

---

## Ⅱ. コード生成エンジン仕様

ソースコードの出力フェーズは、`Kassyi.Generators.Extensions` 名前空間の `SourceWriter` によって管理される。本アーキテクチャでは、定型的なボイラープレートを排除し、ゼロアロケーションと安全なインデントスコープのライフサイクルを強制するために、特定の構造パターンを標準化している。

### 1. クラス全体を囲むスコープヘルパー

ジェネレーターは、反復的な構造的ボイラープレートを単一のメソッド呼び出しでカプセル化する。このエンベロープには `#nullable enable` ディレクティブ、`namespace` 宣言、外部のネストされた親クラス、および対象の `partial class` 定義が含まれる。

```csharp
// ClassScope ヘルパーにより、完全な外殻エンベロープを1回の操作で生成する。
using var _ = writer.ClassScope(@class);

// 中核となるメンバーの生成ロジックがこれに続く。
```

> [!TIP]
> このアプローチは完全に**ゼロアロケーション**で機能する。`ClassScope` メソッドは `ref struct SourceWriterClassScope` を返す。この構造体は破棄時に、開かれたすべてのネストクラスおよび名前空間に対する閉じブレースをヒープ割り当てなしで出力する。

> [!NOTE]
> 対象クラスが `ClassData.ParentClasses` に定義された親クラスの内部にネストされている場合、`ClassScope` は外側から内側のスコープへと順に `partial class` を展開する。その後、破棄時に逆順で自動的にスコープを閉じる。

### 2. ブロックヘッダーのスコープ直接渡し

ブロックのインデントを管理するため、ジェネレーターはメソッドや静的コンストラクタのシグネチャを直接 `Scope` メソッドに渡す。

```csharp
using (writer.Scope($"static {@class.Name}()"))
{
    // 静的コンストラクタ内部の登録コード
}
```

---

## Ⅲ. プロパティとコールバックの解決仕様

### Target-Typed オブジェクト生成の自動展開

`PrepareData` 抽出フェーズでは、Target-Typed な `new` 式が自動的に展開される。`DefaultValueExpression` が C# 9.0 以降の構文に基づく `new(...)` または `new (...)` で開始される場合、パイプラインはそれを完全修飾されたグローバルな型名へと変換する。

**変換プロセスの例**

- **入力:** `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
- **出力:** `new global::MyNamespace.MyProfile(1.5, 48.0)`

この展開機構により、文字列リテラル内での冗長な名前空間の手動指定が不要となり、コードの明確性が向上する。同時に、外部の名前空間から型をインスタンス化する際のリファクタリング耐性も高まる。

### C# 13 partial プロパティ構文の自動解決

ユーザーコードが `public partial int Value { get; set; }` のように C# 13 の partial プロパティ構文で定義されている場合、ジェネレーターは `Modifiers.IsPartialProperty` を自動検出し、プロパティの getter / setter 実装ブロックを出力する。これにより、従来型のプロパティ生成と partial プロパティ生成の双方が透過的にサポートされる。

### コールバックメソッドの照合規則

#### 1. シグネチャ照合ルールエンジン

ジェネレーターは `Rules/Signatures/` ディレクトリに配置された専用のルールクラスを利用してコールバックシグネチャを解決する。このエンジンは、パラメータの上限および型の要件を厳格に適用する。

**サポートされるシグネチャ要件**

- **0引数:** `NoParametersRule` により処理される。
- **1引数:** `SingleParameterRule` により処理される。新しい値、または `EventArgs` を許容する。
- **2引数:** `DoubleParameterRule` により処理される。旧値と新値のペア、送信元と新値のペア、または送信元と `EventArgs` のペアを許容する。
- **3引数:** `TripleParameterRule` により処理される。送信元、旧値、および新値の組み合わせを許容する。

> [!WARNING]
> 4つ以上のパラメータを定義するシグネチャはサポートの対象外となる。内部的に提供可能な引数が存在しないため、ルールエンジンによって明示的に無視される。

```csharp
// 有効な2引数シグネチャの例:
partial void OnTextChanged(string oldValue, string newValue);

// サポート対象外となる4引数シグネチャの例:
void OnTextChanged(MyControl sender, string oldValue, string newValue, object extra);
```

#### 2. エラー報告とコンパイル時の安全性

無効なコールバックシグネチャに対して厳格なコンパイルエラーを適用することで、実行時のサイレントな障害を防止する。
明示的なメソッド指定が不正な場合は `DPG0001`、規約に基づく自動検出メソッドのシグネチャが不正な場合は `DPG0007` が発生する。

> [!IMPORTANT]
> **サイレント不具合の根絶 ([HavenDV#165](https://github.com/HavenDV/DependencyPropertyGenerator/issues/165))**
> 古いジェネレーターにおける「シグネチャが合わない場合に警告なしで無視する」というサイレントバグは、本アーキテクチャでは厳格なコンパイル時診断として即座に通知され、完全に根絶されている。

#### 3. シグネチャ不一致のトラブルシューティング

`DPG0001` や `DPG0007` などの診断が発生する最も頻繁な要因は、標準的なWPFシグネチャと汎用的な `DependencyObject` パラメータを用いてコールバックを定義することにある。厳密な型安全性を強制するため、ジェネレーターのルールエンジンは汎用的な `DependencyObject` を引数として受け入れることを明示的に拒否する。

> [!NOTE]
> 各診断エラー（`DPG0001`〜`DPG0008`）の具体的な発生原因と解決策のコード例については、**[08. 診断エラーコード一覧 (Diagnostics Reference)](./08_diagnostics_reference.md)** を参照のこと。

---

## Ⅳ. パフォーマンス最適化ルール

IDE入力時の応答性を最大限に維持するため、アーキテクチャは厳格なパフォーマンスガイドラインを強制する。ジェネレーターを拡張する際は、以下の原則を遵守しなければならない。

> [!NOTE]
> **過去のベンチマーク実績と最適化レポート**
> 本アーキテクチャで行われたフェーズ別の詳細なベンチマーク測定結果およびパフォーマンス改善実績は、[`tests/Kassyi.Generators.DependencyProperty.Benchmarks`](../../tests/Kassyi.Generators.DependencyProperty.Benchmarks)（特に `Reports/` ディレクトリ配下の `Phase0` ～ `Phase5` レポート）に記録されている。

### ベンチマーク実証済みの原則

> [!TIP]
> **文字列パースを排除した AST ノード直接走査**
> 式の解析においては、直接的な `ExpressionSyntax` 抽象構文木走査を利用する。このアプローチは、再トークナイズや中間構文木の生成アロケーションを完全に回避する。抽出された文字列を `SyntaxFactory.ParseExpression()` で再パースする手法と比較して、実行速度が大幅に向上し、メモリ消費量も低減される。ジェネレーターのホットパス内での文字列再パースは厳格に禁止される。

> [!TIP]
> **コード生成における SyntaxFactory の SourceWriter への置換**
> コード生成のホットパスにおいては、カスタム補間文字列ハンドラーである `SourceWriter` を利用して直接コードを出力する。この手法は、`SyntaxFactory.NormalizeWhitespace().ToFullString()` を介した重い構文木構築とフォーマット処理をパフォーマンスで凌駕する。

> [!NOTE]
> `SyntaxFactory` の使用は、非ホットパスまたはユニットテスト環境内でのみ許容される。

### ベストプラクティス

- **対象を絞り込んだ宣言フィルタリング:** `ForAttributeWithMetadataName` を利用して、属性に基づく宣言のフィルタリングを実行する。これによりジェネレーターの呼び出し回数が劇的に制限される。非推奨となった syntax receiver の使用は禁止される。
- **プリミティブ型への早期投影:** 初期抽出フェーズにおいて、`SyntaxNode` または `ISymbol` インスタンスを、直ちにプリミティブ型または読み取り専用のレコード構造体へと変換する。
- **コレクションの等価性保証:** Data Transfer Object 内のすべてのコレクション型を `EquatableArray<T>` でラップし、要素ごとの等価性チェックを強制する。
- **LINQ の排除:** ホットな抽出およびフォーマット処理の内部では、`.Select()`, `.Where()`, `.Any()` などの LINQ 演算子をインデックスベースの `for` ループに置き換える。これによりイテレータのメモリアロケーションを防止する。
- **属性引数の事前キャッシュ:** `NamedArguments` を辞書データ構造を利用してキャッシュし、$O(1)$ のプロパティ探索を保証する。

### アーキテクチャのアンチパターン

> [!CAUTION]
> **コンパイル参照の保持**
> `ISymbol` または `SyntaxNode` を Data Transfer Object 内に保持してはならない。この実装は深刻なメモリリークを引き起こし、インクリメンタルパイプラインにおける 100% のキャッシュミスを誘発する。

> [!CAUTION]
> **ミュータブルなコレクション型の使用**
> Data Transfer Object 内で生の `List<T>` または `T[]` を利用してはならない。デフォルトの参照比較がインクリメンタルキャッシュの動作を無効化する。

> [!WARNING]
> **中間文字列の割り当て**
> ホットパス内で中間文字列をアロケートしてはならない。`string.Split()` や `string.Join()` などの操作は回避する。ガベージコレクションのスパイクを防ぐため、`SourceWriter`、`StringBuilder`、および `stackalloc Span<char>` を活用する。

---

## Ⅴ. プロファイリング手法

ジェネレーターパイプライン内のパフォーマンスボトルネックを調査する際は、以下の診断手法を適用する。

**1. MSBuild 構造化ログ解析**
ビルドプロセス中にバイナリログを生成し、ジェネレーターの実行時間を検査する。生成された `msbuild.binlog` は MSBuild Structured Log Viewer を利用して解析する。

```bash
dotnet build -c Release -bl:msbuild.binlog
```

**2. BenchmarkDotNet 実行 (Execution)**
人工的なソースツリーを BenchmarkDotNet を使って `CSharpGeneratorDriver` に入力する手法。実行時間だけでなく、Gen0 / Gen1 / Gen2 ヒープにおける正確なメモリアロケーションを測定できる。

---

## Ⅵ. パフォーマンス指標 (Performance Metrics)

これらのアーキテクチャ変更を検証するため、標準的な Roslyn の手法（SyntaxFactory）と独自のトークンストリーミング手法を比較する継続的なベンチマークを実施しています。ベンチマークのソースコードや詳細な計測手法については、[`tests/Kassyi.Generators.DependencyProperty.Benchmarks`](../../tests/Kassyi.Generators.DependencyProperty.Benchmarks) プロジェクトを参照してください。

### 1. マイクロベンチマーク: AST Mutation vs. Token Streaming

_シナリオ:_ ターゲット型のデフォルト式 (`new(1, 2, 3)`) を明示的なインスタンス化 (`new global::System.Collections.Generic.List<string>(1, 2, 3)`) へと変換する処理。

| 方式                                        | 実行時間 (Mean) | 速度比                  | Gen0   | Gen1   | Gen2       | メモリアロケーション | アロケーション比     |
| :------------------------------------------ | :-------------- | :---------------------- | :----- | :----- | :--------- | :------------------- | :------------------- |
| **Roslyn AST Mutation** (`SyntaxFactory`)   | 16,718.6 ns     | 1.00x                   | 0.6409 | 0.2441 | **0.0610** | 9,712 B              | 1.00                 |
| **Direct Token Streaming** (`SourceWriter`) | **365.4 ns**    | **0.02x (約46倍 高速)** | 0.0143 | **-**  | **-**      | **240 B**            | **0.02 (97.5%削減)** |

- _Roslyn AST Mutation:_ `SyntaxFactory.ParseTypeName` → `SyntaxFactory.ObjectCreationExpression` → `.NormalizeWhitespace().ToFullString()` と処理を進めます。この過程で、ヒープ上に再帰的な AST ノードツリーと Trivia（空白やコメントなどの構文要素）リストがアロケーションされます。
- _Direct Token Streaming:_ 解析済みの AST から既存のトークンや Trivia (`ArgumentList`, `Initializer`) を直接切り出し、中間の構文ツリーを一切アロケーションすることなく出力バッファへと直接流し込みます。

### 2. エンドツーエンドのジェネレーターパイプライン

_実行環境: WPF 向け生成, AMD Ryzen 9 7900X_

| フェーズ                          | 実行時間 (ms) | Gen0       | Gen1       | Gen2      | メモリアロケーション |
| :-------------------------------- | :------------ | :--------- | :--------- | :-------- | :------------------- |
| **ベースライン (旧パイプライン)** | 5.34 ms       | 187.5      | 62.5       | 7.8       | 2.87 MB              |
| **v4 パイプライン**               | 3.72 ms       | 125.0      | 31.2       | -         | 2.22 MB              |
| **改善効果**                      | **-30.3%**    | **-33.3%** | **-50.1%** | **-100%** | **-22.6%**           |

> [!NOTE]
> Gen0 / Gen1 / Gen2 カラムは、1,000 操作あたりの GC 発生回数を示しています。最も重い Gen2 GC は、マイクロ／マクロ両方のベンチマークで完全に排除（0回）されています。MAUI, Avalonia, WinUI 向けのベンチマークでも、概ね 20〜30% のスループット向上が確認されています。
