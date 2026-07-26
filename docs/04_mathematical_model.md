# 04. インクリメンタル・ジェネレーターの数理モデル (Mathematical Model)

Roslyn Incremental Source Generator の振る舞いとパフォーマンス最適化の理論的背景は、**集合論と純粋関数**を用いた数式モデルとしてエレガントに表現できます。
ジェネレーターのパフォーマンス低下（キャッシュミス）がなぜ起こるのかを論理的に理解するためのモデルです。

## Ⅰ. パイプラインの関数モデル

ジェネレーターの処理全体は、状態を持たない（ステートレスな）純粋関数の合成として定義されます。

### 1. フィルタリング関数 $F$ (SyntaxProvider)
コンパイル対象のすべての構文ツリーの集合を $S$、特定の属性（例: `[DependencyProperty]`）を $A$ とします。
フィルタリング関数 $F$ は、条件を満たす構文の集合 $S_{filtered}$ を抽出します。

$$ S_{filtered} = \{ s \in S \mid \text{HasAttribute}(s, A) \} $$

### 2. 抽出関数 $E$ (PrepareData)
Roslynのセマンティックモデル（コンパイラの状態）を $C$ とします。
抽出関数 $E$ は、構文 $s$ とコンパイラ状態 $C$ から、**完全に状態から切り離された純粋なデータモデル（DTO）の集合 $D$** を射影（プロジェクション）します。

$$ D = \{ E(s, C) \mid s \in S_{filtered} \} $$

この $D$ こそが `ClassData` や `DependencyPropertyData` (readonly record struct) であり、後続のキャッシュ戦略の要となります。

### 3. 生成関数 $G$ (GetSourceCode)
純粋なデータモデル $d \in D$ を受け取り、最終的なC#ソースコード文字列 $Code$ を出力します。

$$ Code = G(d) $$

---

## Ⅱ. 等価性（キャッシュ）と計算量のモデル

Incremental Generatorの最大の強みは、時間 $t$ におけるコンパイルと、一つ前の時間 $t-1$ におけるコンパイルの差分のみを評価（$G$ を実行）することです。

### キャッシュ判定式
時間 $t$ におけるデータモデルの集合を $D_t$ とします。
Roslyn基盤は、前回の状態 $D_{t-1}$ と比較を行い、**同値関係 $\equiv$ （すなわち `IEquatable<T>.Equals`）** を用いて差分集合 $\Delta D$ を計算します。

$$ \Delta D = D_t \setminus (D_t \cap D_{t-1}) $$

ソースコードの生成処理 $G$ は、この差分集合 $\Delta D$ に対してのみ実行されます。

$$ Output_t = \{ G(d) \mid d \in \Delta D \} \cup CachedOutput_{t-1} $$

**もし $D_t \equiv D_{t-1}$ であれば、$\Delta D = \emptyset$ となり、生成フェーズの計算コストはゼロになります。**

---

## Ⅲ. パフォーマンス最適化の数学的証明

ジェネレーターの合計実行時間 $T_{total}$ は、以下のようにモデル化できます。

$$ T_{total} = T_{filter} + T_{extract} + T_{compare} + (1 - H) \cdot T_{generate} $$

- $T_{filter}$: 構文抽出にかかる時間（Roslyn側で最適化済み）
- $T_{extract}$: `PrepareData` の実行時間
- $T_{compare}$: $D_t$ と $D_{t-1}$ の比較（`Equals`）にかかる時間
- $H$: **キャッシュヒット率** ($0 \le H \le 1$)
- $T_{generate}$: `GetSourceCode` の実行と文字列結合にかかる時間

### アンチパターン：DTOに `ISymbol` を含めてはいけない理由
もし $D$ （DTO）の中に `ISymbol` や `SyntaxNode` などのRoslynオブジェクトを含めてしまうと何が起きるでしょうか。

タイピングなどによりコンパイルが発生するたび、Roslynは新しい $C$（コンパイル状態）を生成し、すべての `ISymbol` インスタンスは再生成されます。
つまり、論理的な意味が同じであっても、メモリ上の参照が変わるため、常に $D_t \not\equiv D_{t-1}$ と判定されます。

数式で表すと、**常に差分集合 $\Delta D = D_t$** となり、**キャッシュヒット率 $H = 0$** となります。

$$ T_{total} \approx T_{filter} + T_{extract} + T_{compare} + 1.0 \cdot T_{generate} $$

これにより、キーストロークのたびに全プロパティのコード再生成 $T_{generate}$ が走り、IDEがフリーズする（パフォーマンスが著しく低下する）原因となります。

### 最適解：純粋な値ベースの等価性
$D$ を `readonly record struct` と `EquatableArray<T>` などの「純粋な値の集合」に射影（プロジェクション）することで、タイピングによる影響を受けていない無関係なプロパティの $D$ においては $D_t \equiv D_{t-1}$ を保証できます。

これにより、関係ない部分のコードにおいては $H \approx 1$ （ほぼ100%キャッシュヒット）となり、$T_{generate}$ の項が消滅し、最速のレスポンスを実現できるのです。
