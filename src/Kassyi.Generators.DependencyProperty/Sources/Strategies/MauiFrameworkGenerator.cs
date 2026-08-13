using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal sealed class MauiFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.FrameworkMetadata.DefaultBindingMode is null or "Default"
            ? property.IsReadOnly
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

    public static string GeneratePropertyChangingCallback(ClassData @class, DependencyPropertyData property)
    {
        if (property.ValidationAndCallbacks.Callbacks is { IsChanging0: false, IsChanging1: false, IsChanging2: false, IsChanging3: false })
        {
            return "null";
        }

        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;

        var name = property.Name;
        var senderCast = $"({senderType})sender";
        var instanceCast = $"(({senderType})sender)";
        var typeCast = $"({SourceGenerationHelper.GenerateType(property)})";
        var isAttached = property.IsAttached;
        
        return $$"""
            static (sender, oldValue, newValue) =>
                            {
                                {{(property.ValidationAndCallbacks.Callbacks.IsChanging0 ? GenerateCall($"On{name}Changing", isAttached, instanceCast) : "")}}
                                {{(property.ValidationAndCallbacks.Callbacks.IsChanging1 ? GenerateCall($"On{name}Changing", isAttached, instanceCast, isAttached ? [senderCast] : [$"{typeCast}newValue"]) : "")}}
                                {{(property.ValidationAndCallbacks.Callbacks.IsChanging2 ? GenerateCall($"On{name}Changing", isAttached, instanceCast, isAttached ? [senderCast, $"{typeCast}newValue"] : [$"{typeCast}oldValue", $"{typeCast}newValue"]) : "")}}
                                {{(property.ValidationAndCallbacks.Callbacks.IsChanging3 ? GenerateCall($"On{name}Changing", isAttached, instanceCast, senderCast, $"{typeCast}oldValue", $"{typeCast}newValue") : "")}}
                            }
            """;
    }

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        return property.IsAttached ? property.IsReadOnly ? "CreateAttachedReadOnly" : "CreateAttached" :
            property.IsReadOnly ? "CreateReadOnly" : "Create";
    }

    public override void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        // MAUI does not use a separate PropertyMetadata object in this way.
    }

    protected override string PropertyChangedCallbackSignature => "static (sender, oldValue, newValue) =>";
    protected override string NewValueExpression => "newValue";
    protected override string OldValueExpression => "oldValue";
    protected override string GenerateArgsExpression(DependencyPropertyData property) => $"new global::Microsoft.Maui.Controls.DependencyPropertyChangedEventArgs(({SourceGenerationHelper.GenerateType(property)})oldValue, ({SourceGenerationHelper.GenerateType(property)})newValue)";
    protected override string CoerceAttachedCallbackSignature => "static (sender, value) =>";
    protected override string CoerceAttachedValueExpression => "value";

    protected override string GenerateValidateValueCallbackInternal(ClassData @class, DependencyPropertyData property)
    {
        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;

        return $"""
               static (sender, value) =>
                                       Is{property.Name}Valid(
                                           ({senderType})sender,
                                           ({SourceGenerationHelper.GenerateType(property, canBeNull: true)})value)
               """;
    }

    public override string GeneratePropertyType(ClassData @class, DependencyPropertyData property)
    {
        return SourceGenerationHelper.GenerateTypeByPlatform(
            property.Framework,
            property.IsReadOnly
                ? "BindablePropertyKey"
                : "BindableProperty");
    }

    public override void GenerateAdditionalPropertyForReadOnlyProperties(
        ref SourceWriter writer,
        DependencyPropertyData property)
    {
        if (!property.IsReadOnly)
        {
            writer.Append(" ");
            return;
        }

        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.XmlDocumentation, property, isProperty: false);
        writer.AppendLine($"        public static readonly {SourceGenerationHelper.GenerateTypeByPlatform(property.Framework, "BindableProperty")} {property.Name}Property");
        writer.AppendLine($"            = {SourceGenerationHelper.GenerateDependencyPropertyName(property)}.BindableProperty;");
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

        writer.AppendLine($$"""

        #nullable enable

        namespace {{@class.Namespace}}
        {
            {{SourceGenerationHelper.GenerateModifiers(@class)}}partial class {{@class.Name}}
            {
        """);
        writer.AppendLine($$"""
        private{{modifiers}} global::Microsoft.Maui.WeakEventManager {{@event.Name}}WeakEventManager { get; } = new global::Microsoft.Maui.WeakEventManager();

""");
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        writer.AppendLine($$"""
        public{{modifiers}} event {{eventHandlerType}} {{@event.Name}}
        {
            add => {{@event.Name}}WeakEventManager.AddEventHandler(value);
            remove => {{@event.Name}}WeakEventManager.RemoveEventHandler(value);
        }

        /// <summary>
        /// A helper method to raise the {{@event.Name}} event.
        /// </summary>
        internal{{modifiers}} void Raise{{@event.Name}}Event(object? sender{{additionalParameters}})
        {
            {{@event.Name}}WeakEventManager.HandleEvent(sender!, {{args}}!, eventName: nameof({{@event.Name}}));
        }
    }
}
""");
    }

    public override string GenerateManagerType(ClassData @class) =>
        SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "BindableProperty");
}
