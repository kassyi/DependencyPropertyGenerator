# 04. インクリメンタル・ジェネレーターの計算量モデル (Complexity Model)

[English](../en/04_mathematical_model.md) | [日本語](./04_mathematical_model.md) | [目次 (Intro)](./intro.md)

Roslyn Incremental Source Generator のパフォーマンスを維持するためには、開発者は「どの操作がどれだけの計算量（アロケーションコストと処理時間）を発生させるか」を理解しておく必要があります。

ここでは、本プロジェクトのジェネレーターアーキテクチャに基づく**最悪計算量（Worst-Case Complexity）**と、それを回避するための設計意図を解説します。

## Ⅰ. 計算量の基本モデル

ジェネレーターの処理は、大きく分けて2つのフェーズからなります。

1. **`PrepareData` (データ抽出フェーズ)**: 属性やクラス構造からデータを抽出する
2. **`SourceWriter` (ソース生成フェーズ)**: 抽出したデータからC#コードを文字列として生成する

コンパイル対象のソースファイル数を $S$、各ファイルに含まれる対象プロパティ（属性）の平均数を $P$、属性に指定されている NamedArguments (名前付き引数) の最大数を $N$ とします。

### 1. `PrepareData` の計算量
属性の解析において、指定された `NamedArguments` を一つずつ走査して設定値を読み取ります。
例えば、[`PrepareData.cs`](../../src/Kassyi.Generators.DependencyProperty/PrepareData.cs) の `GetNamedArgumentExpression` メソッドでは、LINQによるアロケーションを避けるため以下のように意図的な `foreach` ループを使用しています。

```csharp
// [WHY] Avoid LINQ FirstOrDefault(predicate) to eliminate delegate allocations during syntax tree analysis.
foreach (var argument in attributeSyntax.ArgumentList.Arguments)
{
    var nameEquals = argument.NameEquals?.ToFullString().Trim('=', ' ', '\t', '\r', '\n');
    if (nameEquals == name)
    {
        return argument.Expression.ToFullString();
    }
}
```

引数の数 $N$ に対して、設定項目（$M$ 個）ごとにこのループ処理が発生するため、時間計算量は $O(M \times N)$、定数項を無視して **$O(N)$** となります。
本プロジェクトでは、抽出結果を `readonly record struct` と `EquatableArray<T>` という**完全な値型DTO**に詰め込みます。これにより、このフェーズでのメモリ割り当て（アロケーションコスト）を最小限に抑えています。

### 2. `SourceWriter` の計算量
生成されるソースコードの行数（または文字数）を $K$ とします。
文字列の連結処理ですが、`SourceWriter` (内部的に `StringBuilder` をラップ) を用いており、メモリの再確保を抑えつつ線形に書き出すため、時間計算量は **$O(K)$** となります。
また、`using var _ = writer.ClassScope(@class);` などのゼロアロケーションスコープを活用しているため、ガベージコレクション(GC)への負荷も $O(1)$（ほぼゼロ）に抑えられています。

---

## Ⅱ. incremental cache による最適化と「最悪計算量」

Incremental Generator は、過去のコンパイル結果をキャッシュし、変更があった部分のみを再計算します。
[`GeneratorHelper.cs`](../../src/Kassyi.Generators.DependencyProperty/Generators/GeneratorHelper.cs) の `RegisterAttributeGenerator` において、パイプラインは以下のように構築されています。

```csharp
combinedProvider
    .Combine(framework)
    .Combine(version)
    .SelectAndReportExceptions(prepareData, context, id) // O(N)
    .WhereNotNull()
    .SelectAndReportExceptions(getSourceCode, context, id) // O(K)
    .AddSource(context);
```

キャッシュヒット率を $H$ ($0 \le H \le 1$) とすると、このパイプラインを流れる実際の全体計算量 $T$ は以下のように近似できます。

$$ T \approx (1 - H) \times O(S \times P \times (N + K)) $$

理想的な状態（タイピングによる局所的な変更のみ）では、ほぼすべてのファイルで $H \approx 1$ となり、$T \approx 0$ となります。

### 最悪ケース (Worst-Case Scenario)

開発者が直面しうる「最も重い（最悪の）操作」とは何でしょうか？
それは、**キャッシュヒット率 $H = 0$ になるような広範囲な変更**が行われた場合です。

**シナリオ:**
ある共通クラス（Baseクラスなど）で定義されている `[DependencyProperty]` の名前や型、あるいは属性のパラメータ（例: `DefaultValue`）を書き換えたとします。

このとき何が起きるか：
1. そのクラスに依存している、あるいはファイル全体に影響が及ぶとRoslynが判断します。
2. すべての対象ファイル $S$ においてキャッシュが無効化（$H = 0$）されます。
3. 全てのプロパティ $S \times P$ に対して、再度 $O(N)$ の解析と $O(K)$ のソース生成が走ります。

**最悪計算量:** **$O(S \times P \times (N + K))$**

大規模なソリューション（$S$ が数千）において、この最悪ケースが発生すると、IDEが数秒間フリーズする可能性があります。

---

## Ⅲ. パフォーマンス低下を防ぐためのアーキテクチャ上の工夫

この最悪計算量がタイピング（1文字の変更）のたびに発生するのを防ぐため、本ジェネレーターは以下の厳格なルールで設計されています。

1. **DTOへの `ISymbol` や `SyntaxNode` の混入禁止**
   - Roslynの参照オブジェクトをデータモデルに含めると、1キーストロークごとにインスタンスが変わり、`Equals` が `false` になります。
   - つまり、関係のないファイルのキャッシュまで無効化され、常に最悪計算量 $O(S \times P)$ が発生する「IDEフリーズ現象」を引き起こします。
2. **`IEquatable` の完全な実装 (`EquatableArray<T>`)**
   - リストデータも値ベースの比較を行うことで、「意味的に同じならキャッシュを使う」ことを保証し、$H \approx 1$ を維持しています。
3. **`SourceWriter` によるアロケーションフリーなコード生成**
   - 仮に最悪ケース（全ファイル再生成）が発生した場合でも、`StringBuilder` プーリングと `ref struct` (ClassScope等) を駆使することで、GCスパイクによる二次的なパフォーマンス低下（フリーズの延長）を防ぎます。
