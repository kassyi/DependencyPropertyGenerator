using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateDependencyProperty(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        writer.AppendLine($$"""

        #nullable enable

        namespace {{@class.Namespace}}
        {
            {{@class.Modifiers}}partial class {{@class.Name}}
            {
        """);

        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.XmlDocumentation, property, isProperty: false);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);

        var propertyModifier = GeneratePropertyModifier(property);
        var propertyType = GeneratePropertyType(@class, property);
        var dependencyPropertyName = GenerateDependencyPropertyName(property);

        writer.Append($"        {propertyModifier} static readonly {propertyType} {dependencyPropertyName} =");
        GenerateDependencyPropertyCreateCall(ref writer, @class, property);
        writer.AppendLine();

        GenerateAdditionalFieldForDirectProperties(ref writer, property);
        GenerateAdditionalPropertyForReadOnlyProperties(ref writer, property);
        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.GetterXmlDocumentation, property, isProperty: true);
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

        writer.AppendLine($"        public {GenerateType(property)} {property.Name}");
        writer.AppendLine("        {");
        writer.Append("            ");
        GenerateGetter(ref writer, property);
        writer.AppendLine();
        if (!property.IsReadOnly)
        {
            writer.Append("            ");
            GenerateSetter(ref writer, property);
            writer.AppendLine();
        }
        writer.AppendLine("        }");

        GenerateOnChangedMethods(ref writer, @class, property);
        GenerateOnChangingMethods(ref writer, @class, property);
        GenerateCoercePartialMethod(ref writer, property);
        GenerateValidatePartialMethod(ref writer, @class, property);
        GenerateCreateDefaultValueCallbackPartialMethod(ref writer, property);
        GenerateBindEventMethod(ref writer, property);

        writer.AppendLine("""
            }
        }
        """);
    }
    private static void GenerateGetter(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property is { IsDirect: true, Framework: Framework.Avalonia })
        {
            writer.Append($"get => _{property.Name.ToParameterName()};");
        }
        else
        {
            writer.Append($"get => ({GenerateType(property)})GetValue({property.Name}Property);");
        }
    }

    private static void GenerateSetter(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property is { IsDirect: true, Framework: Framework.Avalonia })
        {
            writer.Append($$"""
            private set
                        {
                            var oldValue = _{{property.Name.ToParameterName()}};
                            SetAndRaise({{property.Name}}Property, ref _{{property.Name.ToParameterName()}}, value);
                            On{{property.Name}}Changed();
                            On{{property.Name}}Changed(
                                ({{GenerateType(property)}})value);
                            On{{property.Name}}Changed(
                                ({{GenerateType(property)}})oldValue,
                                ({{GenerateType(property)}})value);
                        }
            """);
        }
        else
        {
            writer.Append($"{GenerateAdditionalSetterModifier(property)}set => SetValue({GenerateDependencyPropertyName(property)}, value);");
        }
    }

    private static void GenerateDependencyPropertyCreateCall(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        if (property.IsAddOwner)
        {
            GenerateAddOwnerCreateCall(ref writer, @class, property);
        }
        else
        {
            writer.AppendLine();
            writer.Append($"""
                              {GenerateManagerType(@class)}.{GenerateRegisterMethod(@class, property)}(
                                  {GenerateRegisterMethodArguments(@class, property)});
                  """);
        }
    }


    private static string GenerateManagerType(ClassData @class)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(@class.Framework);
        return generator.GenerateManagerType(@class);
    }
    
    private static string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(property.Framework);
        return generator.GenerateRegisterMethodArguments(@class, property);
    }

    private static string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(property.Framework);
        return generator.GenerateRegisterMethod(@class, property);
    }
    private static void GenerateCoercePartialMethod(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (!property.ValidationAndCallbacks.Coerce)
        {
            writer.Append(" ");
            return;
        }

        if (property.IsAttached)
        {
            writer.Append($"        private static partial {GenerateType(property)} Coerce{property.Name}({GenerateBrowsableForType(property)} {GenerateBrowsableForTypeParameterName(property)}, {GenerateType(property, canBeNull: true)} value);");
        }
        else
        {
            writer.Append($"        private partial {GenerateType(property)} Coerce{property.Name}({GenerateType(property, canBeNull: true)} value);");
        }
    }

    private static void GenerateAdditionalFieldForDirectProperties(ref SourceWriter writer, DependencyPropertyData property)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(property.Framework);
        generator.GenerateAdditionalFieldForDirectProperties(ref writer, property);
    }



    private static void GenerateAdditionalPropertyForReadOnlyProperties(ref SourceWriter writer, DependencyPropertyData property)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(property.Framework);
        generator.GenerateAdditionalPropertyForReadOnlyProperties(ref writer, property);
    }
}

