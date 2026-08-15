# 06. フレームワーク別生成マッピング仕様

[English](../en/06_framework_strategies.md) | [日本語](./06_framework_strategies.md) | [目次 (Intro)](./intro.md)

DependencyPropertyGenerator は、単一の `[DependencyProperty]` 属性から、ターゲットとするUIフレームワーク（WPF、UWP、WinUI、Uno、Avalonia、MAUI）のそれぞれに最適化されたボイラープレートコードを動的に生成します。
このドキュメントは、特定のフレームワーク固有のバグを修正したり新機能を追加したりする際に、APIのマッピングがどのように行われているかを理解するための地図（Ground Truth）として機能します。

各プラットフォーム間の差異は、内部的に `Sources/Strategies/` ディレクトリに配置された `IFrameworkGeneratorStrategy` の実装クラス群によって完全に吸収されています。

---

## Ⅰ. フレームワークごとのプロパティ登録API

### WPF (WpfFrameworkGenerator)
WPFではプロパティの基盤として `System.Windows.DependencyProperty` や `DependencyPropertyKey` を使用します。
プロパティの登録には `DependencyProperty.Register` メソッドを呼び出し、添付プロパティの場合は `RegisterAttached` を使用します。また、読み取り専用のプロパティを生成する際は `RegisterReadOnly` や `RegisterAttachedReadOnly` が使われます。
メタデータの管理には `System.Windows.FrameworkPropertyMetadata` または `PropertyMetadata` が用いられ、コールバック処理は `PropertyChangedCallback`、`CoerceValueCallback`、`ValidateValueCallback` といった専用のデリゲートを通じて結線されます。
実装上とくに意識すべき点として、WPFのメタデータは `AffectsMeasure` や `BindsTwoWayByDefault` といったレイアウト制御やデータバインディング向けの非常に豊富なフラグを持っています。そのため、ジェネレーター内部でも `FrameworkMetadataData` のフィールドを最も広く活用してコードを生成します。

### Avalonia (AvaloniaFrameworkGenerator)
Avaloniaでは `Avalonia.AvaloniaProperty` を基底とし、通常は `StyledProperty<T>`、`AttachedProperty<T>`、あるいは `DirectProperty<T>` としてプロパティを定義します。
登録時には `AvaloniaProperty.Register` や `RegisterAttached` メソッドを使用しますが、フィールドベースの高速なプロパティである `DirectProperty` を生成する場合（`IsDirect` フラグが有効な場合）は、専用のジェネリックメソッドである `RegisterDirect` を呼び出すコードを出力します。
メタデータは登録メソッドの引数として直接渡されるか、Avalonia固有のメタデータ機能を用いて管理されます。また、変更通知のコールバックは `AvaloniaPropertyChanged` などのObservableやイベントベースの購読モデルを通じて実現されます。

### MAUI (MauiFrameworkGenerator)
MAUIは独自の型システムを採用しており、`DependencyProperty` ではなく `Microsoft.Maui.Controls.BindableProperty` や `BindablePropertyKey` を使用します。
プロパティの登録は `BindableProperty.Create` や `CreateAttached` メソッドで行い、読み取り専用の場合は `CreateReadOnly` や `CreateAttachedReadOnly` を使用します。メタデータは専用のクラスではなく、APIの引数としてフラットに渡される形式をとります。
コールバックは `BindingPropertyChangedDelegate`、`CoerceValueDelegate`、`ValidateValueDelegate` といったデリゲート型にマッピングされます。

### UWP / WinUI / Uno (UwpFrameworkGenerator)
これらのプラットフォームでは、UWPとUnoが `Windows.UI.Xaml.DependencyProperty` を、WinUI 3が `Microsoft.UI.Xaml.DependencyProperty` を使用します。
登録メソッドは `DependencyProperty.Register` と `RegisterAttached` のみで構成されており、メタデータには `PropertyMetadata` が用いられます。コールバックは `PropertyChangedCallback` のみを提供しています。
注意点として、これらのプラットフォームは Coerce (型強制) や Validate (検証) のコールバックをネイティブAPIとして備えていません。そのため、ジェネレーターはプロパティの getter や setter、あるいは PropertyChanged イベントの内部で手動で値をクランプ（補正）するようなフォールバック実装を生成して振る舞いを模倣します。

---

## Ⅱ. ジェネレーター・ストラテジーの拡張方針

新しいUIフレームワークへの対応（例えば、将来のAvaloniaのメジャーアップデートによる破壊的変更など）や特定のフレームワークに起因するバグを修正する際は、以下の原則に従って実装を拡張してください。

最初の原則は、共通のDTOモデル（`DependencyPropertyData` など）を直接変更しないことです。フレームワーク間の差異はすべて `Sources/Strategies/` 以下の該当ジェネレータークラス（例: `XxxFrameworkGenerator.cs`）のメソッドをオーバーライドすることで吸収します。

次に、メソッド抽出を活用してフレームワークごとのAPIシグネチャの違いを吸収します。例えば `GenerateRegisterMethodArguments` メソッドは、`Register` メソッドに渡す引数の文字列を構築する役割を持っています。これを利用して、引数の順序や型の違いを柔軟に解決できます。

最後に、ベンチマークスコアとゼロアロケーション特性を維持するため、文字列生成処理（`SourceWriter`）の内部でLINQを使用したり、不要な `string.Join` を呼び出したりすることは避けてください。
