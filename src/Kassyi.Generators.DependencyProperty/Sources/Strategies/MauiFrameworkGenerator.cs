using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal sealed class MauiFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.FrameworkMetadata.DefaultBindingMode is null or "Default"
            ? property.Modifiers.IsReadOnly
                ? "OneWayToSource"
                : "OneWay"
            : property.FrameworkMetadata.DefaultBindingMode;

        return $"""
            propertyName: "{property.Name}",
            returnType: typeof({property.Type}),
            declaringType: typeof({@class.Type}),
            defaultValue: {SourceGenerationHelper.GenerateDefaultValue(property)},
            defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.{defaultBindingMode},
            validateValue: {GenerateValidateValueCallback(@class, property)},
            propertyChanged: {GeneratePropertyChangedCallback(@class, property)},
            propertyChanging: {GeneratePropertyChangingCallback(@class, property)},
            coerceValue: {GenerateCoerceValueCallback(@class, property)},
            defaultValueCreator: {GenerateCreateDefaultValueCallbackValueCallback(property)}
            """;
    }

    public string GeneratePropertyChangingCallback(ClassData @class, DependencyPropertyData property)
    {
        var signatures = property.ValidationAndCallbacks.Callbacks.ChangingSignatures;
        return signatures == CallbackSignature.None
            ? "null"
            : GenerateCallbackInternal(@class, property, $"On{property.Name}Changing", signatures, PropertyChangedCallbackSignature);
    }

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        return property.Modifiers.IsAttached ? property.Modifiers.IsReadOnly ? "CreateAttachedReadOnly" : "CreateAttached" :
            property.Modifiers.IsReadOnly ? "CreateReadOnly" : "Create";
    }

    public override void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        // [WHY] MAUI does not use a separate PropertyMetadata object in this way.
    }

    protected override string PropertyChangedCallbackSignature => "static (sender, oldValue, newValue) =>";
    protected override string NewValueExpression => "newValue";
    protected override string OldValueExpression => "oldValue";
    protected override string GenerateArgsExpression(DependencyPropertyData property) => $"new global::Microsoft.Maui.Controls.DependencyPropertyChangedEventArgs(({SourceGenerationHelper.GenerateType(property)})oldValue, ({SourceGenerationHelper.GenerateType(property)})newValue)";
    protected override string CoerceAttachedCallbackSignature => "static (sender, value) =>";
    protected override string CoerceAttachedValueExpression => "value";

    protected override string GenerateValidateValueCallbackInternal(ClassData @class, DependencyPropertyData property)
    {
        var senderType = property.Modifiers.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;

        return $"static (sender, value) => Is{property.Name}Valid(({senderType})sender, ({SourceGenerationHelper.GenerateType(property, canBeNull: true)})value)";
    }

    public override string GeneratePropertyType(ClassData @class, DependencyPropertyData property)
    {
        return SourceGenerationHelper.GenerateTypeByPlatform(
            property.Framework,
            property.Modifiers.IsReadOnly
                ? "BindablePropertyKey"
                : "BindableProperty");
    }

    public override void GenerateAdditionalPropertyForReadOnlyProperties(
        ref SourceWriter writer,
        DependencyPropertyData property)
    {
        if (!property.Modifiers.IsReadOnly)
        {
            return;
        }

        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.XmlDocumentation, property, isProperty: false);
        writer.AppendLine($"public static readonly {SourceGenerationHelper.GenerateTypeByPlatform(property.Framework, "BindableProperty")} {property.Name}Property");
        writer.AppendLine($"= {SourceGenerationHelper.GenerateDependencyPropertyName(property)}.BindableProperty;");
    }

    public override void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var additionalParameters = string.IsNullOrWhiteSpace(@event.Type)
            ? string.Empty
            : $", {SourceGenerationHelper.GenerateEventArgsType(@event)} args";
        var args = string.IsNullOrWhiteSpace(@event.Type)
            ? "System.EventArgs.Empty".WithGlobalPrefix()
            : "args";
        var modifiers = @event.IsAttached
            ? " static"
            : string.Empty;

        var nullable = !@event.Type.Contains("EventArgs");
        var eventHandlerType = GenerateEventHandlerType(@event, nullable: nullable, nullableType: nullable);

        using var _ = writer.ClassScope(@class);
        writer.AppendLine($"private{modifiers} global::Microsoft.Maui.WeakEventManager {@event.Name}WeakEventManager {{ get; }} = new global::Microsoft.Maui.WeakEventManager();");
        writer.AppendLine();
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        using (writer.Scope($"public{modifiers} event {eventHandlerType} {@event.Name}"))
        {
            writer.AppendLine($"add => {@event.Name}WeakEventManager.AddEventHandler(value);");
            writer.AppendLine($"remove => {@event.Name}WeakEventManager.RemoveEventHandler(value);");
        }
        writer.AppendLine();
        writer.AppendLine("/// <summary>");
        writer.AppendLine($"/// A helper method to raise the {@event.Name} event.");
        writer.AppendLine("/// </summary>");
        using (writer.Scope($"internal{modifiers} void Raise{@event.Name}Event(object? sender{additionalParameters})"))
        {
            writer.AppendLine($"{@event.Name}WeakEventManager.HandleEvent(sender!, {args}!, eventName: nameof({@event.Name}));");
        }
    }

    public override string GenerateManagerType(ClassData @class) =>
        SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "BindableProperty");
}
