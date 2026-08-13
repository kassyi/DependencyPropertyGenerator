using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateAttachedDependencyProperty(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        writer.AppendLine($$"""

        #nullable enable

        namespace {{@class.Namespace}}
        {
            {{GenerateModifiers(@class)}}partial class {{@class.Name}}
            {
        """);

        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.XmlDocumentation, property, isProperty: false);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);

        var propertyModifier = GeneratePropertyModifier(property);
        var propertyType = GeneratePropertyType(@class, property);
        var dependencyPropertyName = GenerateDependencyPropertyName(property);
        var managerType = GenerateManagerType(@class);
        var registerMethod = GenerateRegisterMethod(@class, property);
        var registerAttachedMethodArguments = GenerateRegisterAttachedMethodArguments(@class, property);

        writer.Append($"        {propertyModifier} static readonly {propertyType} {dependencyPropertyName} =");
        writer.AppendLine($"            {managerType}.{registerMethod}(");
        writer.AppendLine($"                {registerAttachedMethodArguments});");

        GenerateAdditionalPropertyForReadOnlyProperties(ref writer, property);
        
        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.SetterXmlDocumentation, property, isProperty: true);
        GenerateCategoryAttribute(ref writer, property.ComponentModel.Category);
        GenerateDescriptionAttribute(ref writer, property.ComponentModel.Description);
        GenerateTypeConverterAttribute(ref writer, property.ComponentModel.TypeConverter);
        GenerateBindableAttribute(ref writer, property.ComponentModel.Bindable);
        GenerateBrowsableAttribute(ref writer, property.ComponentModel.Browsable);
        GenerateDesignerSerializationVisibilityAttribute(ref writer, property.ComponentModel.DesignerSerializationVisibility);
        GenerateClsCompliantAttribute(ref writer, property.ComponentModel.ClsCompliant);
        GenerateLocalizabilityAttribute(ref writer, property.ComponentModel.Localizability, @class.Framework);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        GenerateExcludeFromCodeCoverageAttribute(ref writer);

        var setterVisibility = property.IsReadOnly ? "internal" : "public";
        var browsableForType = GenerateBrowsableForType(property);
        var type = GenerateType(property);

        writer.AppendLine($$"""
        {{setterVisibility}} static void Set{{property.Name}}({{browsableForType}} element, {{type}} value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));

            element.SetValue({{dependencyPropertyName}}, value);
        }
""");

        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.GetterXmlDocumentation, property, isProperty: true);
        GenerateCategoryAttribute(ref writer, property.ComponentModel.Category);
        GenerateDescriptionAttribute(ref writer, property.ComponentModel.Description);
        GenerateTypeConverterAttribute(ref writer, property.ComponentModel.TypeConverter);
        GenerateBindableAttribute(ref writer, property.ComponentModel.Bindable);
        GenerateBrowsableAttribute(ref writer, property.ComponentModel.Browsable);
        GenerateDesignerSerializationVisibilityAttribute(ref writer, property.ComponentModel.DesignerSerializationVisibility);
        GenerateBrowsableForTypeAttribute(ref writer, property);
        GenerateClsCompliantAttribute(ref writer, property.ComponentModel.ClsCompliant);
        GenerateLocalizabilityAttribute(ref writer, property.ComponentModel.Localizability, @class.Framework);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        GenerateExcludeFromCodeCoverageAttribute(ref writer);

        writer.AppendLine($$"""
        public static {{type}} Get{{property.Name}}({{browsableForType}} element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));

            return ({{type}})element.GetValue({{property.Name}}Property);
        }
""");

        GenerateOnChangedMethods(ref writer, @class, property);
        GenerateOnChangingMethods(ref writer, @class, property);
        GenerateCoercePartialMethod(ref writer, property);
        GenerateValidatePartialMethod(ref writer, @class, property);
        GenerateCreateDefaultValueCallbackPartialMethod(ref writer, property);
        GenerateBindEventMethod(ref writer, property);

        writer.AppendLine("    }");
        writer.AppendLine("}");
    }
    
    private static string GenerateRegisterAttachedMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(property.Framework);
        return generator.GenerateRegisterMethodArguments(@class, property);
    }

    internal static string GenerateModifiers(ClassData @class)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => string.Empty,
            _ => @class.Modifiers
        };
    }
}

