using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal sealed class UwpFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        return $"""

                                          name: "{property.Name}",
                                          propertyType: typeof({property.Type}),
                                          ownerType: typeof({@class.Type}),
                                          {FrameworkGeneratorFactory.Create(@class.Framework).GeneratePropertyMetadata(@class, property)}
                          """;
    }

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property) =>
        property.IsAttached ? "RegisterAttached" : "Register";

    public override void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var defaultValue = SourceGenerationHelper.GenerateDefaultValue(property);
        var propertyChanged = GeneratePropertyChangedCallback(@class, property);
        var type = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "PropertyMetadata");

        if (property.IsAttached)
        {
            writer.Append($"""
                {parameterName}new {type}(
                                    defaultValue: {defaultValue},
                                    propertyChangedCallback: {propertyChanged})
                """);
        }
        else
        {
            if (property.ValidationAndCallbacks.CreateDefaultValueCallback)
            {
                var createDefaultValue = GenerateCreateDefaultValueCallback(property);
                writer.Append($"""
                    {parameterName}{type}.Create(
                                        createDefaultValueCallback: {createDefaultValue},
                                        propertyChangedCallback: {propertyChanged})
                    """);
            }
            else
            {
                // fix for NotImplementedException: The member PropertyMetadata PropertyMetadata.Create(object defaultValue, PropertyChangedCallback propertyChangedCallback) is not implemented in Uno.
                var create = @class.Framework switch
                {
                    Framework.Uno or Framework.UnoWinUi => $"new {type}",
                    _ => $"{type}.Create",
                };
                writer.Append($"""
                    {parameterName}{create}(
                                        defaultValue: {defaultValue},
                                        propertyChangedCallback: {propertyChanged})
                    """);
            }
        }
    }

    public override string GeneratePropertyType(ClassData @class, DependencyPropertyData property)
    {
        return SourceGenerationHelper.GenerateTypeByPlatform(
            property.Framework,
            "DependencyProperty");
    }

    public override string GenerateManagerType(ClassData @class) =>
        SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "DependencyProperty");
}
