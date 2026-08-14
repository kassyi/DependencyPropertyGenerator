# 01. 基盤とドメイン (Foundation & Domain)

## Ⅰ. 目的と設計思想 (Purpose & Philosophy)

**DependencyPropertyGenerator** の主な目的は、WPF, UWP, WinUI, Uno, Avalonia, MAUI といった複数の .NET UI フレームワーク向けに、**ボイラープレート（定型コード）が多くなりがちな依存関係プロパティ (DependencyProperty) やルーティングイベント (RoutedEvent)、弱イベント (WeakEvent) の宣言コードを自動生成** することです。

### 技術的制約と方針
- **Roslyn Incremental Source Generator**: `.NET Standard 2.0` をターゲットとし、インクリメンタルな変更（タイピング中の差分評価）に対して高速に動作することが求められる。
- **ターゲットUIフレームワークの吸収**: 複数のUIフレームワーク（`Framework` enum等で管理）ごとのAPI差異をジェネレーター内部で吸収し、単一の属性 (`[DependencyProperty]`) から適切なコードを生成する。
- **`partial` クラスとメソッドの活用**: 生成されるコードは `partial` クラスとして追加され、イベントフック用の `partial void On...Changed(...)` メソッドを提供する。

---

## Ⅱ. ユビキタス言語辞書 (Glossary)

ジェネレーターのコードベース全体で統一されている用語定義です。

| 日本語名 | 英語名 (Code) | 説明 |
|---|---|---|
| UIフレームワーク | `Framework` | WPF, Uno, MAUIなどの対象プラットフォームを識別する列挙型 |
| 依存関係プロパティ | `DependencyProperty` | UIコントロールが状態を保持するための拡張プロパティ機構 |
| 添付プロパティ | `AttachedDependencyProperty` | 子要素から親要素などに値を設定するためのプロパティ |
| クラスデータ | `ClassData` | 属性が付与された対象クラス（オーナー）のメタデータ |
| プロパティデータ | `DependencyPropertyData` | 生成するプロパティ固有のメタデータ（型、デフォルト値、フラグなど） |

---

## Ⅲ. ドメイン & データ構造仕様 (Domain & Data Models)

Roslynの `SyntaxNode` や `ISymbol` から情報を抽出し、インクリメンタル・パイプラインを流れる純粋なデータモデル (DTO) です。これらは **キャッシュ効率を高めるため、すべて `readonly record struct` で定義され、`IEquatable<T>` による厳密な等価性比較をサポート** します。

### メインデータモデル (Mermaid クラス図)

```mermaid
classDiagram
    class ClassData {
        <<readonly record struct>>
        +string Namespace
        +string Name
        +string FullName
        +string Type
        +string Modifiers
        +string Version
        +bool IsStatic
        +Framework Framework
        +EquatableArray~string~ Methods
    }

    class DependencyPropertyData {
        <<readonly record struct>>
        +string Name
        +string Type
        +string ShortType
        +bool IsValueType
        +bool IsSpecialType
        +string DefaultValue
        +bool IsAttached
        +bool IsAddOwner
        +Framework Framework
        +string Description
        +EquatableArray~string~ BindEvents
        +string OnChanged
        +bool AffectsMeasure
        +bool AffectsArrange
        ...その他各種メタデータフラグ
    }
    
    class EventData {
        <<readonly record struct>>
        +string Name
        +string Strategy
        +string Type
        +bool IsAttached
        +string Description
        ...その他メタデータ
    }
```

### データ構造の設計方針
- **プリミティブ型への早期変換**: Roslynの `INamedTypeSymbol` や `IPropertySymbol` をそのまま保持するとメモリリークを引き起こし、ジェネレーターのキャッシュが無効化されます。そのため、抽出フェーズで必ず `string` や `bool` などのプリミティブ型、または `EquatableArray<T>` に変換します。
- **コレクションの等価性**: コレクションデータ（例: `Methods`, `BindEvents`）は標準の配列や `List<T>` ではなく、構造的な等価性を保証する `EquatableArray<T>` (カスタム実装) を使用します。
