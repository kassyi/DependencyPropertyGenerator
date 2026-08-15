#nullable enable

using Kassyi.Generators.DependencyProperty.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class OrthogonalMatrixTests : SnapshotTestBase
{
    // 1. Basic happy path (block-scoped namespace x public partial class)
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

    // 2. File-scoped namespace x internal partial class
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

    // 3. Generic record class x public
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

    // 4. Generic type constraints with file-scoped namespace
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

    // 5. Global namespace x multiple generic type parameters
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

    // 6. Nested class (asymmetric: non-generic outer, generic inner)
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

    // 7. Nested class (asymmetric: generic outer, non-generic inner)
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

    // 8. Nested structure (mutually generic: outer record + inner class)
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

    // 9. Boundary condition: assembly-private 'file' modifier
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

    // 10. Deeply nested types in global namespace
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

    // 11. Type arity distinction for identically named types in the same namespace
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

    // 12. Conflict avoidance for identically named types across different namespaces
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

    // 13. Nullable context with record class
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

    // 14. Attached property on static class (non-instantiable)
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

    // 15. Modifier order normalization invariance
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

    // 16. C# 13 partial property conflict
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

    // 17. Property type rejection for 'ref struct'
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

    // 18. Primary constructor support
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

    // 20. Generic constraint with 'allows ref struct' (anti-constraint)
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

    // 21. 'new' modifier for shadowing inherited properties
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
            """, framework, skipE2EValidation: true);
    }

    // 23. Properties with 'required' and 'init' accessors
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

    // 24. Tuple property type
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

    // 25. Complex nested nullable array type
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

    // DPG0004: Diagnostic rejection for shared reference type default values
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

    // 27. required partial property with init
    [TestMethod]
    [DataRow(Framework.Wpf)]
    public Task Partial_Property_Required_Init(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + """

            namespace System.Runtime.CompilerServices
            {
                internal static class IsExternalInit {}
                public class RequiredMemberAttribute : Attribute {}
                public class CompilerFeatureRequiredAttribute : Attribute { public CompilerFeatureRequiredAttribute(string name) {} }
            }

            namespace Kassyi.Generators.DependencyProperty.IntegrationTests
            {
                [DependencyProperty("MyProperty", typeof(int))]
                public partial class MyControl : UserControl
                {
                    public required partial int MyProperty { get; init; }
                }
            }
            """, framework);
    }
}

