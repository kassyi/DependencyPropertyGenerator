#nullable enable

using Kassyi.Generators.DependencyProperty.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
[TestCategory(TestCategoryNames.Language)]
public class LanguageFeatureTests : SnapshotTestBase
{
    // 1. Basic happy path (block-scoped namespace x public partial class)
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-001")]
    [DataRow(Framework.Wpf)]
    public Task Basic_Block_Public_Class(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    // 2. File-scoped namespace x internal partial class
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-002")]
    [DataRow(Framework.Wpf)]
    public Task FileScoped_Internal_Class(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + $$"""

            namespace MyNamespace;

            [DependencyProperty("MyProperty", typeof(string))]
            internal partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    // 3. Generic record class x public
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-003")]
    [DataRow(Framework.Wpf)]
    public Task RecordClass_Generic_Public(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty("MyProperty", typeof(object))]
            public partial record MyControl<T>
            {
            }
            """, framework, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 4. Generic type constraints with file-scoped namespace
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-004")]
    [DataRow(Framework.Wpf)]
    public Task GlobalNamespace_MultipleGenerics(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + $$"""

            [DependencyProperty("MyProperty", typeof(object))]
            internal partial class MyControl<T1, T2> : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    // 6. Nested class (asymmetric: non-generic outer, generic inner)
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-005")]
    [DataRow(Framework.Wpf)]
    public Task Generic_Class_WithConstraints(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + $$"""

            namespace MyNamespace;

            [DependencyProperty("MyProperty", typeof(object))]
            public partial class MyControl<T> : {{FrameworkTestData.GetUserControl(framework)}} where T : class, new()
            {
            }
            """, framework);
    }

    // 5. Global namespace x multiple generic type parameters
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-006")]
    [DataRow(Framework.Wpf)]
    public Task Generic_AllowsRefStruct(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System") + $$"""

            [DependencyProperty("MyProperty", typeof(object))]
            public partial class MyControl<T> : {{FrameworkTestData.GetUserControl(framework)}} where T : allows ref struct
            {
            }
            """, framework, skipE2EValidation: true);
    }

    // 21. 'new' modifier for shadowing inherited properties
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-007")]
    [DataRow(Framework.Wpf)]
    public Task SameNameClass_DifferentGenerics(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("MyProperty1", typeof(int))]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}} { }

            [DependencyProperty("MyProperty2", typeof(string))]
            public partial class MyControl<T> : UserControl { }
            """, framework);
    }

    // 12. Conflict avoidance for identically named types across different namespaces
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-008")]
    [DataRow(Framework.Wpf)]
    public Task Nested_OuterNonGen_InnerGen(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            public partial class OuterClass
            {
                [DependencyProperty("MyProperty", typeof(object))]
                private partial class InnerControl<T> : {{FrameworkTestData.GetUserControl(framework)}}
                {
                }
            }
            """, framework);
    }

    // 7. Nested class (asymmetric: generic outer, non-generic inner)
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-009")]
    [DataRow(Framework.Wpf)]
    public Task Nested_OuterGen_InnerNonGen(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + $$"""

            namespace MyNamespace;

            internal partial class OuterClass<T>
            {
                [DependencyProperty("MyProperty", typeof(int))]
                internal partial class InnerControl : {{FrameworkTestData.GetUserControl(framework)}}
                {
                }
            }
            """, framework);
    }

    // 8. Nested structure (mutually generic: outer record + inner class)
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-010")]
    [DataRow(Framework.Wpf)]
    public Task Nested_OuterGenRecord_InnerGenClass(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            public partial record OuterRecord<T1>
            {
                [DependencyProperty("MyProperty", typeof(object))]
                protected internal partial class InnerControl<T2> : {{FrameworkTestData.GetUserControl(framework)}}
                {
                }
            }
            """, framework);
    }

    // 10. Deeply nested types in global namespace
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-011")]
    [DataRow(Framework.Wpf)]
    public Task Global_Nested_Deep(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + $$"""

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
    [TestCategory($"{TestCategoryNames.Language}-012")]
    [DataRow(Framework.Wpf)]
    public Task Primary_Constructor(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl(int myParam) : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    // 19. Keyword escaping
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-013")]
    [DataRow(Framework.Wpf)]
    public Task Partial_Property_Conflict(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
                public partial int MyProperty { get; set; }
            }
            """, framework);
    }

    // 18. Primary constructor support
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-014")]
    [DataRow(Framework.Maui)]
    public Task RequiredInit_Properties(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Runtime.CompilerServices") + $$"""

            [DependencyProperty("MyProperty", typeof(int))]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
                public required partial int MyProperty { get; init; }
            }
            """, framework, skipE2EValidation: true);
    }

    // 24. Tuple property type
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-015")]
    [DataRow(Framework.Wpf)]
    public Task DefaultValue_CollectionExpression(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Collections.Generic") + $$"""

            [DependencyProperty("MyProperty", typeof(List<int>), DefaultValueExpression = "[]")]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework, skipE2EValidation: true);
    }

    // 23. Properties with 'required' and 'init' accessors
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-016")]
    [DataRow(Framework.Wpf)]
    public Task TargetTypedNewDefaultValueExpression(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            public record struct MyProfile(double A, double B);

            [DependencyProperty<MyProfile>("Profile", DefaultValueExpression = "new(1.5, 48.0)")]
            [DependencyProperty<MyProfile?>("NullableProfile", DefaultValueExpression = "new      (1.5, 48.0)")]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    // 29. ComponentModel attributes propagation
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-017")]
    [DataRow(Framework.Wpf)]
    public Task PropertyType_Tuple(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("MyProperty", typeof((int id, string name)))]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework, skipE2EValidation: true);
    }

    // 25. Complex nested nullable array type
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-018")]
    [DataRow(Framework.Wpf)]
    public Task PropertyType_ComplexNullableArray(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls", "System.Collections.Generic") + $$"""

            [DependencyProperty("MyProperty", typeof(List<int?>?[]))]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    // 27. required partial property with init
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-019")]
    [DataRow(Framework.Wpf)]
    public Task Keyword_Escaping(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("@event", typeof(string))]
            [DependencyProperty("class", typeof(int))]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    // 20. Generic constraint with 'allows ref struct' (anti-constraint)
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-020")]
    [DataRow(Framework.Wpf)]
    public Task Modifiers_Order_Normalization(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty("MyProperty1", typeof(int))]
            sealed public partial class ControlA : {{FrameworkTestData.GetUserControl(framework)}} { }

            [DependencyProperty("MyProperty2", typeof(int))]
            public sealed partial class ControlB : UserControl { }
            """, framework);
    }

    // 16. C# 13 partial property conflict
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-021")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task Attributes_ComponentModel(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(
            GetHeader(framework, "Controls", "System.ComponentModel") +
                    $$"""

                    [DependencyProperty<string>("AttributedProperty",
                        Category = "Category",
                        Description = "Description",
                        TypeConverter = typeof(EnumConverter),
                        Bindable = true,
                        DesignerSerializationVisibility = DesignerSerializationVisibility.Hidden,
                        ClsCompliant = false,
                        Localizability = Localizability.Text)]
                    public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
                    {
                    }
                    """, framework);
    }

    // 30. Validate and Coerce callbacks
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-022")]
    [DataRow(Framework.Wpf)]
    public Task Inheritance_NewModifier(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            public partial class ParentControl : {{FrameworkTestData.GetUserControl(framework)}}
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
    [TestCategory($"{TestCategoryNames.Language}-023")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task ValidateAndCoerce(Framework framework)
    {
        var conditionalSource = framework == Framework.Maui ?
            """

                private static partial bool IsNotNullStringPropertyValid(MyControl sender, string? value)
                {
                    return value != null;
                }
            """ :
            """

                private static partial bool IsNotNullStringPropertyValid(string? value)
                {
                    return value != null;
                }
            """;
            
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

              [DependencyProperty<string>("NotNullStringProperty", DefaultValue = "", Validate = true, Coerce = true)]
              public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
              {
                  private partial string CoerceNotNullStringProperty(string? value)
                  {
                      return value ?? string.Empty;
                  }
              """ + conditionalSource + """
              
              }
              """, framework);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-024")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task AttachedValidateAndCoerce(Framework framework)
    {
        var conditionalSource = framework == Framework.Maui ?
            $$"""

                private static partial bool IsAttachedNotNullStringPropertyValid({{FrameworkTestData.GetUserControl(framework)}} sender, string? value)
                {
                    return value != null;
                }
            """ :
            """

                private static partial bool IsAttachedNotNullStringPropertyValid(string? value)
                {
                    return value != null;
                }
            """;
            
        return CheckSourceAsync<AttachedDependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

              [AttachedDependencyProperty<string, {{FrameworkTestData.GetUserControl(framework)}}>("AttachedNotNullStringProperty", DefaultValue = "", Validate = true, Coerce = true)]
              public static partial class MyControlHelper
              {
                  private static partial string CoerceAttachedNotNullStringProperty({{FrameworkTestData.GetUserControl(framework)}} sender, string? value)
                  {
                      return value ?? string.Empty;
                  }
              """ + conditionalSource + """
              
              }
              """, framework);
    }

    // 31. Avalonia specific affects flags
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-025")]
    public Task AvaloniaAffectsFlags()
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(Framework.Avalonia, "Controls", "Media") + $$"""

            [DependencyProperty<Brush>("Fill", AffectsRender = true, AffectsMeasure = true, AffectsArrange = true)]
            public partial class MyControl : Control
            {
            }
            """, Framework.Avalonia, additionalGenerators: new StaticConstructorGenerator());
    }

    // 32. BindEvents static constructor generation
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-026")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task BindEvents(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, string.Empty, "Input") + $$"""

            [DependencyProperty<object>("BindEventsProperty",
                BindEvents = new[] { nameof({{FrameworkTestData.GetUIElement(framework)}}.{{FrameworkTestData.GetPointerEnteredEventName(framework)}}), nameof({{FrameworkTestData.GetUIElement(framework)}}.{{FrameworkTestData.GetPointerExitedEventName(framework)}}) })]
            public partial class MyUIElement : {{FrameworkTestData.GetUIElement(framework)}}
            {
                private static void OnBindEventsPropertyChanged_{{FrameworkTestData.GetPointerEnteredEventName(framework)}}(object? sender, {{FrameworkTestData.GetPointerEventArgs(framework)}} args)
                {
                }

                private static void OnBindEventsPropertyChanged_{{FrameworkTestData.GetPointerExitedEventName(framework)}}(object? sender, {{FrameworkTestData.GetPointerEventArgs(framework)}} args)
                {
                }
            }
            """, framework, additionalGenerators: new StaticConstructorGenerator());
    }

    // 33. Multidimensional array property type
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-027")]
    [DataRow(Framework.Wpf)]
    public Task MultidimensionalArray(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, string.Empty) + $$"""

            [DependencyProperty<int[,,]>("Values3")]
            public partial class MyControl : {{FrameworkTestData.GetFrameworkElement(framework)}}
            {
            }
            """, framework);
    }

    // 34. Unmanaged function pointer
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-028")]
    [DataRow(Framework.Wpf)]
    public Task Nullable_Context_Record(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>("#nullable enable\n" + GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty("MyProperty", typeof(string))]
            public partial record MyControl
            {
            }
            """, framework, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 14. Attached property on static class (non-instantiable)
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-029")]
    [DataRow(Framework.Wpf)]
    public Task AttachedProperty_StaticClass(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [AttachedDependencyProperty("MyProperty", typeof(int), BrowsableForType = typeof(global::System.Windows.{{FrameworkTestData.GetDependencyObject(framework)}}))]
            public static partial class MyHelper
            {
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 15. Modifier order normalization invariance
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-030")]
    [DataRow(Framework.Wpf)]
    public Task SameNameClass_DifferentNamespace(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + $$"""

            namespace NamespaceA
            {
                [DependencyProperty("MyPropertyA", typeof(int))]
                public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}} { }
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
    [TestCategory($"{TestCategoryNames.Language}-031")]
    [DataRow(Framework.Wpf)]
    public Task Partial_Property_Required_Init(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, nullable: true, @namespace: false, "Controls") + $$"""

            namespace System.Runtime.CompilerServices
            {
                internal static class IsExternalInit {}
                public class RequiredMemberAttribute : Attribute {}
                public class CompilerFeatureRequiredAttribute : Attribute { public CompilerFeatureRequiredAttribute(string name) {} }
            }

            namespace Kassyi.Generators.DependencyProperty.IntegrationTests
            {
                [DependencyProperty("MyProperty", typeof(int))]
                public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
                {
                    public required partial int MyProperty { get; init; }
                }
            }
            """, framework);
    }

    // 28. Target-typed new default value expression
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-032")]
    [DataRow(Framework.Wpf)]
    public Task Unmanaged_Function_Pointer(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, string.Empty) + $$"""

            [AttachedDependencyProperty<delegate* unmanaged<int, void>, {{FrameworkTestData.GetDependencyObject(framework)}}>("CallbackPtr")]
            public static partial class UnsafeExtensions
            {
            }
            """, framework, skipE2EValidation: true, additionalGenerators: new AttachedDependencyPropertyGenerator());
    }

    // 35. String default value with expressions (nameof, string.Concat) should not emit DPG0004
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-033")]
    [DataRow(Framework.Wpf)]
    public Task String_DefaultValue_Expressions(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            [DependencyProperty<string>("Name1", DefaultValue = "nameof(MyControl)")]
            [DependencyProperty<string>("Name2", DefaultValueExpression = "string.Concat(\"A\", \"B\")")]
            [DependencyProperty<string>("Name3", DefaultValue = "string.Empty")]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-034")]
    [DataRow(Framework.Wpf)]
    public Task SemanticType_NullableGenerics_ShouldNotBeReplaced(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            using System.Collections.Generic;

            [DependencyProperty<List<int?>>("MyProperty")]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
                partial void OnMyPropertyChanged(List<int?>? newValue) { }
            }
            """, framework);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Language}-035")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task CreateDefaultValueCallback_ShouldNotForceNullableOnReferenceTypes(Framework framework)
    {
        return CheckSourceAsync<DependencyPropertyGenerator>(GetHeader(framework, "Controls") + $$"""

            using System;

            [DependencyProperty<Uri>("CardBackground", CreateDefaultValueCallback = true)]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
                private static partial Uri GetCardBackgroundDefaultValue() => null!;
            }
            """, framework);
    }
}
