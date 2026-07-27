# 03. 生成戦略と最適化 (Generation & Optimization)

## Ⅰ. インターフェース仕様 (生成されるコードの構造)

ジェネレーターは、抽出したDTO（`DependencyPropertyData`等）をもとに、WPF/MAUIなどのフレームワークに対応したC#コードをテキストとして出力します。
生成されるコードは、ユーザーが記述したクラスを拡張する形（`partial` クラス）で提供されます。

### 境界とコントラクト
- **入力 (User Code)**: `partial` 修飾子が付けられたクラス宣言と `[DependencyProperty]` 属性。オプションとして `partial void On...Changed()` の宣言。
- **出力 (Generated Code)**: 
  - 依存関係プロパティの静的フィールド (`...Property`)
  - CLR プロパティラッパー (`get` / `set`)
  - プロパティ変更時のコールバックメソッドの実装 (`propertyChangedCallback`)
  - XMLドキュメントコメント

### デフォルト値式 (`DefaultValueExpression`) の自動補完ルール
- `[DependencyProperty<T>("Name", DefaultValueExpression = "...")]` において、`DefaultValueExpression` に指定された文字列が `new(...)` または `new (...)`（C# 9.0+ の target-typed `new` 構文）で始まる場合、ジェネレーター抽出処理（`PrepareData`）時にプロパティの型 `T` の完全修飾型名（`global::...`）へと自動的に置換・展開します。
  - 入力例: `[DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]`
  - 展開後: `new global::MyNamespace.MyProfile(1.5, 48.0)`
- これにより、他の名前空間にある型をデフォルト値としてインスタンス化する際にも、属性の文字列内で手動で冗長な完全修飾名を書く必要がなくなり、コードの保守性と視認性が向上します。

### コールバックメソッド (`OnChanged` / `OnChanging`) の解決規則と制約

#### 1. サポートされるシグネチャ（最大3引数）
プロパティ変更時にフレームワークから渡される情報は最大3つ（`sender`, `oldValue`, `newValue` または `EventArgs`）のため、以下のシグネチャのみサポートされます。

```csharp
// ✅ 0引数
partial void OnTextChanged();

// ✅ 1引数 (新値 または EventArgs)
partial void OnTextChanged(string newValue);
partial void OnTextChanged(DependencyPropertyChangedEventArgs e);

// ✅ 2引数 (旧値・新値 / sender・新値 / sender・EventArgs)
partial void OnTextChanged(string oldValue, string newValue);
partial void OnTextChanged(MyControl sender, string newValue);
partial void OnTextChanged(MyControl sender, DependencyPropertyChangedEventArgs e);

// ✅ 3引数 (sender・旧値・新値)
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
    // 💡 サイレント無視を防ぐため、重要な処理は 方式 A (OnChanged = nameof(...)) での明示指定を推奨。
    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
}
```

---

## Ⅱ. パフォーマンス最適化ルール (Dos & Don'ts)

インクリメンタル・ソースジェネレーターのパフォーマンス（特にVisual StudioなどのIDE上でのタイピング時の応答速度）を維持・向上させるため、以下のルールを厳守してください。

### 🟢 Dos (推奨事項)
- **`ForAttributeWithMetadataName` を使う**: 古い `ISyntaxReceiver` を使わず、属性ベースで構文をフィルタリングするRoslyn 4.3+のAPIを使用します。これにより、対象外のコード変更に対するジェネレーターの起動を劇的に減らすことができます。
- **データ抽出は早期に行う**: `SyntaxNode` や `ISymbol` を受け取ったら、直ちに `string`, `bool`, `Framework` (enum) などの単純な型に変換（マッピング）してDTOに格納します。
- **`EquatableArray<T>` を使う**: コレクションを扱う場合は、必ず要素単位の比較が行われるようにラップします。

### 🔴 Don'ts (禁止事項)
- **❌ `ISymbol` や `SyntaxNode` をDTOに含めない**: 
  - これらを保持したまま `Select` を抜けると、ジェネレーター基盤が前のコンパイル状態をGC（ガベージコレクション）できず、**深刻なメモリリーク**を引き起こします。
  - さらに、コンパイルのたびに `ISymbol` の参照が変わるため、`Equals` が常に `false` になり、キャッシュが全く機能しなくなります。
- **❌ DTO内で `List<T>` や `T[]` を直接使わない**:
  - 参照比較となるため、中身が同じでも再生成が走ります（前述の `EquatableArray` を使用すること）。
- **❌ `Select` 内で重い処理（I/Oやネットワーク）をしない**:
  - インクリメンタルジェネレーターはタイピングのたびに頻繁に呼ばれるため、処理は純粋な関数であるべきです。
- **❌ 文字列生成時の中間アロケーション（無駄なヒープ割り当て）**:
  - `string.Split()` や `string.Join()`、不要な `List<string>` の生成などは、GCスパイク（IDEのプチフリーズ）の原因となるため避けてください。
  - 代わりに `StringBuilder` やインデックススキャン、可能であれば `stackalloc Span<char>` などを活用してGC負荷を極限までゼロに近づけます。

---

## Ⅲ. プロファイリング手法

ジェネレーターのパフォーマンス・ボトルネックを調査する際は、以下の手法を用います。

1. **ビルドログ解析 (`.binlog`)**
   ```bash
   dotnet build -c Release -bl:msbuild.binlog
   ```
   生成された `msbuild.binlog` を [MSBuild Structured Log Viewer](https://msbuildlog.com/) で開き、タスクごとの所要時間や、Source Generator がどの程度時間を消費しているかを確認します。

2. **ジェネレーターのベンチマーク**
   BenchmarkDotNet を使用し、`CSharpGeneratorDriver` に擬似的なソースコードを流し込んで、実行時間とメモリのアロケーション量 (Gen0/Gen1/Gen2, Allocated Bytes) を計測します。
