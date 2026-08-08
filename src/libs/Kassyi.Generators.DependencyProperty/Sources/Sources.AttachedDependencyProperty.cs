using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateAttachedDependencyProperty(ClassData @class, DependencyPropertyData property)
    {
        var modifiers = GenerateModifiers(@class);
        var xmlDocumentation = GenerateXmlDocumentationFrom(property.XmlDocumentation, property, isProperty: false);
        var generatedCodeAttribute = GenerateGeneratedCodeAttribute(@class.Version);
        var propertyModifier = GeneratePropertyModifier(property);
        var propertyType = GeneratePropertyType(@class, property);
        var dependencyPropertyName = GenerateDependencyPropertyName(property);
        var managerType = GenerateManagerType(@class);
        var registerMethod = GenerateRegisterMethod(@class, property);
        var registerAttachedMethodArguments = GenerateRegisterAttachedMethodArguments(@class, property);

        var additionalPropertyForReadOnlyProperties = GenerateAdditionalPropertyForReadOnlyProperties(property);
        var setterXmlDocumentation = GenerateXmlDocumentationFrom(property.SetterXmlDocumentation, property, isProperty: true);
        var categoryAttribute = GenerateCategoryAttribute(property.Category);
        var descriptionAttribute = GenerateDescriptionAttribute(property.Description);
        var typeConverterAttribute = GenerateTypeConverterAttribute(property.TypeConverter);
        var bindableAttribute = GenerateBindableAttribute(property.Bindable);
        var browsableAttribute = GenerateBrowsableAttribute(property.Browsable);
        var designerSerializationVisibilityAttribute = GenerateDesignerSerializationVisibilityAttribute(property.DesignerSerializationVisibility);
        var clsCompliantAttribute = GenerateClsCompliantAttribute(property.ClsCompliant);
        var localizabilityAttribute = GenerateLocalizabilityAttribute(property.Localizability, @class.Framework);
        var excludeFromCodeCoverageAttribute = GenerateExcludeFromCodeCoverageAttribute();
        
        var setterVisibility = property.IsReadOnly ? "internal" : "public";
        var browsableForType = GenerateBrowsableForType(property);
        var type = GenerateType(property);

        var getterXmlDocumentation = GenerateXmlDocumentationFrom(property.GetterXmlDocumentation, property, isProperty: true);
        var browsableForTypeAttribute = GenerateBrowsableForTypeAttribute(property);

        var onChangedMethods = GenerateOnChangedMethods(@class, property);
        var onChangingMethods = GenerateOnChangingMethods(property);
        var coercePartialMethod = GenerateCoercePartialMethod(property);
        var validatePartialMethod = GenerateValidatePartialMethod(@class, property);
        var createDefaultValueCallbackPartialMethod = GenerateCreateDefaultValueCallbackPartialMethod(property);
        var bindEventMethod = GenerateBindEventMethod(property);

        return $$"""

            #nullable enable

            namespace {{@class.Namespace}}
            {
                {{modifiers}}partial class {{@class.Name}}
                {
            {{xmlDocumentation}}
            {{generatedCodeAttribute}}
                    {{propertyModifier}} static readonly {{propertyType}} {{dependencyPropertyName}} =
                        {{managerType}}.{{registerMethod}}(
                            {{registerAttachedMethodArguments}});

            {{additionalPropertyForReadOnlyProperties}}
            {{setterXmlDocumentation}}
            {{categoryAttribute}}
            {{descriptionAttribute}}
            {{typeConverterAttribute}}
            {{bindableAttribute}}
            {{browsableAttribute}}
            {{designerSerializationVisibilityAttribute}}
            {{clsCompliantAttribute}}
            {{localizabilityAttribute}}
            {{generatedCodeAttribute}}
            {{excludeFromCodeCoverageAttribute}}
                    {{setterVisibility}} static void Set{{property.Name}}({{browsableForType}} element, {{type}} value)
                    {
                        element = element ?? throw new global::System.ArgumentNullException(nameof(element));

                        element.SetValue({{dependencyPropertyName}}, value);
                    }

            {{getterXmlDocumentation}}
            {{categoryAttribute}}
            {{descriptionAttribute}}
            {{typeConverterAttribute}}
            {{bindableAttribute}}
            {{browsableAttribute}}
            {{designerSerializationVisibilityAttribute}}
            {{browsableForTypeAttribute}}
            {{clsCompliantAttribute}}
            {{localizabilityAttribute}}
            {{generatedCodeAttribute}}
            {{excludeFromCodeCoverageAttribute}}
                    public static {{type}} Get{{property.Name}}({{browsableForType}} element)
                    {
                        element = element ?? throw new global::System.ArgumentNullException(nameof(element));

                        return ({{type}})element.GetValue({{property.Name}}Property);
                    }

            {{onChangedMethods}}
            {{onChangingMethods}}
            {{coercePartialMethod}}
            {{validatePartialMethod}}
            {{createDefaultValueCallbackPartialMethod}}
            {{bindEventMethod}}
                }
            }
            """.RemoveBlankLinesWhereOnlyWhitespaces();
    }
    
    private static string GenerateRegisterAttachedMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        if (@class.Framework == Framework.Maui)
        {
            return GenerateMauiRegisterMethodArguments(@class, property);
        }

        if (@class.Framework == Framework.Avalonia)
        {
            return GenerateAvaloniaRegisterMethodArguments(@class, property);
        }

        if (@class.Framework == Framework.Wpf)
        {
            return $"""

                                    name: "{property.Name}",
                                    propertyType: typeof({property.Type}),
                                    ownerType: typeof({@class.Type}),
                                    {GeneratePropertyMetadata(@class, property)},
                                    validateValueCallback: {GenerateValidateValueCallback(@class, property)}
                    """;
        }

        return $"""

                                name: "{property.Name}",
                                propertyType: typeof({property.Type}),
                                ownerType: typeof({@class.Type}),
                                {GeneratePropertyMetadata(@class, property)}
                """;
    }

    private static string GenerateModifiers(ClassData @class)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return string.Empty;
        }

        return @class.Modifiers;
    }
}

