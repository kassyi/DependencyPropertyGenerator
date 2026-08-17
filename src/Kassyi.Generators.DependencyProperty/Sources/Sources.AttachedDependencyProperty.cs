using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateAttachedDependencyPropertySource(ClassData @class, DependencyPropertyData property)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateAttachedDependencyProperty(ref writer, @class, property);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    public static void GenerateAttachedDependencyProperty(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        using var _ = writer.ClassScope(@class);
        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.XmlDocumentation, property, isProperty: false);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);

        var strategy = Strategies.FrameworkGeneratorFactory.CreateDependencyPropertyStrategy(property.Framework);

        var propertyModifier = GeneratePropertyModifier(property);
        var propertyType = strategy.GeneratePropertyType(@class, property);
        var dependencyPropertyName = GenerateDependencyPropertyName(property);
        var managerType = strategy.GenerateManagerType(@class);
        var registerMethod = strategy.GenerateRegisterMethod(@class, property);
        var registerAttachedMethodArguments = strategy.GenerateRegisterMethodArguments(@class, property);

        writer.Append($"{propertyModifier} static readonly {propertyType} {dependencyPropertyName} =");
        writer.AppendLine($"{managerType}.{registerMethod}({registerAttachedMethodArguments});");

        strategy.GenerateAdditionalPropertyForReadOnlyProperties(ref writer, property);
        
        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.SetterXmlDocumentation, property, isProperty: true);
        GenerateCommonPropertyAttributes(ref writer, property, @class);

        var setterVisibility = property.IsReadOnly ? "internal" : "public";
        var browsableForType = GenerateBrowsableForType(property);
        var type = GenerateType(property);

        using (writer.Scope($"{setterVisibility} static void Set{property.Name}({browsableForType} element, {type} value)"))
        {
            writer.AppendLine("element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
            writer.AppendLine($"element.SetValue({dependencyPropertyName}, value);");
        }

        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.GetterXmlDocumentation, property, isProperty: true);
        GenerateCommonPropertyAttributes(ref writer, property, @class);
        GenerateBrowsableForTypeAttribute(ref writer, property);

        using (writer.Scope($"public static {type} Get{property.Name}({browsableForType} element)"))
        {
            writer.AppendLine("element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
            writer.AppendLine($"return ({type})element.GetValue({property.Name}Property);");
        }

        GenerateOnChangedMethods(ref writer, @class, property);
        GenerateOnChangingMethods(ref writer, @class, property);
        GenerateCoercePartialMethod(ref writer, property);
        GenerateValidatePartialMethod(ref writer, @class, property);
        GenerateCreateDefaultValueCallbackPartialMethod(ref writer, property);
        GenerateBindEventMethod(ref writer, property);
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

