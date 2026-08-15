using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal sealed class UwpFrameworkGenerator : FrameworkGenerator
{
    public override string GeneratePropertyChangedCallback(ClassData @class, DependencyPropertyData property)
    {
        var baseCallback = base.GeneratePropertyChangedCallback(@class, property);
        if (!property.ValidationAndCallbacks.Coerce)
        {
            return baseCallback;
        }

        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;
        var propertyType = SourceGenerationHelper.GenerateType(property);
        var propCallbackType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "PropertyChangedCallback");
        
        using var writer = new SourceWriter();
        writer.AppendLine("static (sender, args) =>");
        writer.AppendLine("{");
        using (writer.Scope())
        {
            var senderExpr = property.IsAttached ? $"({senderType})sender" : $"(({senderType})sender)";
            var argsValue = $"({SourceGenerationHelper.GenerateType(property, canBeNull: true)})args.NewValue";
            var coerceCall = property.IsAttached
                ? $"Coerce{property.Name}({senderExpr}, {argsValue})"
                : $"{senderExpr}.Coerce{property.Name}({argsValue})";

            writer.AppendLine($"var coercedValue = {coerceCall};");
            writer.AppendLine($"if (!global::System.Collections.Generic.EqualityComparer<{propertyType}>.Default.Equals(({propertyType})args.NewValue, coercedValue))");
            using (writer.Scope())
            {
                writer.AppendLine($"{senderExpr}.SetValue({property.Name}Property, coercedValue);");
                writer.AppendLine("return;");
            }
            if (baseCallback != "null")
            {
                writer.AppendLine($"var callback = new {propCallbackType}({baseCallback});");
                writer.AppendLine("callback(sender, args);");
            }
        }
        writer.Append("}");
        return writer.ToString();
    }

    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property) => $"""
        name: "{property.Name}",
        propertyType: typeof({property.Type}),
        ownerType: typeof({@class.Type}),
        {FrameworkGeneratorFactory.Create(@class.Framework).GeneratePropertyMetadata(@class, property)}
        """;

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
                // [WHY] Workaround for NotImplementedException: PropertyMetadata.Create(defaultValue, callback) is not implemented in Uno.
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
