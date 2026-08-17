using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateDependencyPropertySource(ClassData @class, DependencyPropertyData property)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateDependencyProperty(ref writer, @class, property);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    public static void GenerateDependencyProperty(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        using var _ = writer.ClassScope(@class);
        var strategy = GeneratePropertyHeader(ref writer, @class, property);

        var propertyModifier = GeneratePropertyModifier(property);
        var propertyType = strategy.GeneratePropertyType(@class, property);
        var dependencyPropertyName = GenerateDependencyPropertyName(property);

        writer.Append($"{propertyModifier} static readonly {propertyType} {dependencyPropertyName} =");
        if (property.Modifiers.IsAddOwner)
        {
            GenerateAddOwnerCreateCall(ref writer, @class, property);
        }
        else
        {
            writer.AppendLine();
            writer.AppendLine($"{strategy.GenerateManagerType(@class)}.{strategy.GenerateRegisterMethod(@class, property)}({strategy.GenerateRegisterMethodArguments(@class, property)});");
        }
        writer.AppendLine();

        strategy.GenerateAdditionalFieldForDirectProperties(ref writer, property);
        strategy.GenerateAdditionalPropertyForReadOnlyProperties(ref writer, property);
        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.GetterXmlDocumentation, property, isProperty: true);
        GenerateCommonPropertyAttributes(ref writer, property, @class);

        var partialModifier = property.Modifiers.IsPartialProperty ? "partial " : string.Empty;
        var requiredModifier = property.Modifiers.IsRequired ? "required " : string.Empty;
        var newModifier = property.Modifiers.HidesBaseProperty ? "new " : string.Empty;
        using (writer.Scope($"public {newModifier}{requiredModifier}{partialModifier}{GenerateType(property)} {property.Name.EscapeKeyword()}"))
        {
            GenerateGetter(ref writer, property);
            writer.AppendLine();
            if (!property.Modifiers.IsReadOnly)
            {
                GenerateSetter(ref writer, property);
                writer.AppendLine();
            }
        }

        GeneratePropertyFooter(ref writer, @class, property);
    }
    private static void GenerateGetter(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property is { Modifiers.IsDirect: true, Framework: Framework.Avalonia })
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
        if (property is { Modifiers.IsDirect: true, Framework: Framework.Avalonia })
        {
            using (writer.Scope("private set"))
            {
                var type = GenerateType(property);
                writer.AppendLine($"var oldValue = _{property.Name.ToParameterName()};");
                writer.AppendLine($"SetAndRaise({property.Name}Property, ref _{property.Name.ToParameterName()}, value);");
                writer.AppendLine($"On{property.Name}Changed();");
                writer.AppendLine($"On{property.Name}Changed(({type})value);");
                writer.AppendLine($"On{property.Name}Changed(({type})oldValue, ({type})value);");
            }
        }
        else
        {
            var setOrInit = property.Modifiers.IsInitOnly ? "init" : "set";
            writer.AppendLine($"{GenerateAdditionalSetterModifier(property)}{setOrInit} => SetValue({GenerateDependencyPropertyName(property)}, value);");
        }
    }


    private static void GenerateCoercePartialMethod(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (!property.ValidationAndCallbacks.Coerce)
        {
            return;
        }

        if (property.Modifiers.IsAttached)
        {
            writer.AppendLine($"private static partial {GenerateType(property)} Coerce{property.Name}({GenerateBrowsableForType(property)} {GenerateBrowsableForTypeParameterName(property)}, {GenerateType(property, canBeNull: true)} value);");
        }
        else
        {
            writer.AppendLine($"private partial {GenerateType(property)} Coerce{property.Name}({GenerateType(property, canBeNull: true)} value);");
        }
    }


}

