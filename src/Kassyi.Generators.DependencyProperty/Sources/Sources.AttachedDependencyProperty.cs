using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateAttachedDependencyProperty(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        writer.AppendLine();
        writer.AppendLine("#nullable enable");
        writer.AppendLine();
        writer.AppendLine($"namespace {@class.Namespace}");
        writer.AppendLine("{");
        writer.AppendLine($"    {GenerateModifiers(@class)}partial class {@class.Name}");
        writer.AppendLine("    {");

        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation, property, isProperty: false);
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
        
        GenerateXmlDocumentationFrom(ref writer, property.SetterXmlDocumentation, property, isProperty: true);
        GenerateCategoryAttribute(ref writer, property.Category);
        GenerateDescriptionAttribute(ref writer, property.Description);
        GenerateTypeConverterAttribute(ref writer, property.TypeConverter);
        GenerateBindableAttribute(ref writer, property.Bindable);
        GenerateBrowsableAttribute(ref writer, property.Browsable);
        GenerateDesignerSerializationVisibilityAttribute(ref writer, property.DesignerSerializationVisibility);
        GenerateClsCompliantAttribute(ref writer, property.ClsCompliant);
        GenerateLocalizabilityAttribute(ref writer, property.Localizability, @class.Framework);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        GenerateExcludeFromCodeCoverageAttribute(ref writer);

        var setterVisibility = property.IsReadOnly ? "internal" : "public";
        var browsableForType = GenerateBrowsableForType(property);
        var type = GenerateType(property);

        writer.AppendLine($"        {setterVisibility} static void Set{property.Name}({browsableForType} element, {type} value)");
        writer.AppendLine("        {");
        writer.AppendLine("            element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
        writer.AppendLine();
        writer.AppendLine($"            element.SetValue({dependencyPropertyName}, value);");
        writer.AppendLine("        }");

        GenerateXmlDocumentationFrom(ref writer, property.GetterXmlDocumentation, property, isProperty: true);
        GenerateCategoryAttribute(ref writer, property.Category);
        GenerateDescriptionAttribute(ref writer, property.Description);
        GenerateTypeConverterAttribute(ref writer, property.TypeConverter);
        GenerateBindableAttribute(ref writer, property.Bindable);
        GenerateBrowsableAttribute(ref writer, property.Browsable);
        GenerateDesignerSerializationVisibilityAttribute(ref writer, property.DesignerSerializationVisibility);
        GenerateBrowsableForTypeAttribute(ref writer, property);
        GenerateClsCompliantAttribute(ref writer, property.ClsCompliant);
        GenerateLocalizabilityAttribute(ref writer, property.Localizability, @class.Framework);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        GenerateExcludeFromCodeCoverageAttribute(ref writer);

        writer.AppendLine($"        public static {type} Get{property.Name}({browsableForType} element)");
        writer.AppendLine("        {");
        writer.AppendLine("            element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
        writer.AppendLine();
        writer.AppendLine($"            return ({type})element.GetValue({property.Name}Property);");
        writer.AppendLine("        }");

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

