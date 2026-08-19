using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Implements dependency property and routed event source generation for UWP, WinUI, and Uno frameworks.</summary>
internal sealed class UwpFrameworkGenerator : FrameworkGenerator
{
    public override string GeneratePropertyChangedCallback(ClassData @class, DependencyPropertyData property)
    {
        var baseCallback = base.GeneratePropertyChangedCallback(@class, property);
        return !property.ValidationAndCallbacks.Coerce 
            ? baseCallback : GenerateCoercionWrappingCallback(@class, property, baseCallback);
    }

    private static string GenerateCoercionWrappingCallback(
        ClassData @class,
        DependencyPropertyData property,
        string baseCallback)
    {
        var senderType = property.Modifiers.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;
        var propertyType = SourceGenerationHelper.GenerateType(property);
        var propCallbackType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "PropertyChangedCallback");

        var senderCast = $"({senderType})sender";
        var senderInstance = $"(({senderType})sender)";
        var rawNewValue = $"({SourceGenerationHelper.GenerateType(property, canBeNull: true)})args.NewValue";
        var typedNewValue = $"({propertyType})args.NewValue";

        var coerceCall = property.Modifiers.IsAttached
            ? $"Coerce{property.Name}({senderCast}, {rawNewValue})"
            : $"{senderInstance}.Coerce{property.Name}({rawNewValue})";

        var equalityCheck = $"if (!global::System.Collections.Generic.EqualityComparer<{propertyType}>.Default.Equals({typedNewValue}, coercedValue))";

        using var writer = new SourceWriter();
        using (writer.Scope("static (sender, args) =>"))
        {
            writer.AppendLine($"var coercedValue = {coerceCall};");
            using (writer.Scope(equalityCheck))
            {
                writer.AppendLine($"{senderInstance}.SetValue({property.Name}Property, coercedValue);");
                writer.AppendLine("return;");
            }

            if (baseCallback == "null")
            {
                return writer.ToString();
            }

            writer.AppendLine($"var callback = new {propCallbackType}({baseCallback});");
            writer.AppendLine("callback(sender, args);");
        }

        return writer.ToString();
    }

    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property) => $"""
        name: "{property.Name}",
        propertyType: typeof({property.Type}),
        ownerType: typeof({@class.Type}),
        {FrameworkGeneratorFactory.Create(@class.Framework).GeneratePropertyMetadata(@class, property)}
        """;

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property) =>
        property.Modifiers.IsAttached ? "RegisterAttached" : "Register";

    public override void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var defaultValue = SourceGenerationHelper.GenerateDefaultValue(property);
        var propertyChanged = GeneratePropertyChangedCallback(@class, property);
        var type = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "PropertyMetadata");

        if (property.Modifiers.IsAttached)
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

    public override void GenerateSetter(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        var setMethodName = property.Type.GetXamlBindingHelperSetMethodName();
        if (setMethodName == null)
        {
            base.GenerateSetter(ref writer, @class, property);
            return;
        }

        var setOrInit = property.Modifiers.IsInitOnly ? "init" : "set";
        var modifier = SourceGenerationHelper.GenerateAdditionalSetterModifier(property);
        var propName = SourceGenerationHelper.GenerateDependencyPropertyName(property);
        var helperType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Markup.XamlBindingHelper");

        if (setMethodName == "SetPropertyFromString")
        {
            using (writer.Scope($"{modifier}{setOrInit}"))
            {
                using (writer.Scope("if (value is null || value.Length == 0)"))
                {
                    writer.AppendLine($"SetValue({propName}, value);");
                }
                using (writer.Scope("else"))
                {
                    writer.AppendLine($"{helperType}.SetPropertyFromString(this, {propName}, value);");
                }
            }
        }
        else
        {
            writer.AppendLine($"{modifier}{setOrInit} => {helperType}.{setMethodName}(this, {propName}, value);");
        }
    }

    public override void GenerateAttachedSetterBody(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string dependencyPropertyName)
    {
        var setMethodName = property.Type.GetXamlBindingHelperSetMethodName();
        if (setMethodName == null)
        {
            base.GenerateAttachedSetterBody(ref writer, @class, property, dependencyPropertyName);
            return;
        }

        var helperType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Markup.XamlBindingHelper");

        if (setMethodName == "SetPropertyFromString")
        {
            using (writer.Scope("if (value is null || value.Length == 0)"))
            {
                writer.AppendLine($"element.SetValue({dependencyPropertyName}, value);");
            }
            using (writer.Scope("else"))
            {
                writer.AppendLine($"{helperType}.SetPropertyFromString(element, {dependencyPropertyName}, value);");
            }
        }
        else
        {
            writer.AppendLine($"{helperType}.{setMethodName}(element, {dependencyPropertyName}, value);");
        }
    }
}
