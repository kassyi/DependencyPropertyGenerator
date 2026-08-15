#nullable enable

using Kassyi.Generators.DependencyProperty.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class OrthogonalMatrixTests : SnapshotTestBase
{
    // 1. 基本ハッピーパス (ブロック名前空間 x public partial class)
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Basic_Block_Public_Class(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl : UserControl
            {
            }
            """, framework);
    }

    // 2. File-scoped x internal partial class
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task FileScoped_Internal_Class(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + """

            namespace MyNamespace;

            [DependencyProperty("MyProperty", typeof(string))]
            internal partial class MyControl : UserControl
            {
            }
            """, framework);
    }

    // 3. Record Class x Generic x public
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task RecordClass_Generic_Public(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [AttachedDependencyProperty("MyProperty", typeof(object))]
            public partial record MyControl<T>
            {
            }
            """, framework, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 4. ジェネリック制約 (ファイルスコープ)
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Generic_Class_WithConstraints(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + """

            namespace MyNamespace;

            [DependencyProperty("MyProperty", typeof(object))]
            public partial class MyControl<T> : UserControl where T : class, new()
            {
            }
            """, framework);
    }

    // 5. Global名前空間 x 複数ジェネリクス
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task GlobalNamespace_MultipleGenerics(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + """

            [DependencyProperty("MyProperty", typeof(object))]
            internal partial class MyControl<T1, T2> : UserControl
            {
            }
            """, framework);
    }

    // 6. ネスト構造 (非対称: 外側非ジェネリック、内側ジェネリック)
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Nested_OuterNonGen_InnerGen(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            public partial class OuterClass
            {
                [DependencyProperty("MyProperty", typeof(object))]
                private partial class InnerControl<T> : UserControl
                {
                }
            }
            """, framework);
    }

    // 7. ネスト構造 (非対称: 外側ジェネリック、内側非ジェネリック)
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Nested_OuterGen_InnerNonGen(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + """

            namespace MyNamespace;

            internal partial class OuterClass<T>
            {
                [DependencyProperty("MyProperty", typeof(int))]
                internal partial class InnerControl : UserControl
                {
                }
            }
            """, framework);
    }

    // 8. ネスト構造 (相互ジェネリック + RecordとClassの混合)
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Nested_OuterGenRecord_InnerGenClass(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            public partial record OuterRecord<T1>
            {
                [DependencyProperty("MyProperty", typeof(object))]
                protected internal partial class InnerControl<T2> : UserControl
                {
                }
            }
            """, framework);
    }

    // 9. file 修飾子によるアセンブリ内非公開の境界条件
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task FileModifier_Class(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("MyProperty", typeof(int))]
            file partial class MyControl : UserControl
            {
            }
            """, framework, skipE2EValidation: true);
    }

    // 10. グローバル名前空間での深いネスト
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Global_Nested_Deep(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + """

            public partial class Level1
            {
                public partial class Level2
                {
                    [AttachedDependencyProperty("MyProperty", typeof(string))]
                    public partial record Level3
                    {
                    }
                }
            }
            """, framework, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 11. 同一名前空間におけるメタデータアリティ(ジェネリクス引数)の違い
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task SameNameClass_DifferentGenerics(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("MyProperty1", typeof(int))]
            public partial class MyControl : UserControl { }

            [DependencyProperty("MyProperty2", typeof(string))]
            public partial class MyControl<T> : UserControl { }
            """, framework);
    }

    // 12. 異なる名前空間における同名クラスの衝突回避
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task SameNameClass_DifferentNamespace(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + """

            namespace NamespaceA
            {
                [DependencyProperty("MyPropertyA", typeof(int))]
                public partial class MyControl : UserControl { }
            }

            namespace NamespaceB
            {
                [DependencyProperty("MyPropertyB", typeof(string))]
                public partial class MyControl : UserControl { }
            }
            """, framework);
    }

    // 13. Nullableコンテキストとレコード
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Nullable_Context_Record(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>("#nullable enable\n" + GetHeader(framework, "Controls") + """

            [AttachedDependencyProperty("MyProperty", typeof(string))]
            public partial record MyControl
            {
            }
            """, framework, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 14. 添付プロパティ x スタティッククラス (インスタンス化不可)
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task AttachedProperty_StaticClass(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [AttachedDependencyProperty("MyProperty", typeof(int), BrowsableForType = typeof(global::System.Windows.DependencyObject))]
            public static partial class MyHelper
            {
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 15. 修飾子の順序に依存しない正規化検証
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Modifiers_Order_Normalization(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("MyProperty1", typeof(int))]
            sealed public partial class ControlA : UserControl { }

            [DependencyProperty("MyProperty2", typeof(int))]
            public sealed partial class ControlB : UserControl { }
            """, framework);
    }

    // 16. C# 13 partial properties conflict
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Partial_Property_Conflict(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl : UserControl
            {
                public partial int MyProperty { get; set; }
            }
            """, framework);
    }

    // 17. ref struct type rejection
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task RefStruct_PropertyType_Rejection(Framework framework)
    {
        // This should skip generation or emit a diagnostic because MyRefStruct is a ref struct
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System") + """

            public ref struct MyRefStruct { }

            [DependencyProperty("MyProperty", typeof(MyRefStruct))]
            public partial class MyControl : UserControl
            {
            }
            """, framework, skipE2EValidation: true);
    }

    // 18. Primary constructor
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Primary_Constructor(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl(int myParam) : UserControl
            {
            }
            """, framework);
    }

    // 19. Keyword escaping
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Keyword_Escaping(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("@event", typeof(string))]
            [DependencyProperty("class", typeof(int))]
            public partial class MyControl : UserControl
            {
            }
            """, framework);
    }

    // 20. allows ref struct
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Generic_AllowsRefStruct(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System") + """

            [DependencyProperty("MyProperty", typeof(object))]
            public partial class MyControl<T> : UserControl where T : allows ref struct
            {
            }
            """, framework, skipE2EValidation: true);
    }

    // 21. new modifier for inherited properties
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Inheritance_NewModifier(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            public partial class ParentControl : UserControl
            {
                public string MyProperty { get; set; } = string.Empty;
            }

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl : ParentControl
            {
            }
            """, framework);
    }

    // 22. Collection expression default value
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task DefaultValue_CollectionExpression(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Collections.Generic") + """

            [DependencyProperty("MyProperty", typeof(List<int>), DefaultValueExpression = "[]")]
            public partial class MyControl : UserControl
            {
            }
            """, framework);
    }

    // 23. required / init properties
    [TestMethod]
    [DataRow(Framework.Maui)]
    public Task RequiredInit_Properties(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Runtime.CompilerServices") + """

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl : UserControl
            {
                public required partial int MyProperty { get; init; }
            }
            """, framework, skipE2EValidation: true);
    }

    // 24. Tuple Type
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task PropertyType_Tuple(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + """

            [DependencyProperty("MyProperty", typeof((int id, string name)))]
            public partial class MyControl : UserControl
            {
            }
            """, framework, skipE2EValidation: true);
    }

    // 25. Complex Array/Nullable
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task PropertyType_ComplexNullableArray(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Collections.Generic") + """

            [DependencyProperty("MyProperty", typeof(List<int?>?[]))]
            public partial class MyControl : UserControl
            {
            }
            """, framework);
    }

    // DPG0004: Reference Type Default Value Sharing
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task ReferenceType_DefaultValue_Rejection(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Collections.Generic") + """

            [DependencyProperty("MyProperty", typeof(List<int>), DefaultValueExpression = "new List<int>()")]
            public partial class MyControl : UserControl
            {
            }
            """, framework, skipE2EValidation: true);
    }
}

