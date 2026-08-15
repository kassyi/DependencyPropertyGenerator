# Test Specification Index

本テストスイートの設計思想、直交表パラメータ、テストケース一覧、および合否判定基準をまとめた正式なテスト仕様書は以下に配置されています。

- 🇯🇵 **[テスト仕様書（日本語）](../../spec/ja/05_test_specification.md)**
- 🇺🇸 **[Test Specification (English)](../../spec/en/05_test_specification.md)**

---

## 概要クイックリンク

1. **[CombinatorialMatrixTests.cs](./Kassyi.Generators.DependencyProperty.SnapshotTests/CombinatorialMatrixTests.cs)**: 全 672 ケースの全直積属性・型組み合わせテスト（データ駆動型）
2. **[LanguageFeatureTests.cs](./Kassyi.Generators.DependencyProperty.SnapshotTests/LanguageFeatureTests.cs)**: C# 言語仕様・構文エッジケースとの直交性スナップショットテスト
3. **[ErrorTests.cs](./Kassyi.Generators.DependencyProperty.SnapshotTests/ErrorTests.cs)**: `DPG0001`〜`DPG0005` のコンパイル時診断通知テスト
