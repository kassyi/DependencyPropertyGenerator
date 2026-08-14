# 02. パイプラインとアーキテクチャ (Pipeline & Architecture)

[English](../en/02_pipeline_architecture.md) | [日本語](./02_pipeline_architecture.md) | [目次 (Intro)](./intro.md)

## Ⅰ. インクリメンタル・パイプライン構造

Roslyn Incremental Source Generator (ISG) は、入力である構文ツリーから最終的なソースコード出力までを、LINQのようなパイプラインで処理します。本プロジェクトでは `Kassyi.Generators.Extensions` のヘルパーを利用してパイプラインを簡潔かつ超低アロケーションに構築しています。

### パイプライン・フロー (Mermaid シーケンス図)

```mermaid
sequenceDiagram
    autonumber
    participant Compiler as Roslyn Compiler
    participant SP as SyntaxProvider (ISG)
    participant Prepare as PrepareData (抽出)
    participant Model as DTO (ClassData/DPData)
    participant Source as Sources.* (生成)
    
    Compiler->>SP: 構文変更の通知
    SP->>SP: ForAttributeWithMetadataName...<br/>(属性付きクラスをフィルタリング)
    SP->>SP: Combine(Framework, Version)
    SP->>Prepare: Select(PrepareData)
    Note over Prepare: ISymbolやSyntaxNodeから<br/>プリミティブなデータのみを抽出<br/>(NamedArguments辞書キャッシュ等)
    Prepare-->>Model: (ClassData, DependencyPropertyData) を構築
    SP->>SP: WhereNotNull()
    Note over SP: 前回のコンパイル時と等価(Equals)なら<br/>ここで処理を打ち切りキャッシュを使う
    SP->>Source: Select(Generate)
    Source-->>SP: 生成されたC#ソース文字列
    SP->>Compiler: AddSource()
```

### パイプラインの各フェーズ

1. **構文のフィルタリング (`ForAttributeWithMetadataName`)**
   - Roslyn 4.3.0+ の機能。指定した属性 (`[DependencyProperty]`, `[AttachedDependencyProperty]`, `[RoutedEvent]`, `[WeakEvent]` など) が付与されているクラス・レコードの構文のみを抽出します。
2. **データの抽出 (`PrepareData` / `DependencyPropertyDataBuilder`)**
   - 抽出された `AttributeData` や `INamedTypeSymbol` から、生成に必要なすべてのメタデータ（型名、デフォルト値、フラグ類）を抽出・構造化します。
   - `PrepareData.cs` および `DependencyPropertyDataBuilder` が集約して担当しています。
   - 属性の NamedArguments 解決では辞書キャッシュ化および重複構文ルックアップの排除を行い、抽出パフォーマンスを最大化しています。
3. **等価性評価とキャッシュ**
   - RoslynのISG基盤は、`Select` の出力が前回と同一（`Equals` が `true`）である場合、後続のパイプライン処理をスキップ（キャッシュ利用）します。
4. **ソースコードの生成 (`Sources.*`)**
   - キャッシュミスがあった場合のみ呼び出され、DTOをもとに最終的な `.g.cs` コード文字列を生成します。
   - `SourceWriter` のスコープ管理機構により、ゼロアロケーションかつ安全に出力されます。

---

## Ⅱ. モデルの等価性（キャッシュ）戦略

インクリメンタルジェネレーターにおいて**最も重要なパフォーマンス指標はキャッシュヒット率**です。
本プロジェクトでは、ジェネレーター内のデータモデル (`DependencyPropertyData`, `ClassData`, `EventData`, および各種サブモデル) において以下の設計を徹底しています。

### `readonly record struct` による値の比較
すべてのモデルは `readonly record struct` として定義されています。これにより、C#のコンパイラがすべてのプロパティに対する `Equals()` メソッドと `GetHashCode()` を自動生成し、プロパティの「値」に基づく等価性比較が行われます。

### コレクションの等価性担保 (`EquatableArray<T>`)
Roslynパイプラインにおいて、標準の配列 `T[]` や `ImmutableArray<T>` は「参照」で比較されてしまうため、中身が同じでも `Equals` が `false` になりキャッシュが無効化されてしまいます。

これを防ぐため、コレクションデータ（`BindEvents` など）は必ず `EquatableArray<T>` でラップしています。
- **実装箇所**: `BindEvents: bindEvents.AsEquatableArray()`
- **効果**: 配列の要素同士を深く比較 (SequenceEqual) し、要素が同じであれば等価とみなすことで、余計なコード再生成を抑制しています。
