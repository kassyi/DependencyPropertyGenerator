# 08. 診断エラーコード一覧 (Diagnostics Reference)

[English](../en/08_diagnostics_reference.md) | [日本語](./08_diagnostics_reference.md)
前へ: [⬅ 07. テスト仕様書](./07_test_specification.md) | [目次 (Intro)](./intro.md)

本ドキュメントは、DependencyPropertyGenerator がソースコード解析中に発行する診断エラー（Diagnostics）の一覧と、そのトラブルシューティングガイドです。
Microsoft Docs のように、各エラーの発生原因と具体的な解決策（Before / After のコード例）を記載しています。

---

## 診断ID クイックリファレンス

| 診断ID                                                            | 重大度 | タイトル                       | 概要                                                                       |
| :---------------------------------------------------------------- | :----- | :----------------------------- | :------------------------------------------------------------------------- |
| [`DPG0000`](#dpg0000-framework-is-not-recognized)                 | Error  | Framework is not recognized    | 対象の UI フレームワークを自動判別できません。                             |
| [`DPG0001`](#dpg0001-onchanged-method-not-found-or-unsupported)   | Error  | OnChanged Method Not Found     | 指定されたコールバックメソッドが見つからないか、シグネチャが不正です。     |
| [`DPG0002`](#dpg0002-invalid-type-modifier-file-scoped)           | Error  | Invalid Type Modifier          | `file` スコープ修飾子が付与されたクラスには使用できません。                |
| [`DPG0003`](#dpg0003-invalid-property-type-ref-struct)            | Error  | Invalid Property Type          | `ref struct` 型は依存関係プロパティとして使用できません。                  |
| [`DPG0004`](#dpg0004-reference-type-default-value-sharing)        | Error  | Reference Type Sharing         | 参照型のデフォルト値が全インスタンスで共有される危険があります。           |
| [`DPG0005`](#dpg0005-invalid-callback-signature-overridemetadata) | Error  | Invalid Callback Signature     | 旧値を提供しないプラットフォームで旧値を受け取るシグネチャを指定しました。 |
| [`DPG0007`](#dpg0007-unsupported-callback-signature)              | Error  | Unsupported Callback Signature | 命名規約に一致したメソッドのシグネチャが不正です。                         |
| [`DPG0008`](#dpg0008-invalid-default-value-expression)            | Error  | Invalid Default Expression     | `DefaultValueExpression` の C# 構文がパースできません。                    |
| `DPG0009`                                                         | Info   | Duplicate Attribute Helper     | 内部属性ヘルパー型の重複検知（自動抑制されるため対応不要）。               |
| `DPG9999`                                                         | Error  | Unhandled Exception            | ジェネレーターの予期せぬ内部エラー。                                       |

---

## エラー詳細と解決策

### DPG0000: Framework is not recognized

対象となる UI フレームワーク（WPF, WinUI, Uno, Avalonia, MAUI）が現在のプロジェクトから自動検出できませんでした。

❌ **発生する原因 (Before):**
プロジェクト参照（`.csproj`）に UI フレームワークのパッケージ（例: `Avalonia` や `Microsoft.WindowsAppSDK`）が含まれていない、または純粋なクラスライブラリで動作させようとしています。

✅ **解決策 (After):**
対応する NuGet パッケージをインストールするか、カスタムライブラリの場合は明示的にコンパイラ定数を定義して対象フレームワークを指定してください。

```xml
<!-- .csproj 内に明示的な定数を追加してフレームワークを指定する例 -->
<PropertyGroup>
    <DefineConstants>$(DefineConstants);HAS_WPF</DefineConstants>
</PropertyGroup>
```

---

### DPG0001: OnChanged Method Not Found or Unsupported

属性の `OnChanged` 引数で明示的に指定したメソッドがクラス内に存在しないか、引数シグネチャがサポート対象外です。

❌ **間違ったコード (Before):**

```csharp
[DependencyProperty<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyControl : UserControl
{
    // エラー: 第1引数に自身のクラス型 (MyControl) を受け取っていない
    private void OnCountChanged(int oldValue, int newValue)
    {
    }
}
```

✅ **正しいコード (After):**

```csharp
[DependencyProperty<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyControl : UserControl
{
    // 解決策: 第1引数にジェネレーターが自動結線するための「自身の型」を指定する
    private void OnCountChanged(MyControl sender, int oldValue, int newValue)
    {
    }
}
```

---

### DPG0002: Invalid Type Modifier (File scoped)

C# 11 で導入された `file` スコープ修飾子が指定されたクラスにジェネレーターを適用しようとしました。Roslyn Source Generator は `file` スコープ型へのコード追加をサポートしていません。

❌ **間違ったコード (Before):**

```csharp
[DependencyProperty<string>("Text")]
file partial class LocalControl : UserControl // エラー: file スコープ
{
}
```

✅ **正しいコード (After):**

```csharp
[DependencyProperty<string>("Text")]
internal partial class LocalControl : UserControl // 解決策: internal または public に変更
{
}
```

---

### DPG0003: Invalid Property Type (Ref struct)

`ReadOnlySpan<T>` などの `ref struct` 型はマネージドヒープ上に置くことができないため、依存関係プロパティ（内部的にボクシングやオブジェクトディクショナリを使用する）の型としては使用できません。

❌ **間違ったコード (Before):**

```csharp
// エラー: ReadOnlySpan<char> は ref struct のためボクシング不可
[DependencyProperty<ReadOnlySpan<char>>("Buffer")]
public partial class MyControl : UserControl
{
}
```

✅ **正しいコード (After):**

```csharp
// 解決策: 通常の struct または配列、Memory<T> などを利用する
[DependencyProperty<ReadOnlyMemory<char>>("Buffer")]
public partial class MyControl : UserControl
{
}
```

---

### DPG0004: Reference Type Default Value Sharing

参照型（`class` や `List<T>` など）のインスタンスを `DefaultValue` に直接指定しています。
WPF などのフレームワークでは、参照型のデフォルト値は **すべてのコントロールインスタンス間で同じオブジェクト（参照）が共有される** という致命的なメモリリーク・状態共有バグを引き起こします。ジェネレーターはこれを未然に防ぎます。

❌ **間違ったコード (Before):**

```csharp
// エラー: リストのインスタンスが全 MyControl で1つだけ共有されてしまう
[DependencyProperty<List<string>>("Items", DefaultValueExpression = "new()")]
public partial class MyControl : UserControl
{
}
```

✅ **正しいコード (After):**

```csharp
// 解決策: CreateDefaultValueCallback = true を設定する
[DependencyProperty<List<string>>("Items", CreateDefaultValueCallback = true)]
public partial class MyControl : UserControl
{
    // ジェネレーターが呼び出す部分メソッドを通じて、インスタンスごとに個別の new を行う
    static partial void GetItemsDefaultValue(ref List<string> defaultValue)
    {
        defaultValue = new List<string>();
    }
}
```

---

### DPG0005: Invalid Callback Signature (OverrideMetadata)

WPF 以外のプラットフォーム（UWP, WinUI, Uno, MAUI など）では、フレームワークの仕様上、親クラスのプロパティ設定を上書き（`OverrideMetadata`）する際に **「変更前の古い値（`oldValue`）」を取得する仕組みがありません**。
古い値が取得できない環境にもかかわらず、コールバックメソッドの引数で `oldValue` を受け取ろうとしたためエラーになっています。

❌ **間違ったコード (Before):**

```csharp
// エラー: WinUI/Uno などの環境ではフレームワークから古い値をもらえない
[OverrideMetadata<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyWinUIControl : UserControl
{
    private void OnCountChanged(MyWinUIControl sender, int oldValue, int newValue) { }
}
```

✅ **正しいコード (After):**

```csharp
[OverrideMetadata<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyWinUIControl : UserControl
{
    // 解決策: 新しい値 (newValue) のみを受け取るようにする
    private void OnCountChanged(MyWinUIControl sender, int newValue) { }
}
```

---

### DPG0007: Unsupported Callback Signature

ジェネレーターは `partial void On{PropertyName}Changed(...)` という命名規約のメソッドを見つけると自動呼出を試みますが、そのメソッドの引数シグネチャが間違っています。（例: `DependencyObject` を指定している等）

❌ **間違ったコード (Before):**

```csharp
[DependencyProperty<string>("Text")]
public partial class MyControl : UserControl
{
    // エラー: 汎用的な DependencyObject と RoutedEventArgs を指定している
    partial void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
}
```

✅ **正しいコード (After):**

```csharp
[DependencyProperty<string>("Text")]
public partial class MyControl : UserControl
{
    // 解決策: 自クラス型と、強く型付けされた値の型 (string) を指定する
    partial void OnTextChanged(string? oldValue, string? newValue);

    // または自クラスを受け取るシグネチャ
    // partial void OnTextChanged(MyControl sender, string? oldValue, string? newValue);
}
```

---

### DPG0008: Invalid Default Value Expression

`DefaultValueExpression` に渡された C# 式の文字列が文法的に間違っており、Roslyn パーサーが解釈できません。

❌ **間違ったコード (Before):**

```csharp
// エラー: カッコが閉じていない、クォーテーションのミスなど
[DependencyProperty<string>("Text", DefaultValueExpression = "new(123, ")]
public partial class MyControl : UserControl
{
}
```

✅ **正しいコード (After):**

```csharp
// 解決策: 正しい C# の文法・式としてパースできる文字列を渡す
[DependencyProperty<string>("Text", DefaultValueExpression = "new(123, 456)")]
public partial class MyControl : UserControl
{
}
```

---

前へ: [⬅ 07. テスト仕様書](./07_test_specification.md) | [目次 (Intro)](./intro.md)
