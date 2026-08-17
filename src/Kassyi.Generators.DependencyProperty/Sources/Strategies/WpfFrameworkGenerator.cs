using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal sealed class WpfFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property) => $"""
        name: "{property.Name}",
        propertyType: typeof({property.Type}),
        ownerType: typeof({@class.Type}),
        {GeneratePropertyMetadata(@class, property)},
        validateValueCallback: {GenerateValidateValueCallback(@class, property)}
        """;

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        return property is { IsReadOnly: true }
            ? property.IsAttached
                ? "RegisterAttachedReadOnly"
                : "RegisterReadOnly"
            : property.IsAttached ? "RegisterAttached" : "Register";
    }

    public override void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var defaultValue = SourceGenerationHelper.GenerateDefaultValue(property);
        var flags = SourceGenerationHelper.GenerateOptions(property);
        var propertyChanged = GeneratePropertyChangedCallback(@class, property);
        var coerceValue = GenerateCoerceValueCallback(@class, property);
        var isAnimationProhibited = property.FrameworkMetadata.IsAnimationProhibited.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);

        var updateSourceTrigger = property.FrameworkMetadata.DefaultUpdateSourceTrigger is null
            ? string.Empty
            : $",\ndefaultUpdateSourceTrigger: global::System.Windows.Data.UpdateSourceTrigger.{property.FrameworkMetadata.DefaultUpdateSourceTrigger}";

        writer.Append($"""
            {parameterName}new global::System.Windows.FrameworkPropertyMetadata(
                defaultValue: {defaultValue},
                flags: {flags},
                propertyChangedCallback: {propertyChanged},
                coerceValueCallback: {coerceValue},
                isAnimationProhibited: {isAnimationProhibited}{updateSourceTrigger})
            """);
    }

    public override string GeneratePropertyType(ClassData @class, DependencyPropertyData property)
    {
        return SourceGenerationHelper.GenerateTypeByPlatform(
            property.Framework,
            property.IsReadOnly
                ? "DependencyPropertyKey"
                : "DependencyProperty");
    }

    public override string GenerateManagerType(ClassData @class) =>
        SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "DependencyProperty");

    public override void GenerateAdditionalPropertyForReadOnlyProperties(
        ref SourceWriter writer,
        DependencyPropertyData property)
    {
        if (!property.IsReadOnly)
        {
            return;
        }

        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation.XmlDocumentation, property, isProperty: false);
        writer.AppendLine($"public static readonly {SourceGenerationHelper.GenerateTypeByPlatform(property.Framework, "DependencyProperty")} {property.Name}Property");
        writer.AppendLine($"= {SourceGenerationHelper.GenerateDependencyPropertyName(property)}.DependencyProperty;");
    }

    public override void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event) =>
        GenerateRoutedEventInternal(ref writer, @class, @event);

    public override void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event) =>
        GenerateAttachedRoutedEventInternal(ref writer, @class, @event);

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
        var source = @event.IsAttached
            ? @class.Name
            : $"(source as {@class.Name})!";

        var eventHandlerType = GenerateEventHandlerType(@event);
        var eventArgsType = SourceGenerationHelper.GenerateEventArgsType(@event);

        using var _ = writer.ClassScope(@class);
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        
        GenerateWeakEventManagerClass(ref writer, @event.Name, eventHandlerType, eventArgsType, source);

        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        using (writer.Scope($"public{modifiers} event {eventHandlerType} {@event.Name}"))
        {
            writer.AppendLine($"add => {@event.Name}WeakEventManager.AddHandler(null, value);");
            writer.AppendLine($"remove => {@event.Name}WeakEventManager.RemoveHandler(null, value);");
        }
        writer.AppendLine();
        writer.AppendLine("/// <summary>");
        writer.AppendLine($"/// A helper method to raise the {@event.Name} event.");
        writer.AppendLine("/// </summary>");
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        using (writer.Scope($"internal{modifiers} void Raise{@event.Name}Event(object? sender{additionalParameters})"))
        {
            writer.AppendLine($"{@event.Name}WeakEventManager.CurrentManager.On{@event.Name}(sender, {args});");
        }
    }

    private static void GenerateWeakEventManagerClass(
        ref SourceWriter writer, 
        string eventName, 
        string eventHandlerType, 
        string eventArgsType, 
        string source)
    {
        var deliverEventArg = eventArgsType == "global::System.EventArgs"
            ? "args"
            : "args as object as global::System.EventArgs ?? global::System.EventArgs.Empty";

        writer.AppendLine($$"""
            private class {{eventName}}WeakEventManager : global::System.Windows.WeakEventManager
            {
            private {{eventName}}WeakEventManager()
            {
            }
            public static void AddHandler(object? source, {{eventHandlerType}} handler)
            {
            if (source == null) throw new global::System.ArgumentNullException(nameof(source));
            if (handler == null) throw new global::System.ArgumentNullException(nameof(handler));
            CurrentManager.ProtectedAddHandler(source, handler);
            }
            public static void RemoveHandler(object? source, {{eventHandlerType}} handler)
            {
            if (source == null) throw new global::System.ArgumentNullException(nameof(source));
            if (handler == null) throw new global::System.ArgumentNullException(nameof(handler));
            CurrentManager.ProtectedRemoveHandler(source, handler);
            }
            internal static {{eventName}}WeakEventManager CurrentManager
            {
            get
            {
            var managerType = typeof({{eventName}}WeakEventManager);
            var manager = ({{eventName}}WeakEventManager)GetCurrentManager(managerType);
            if (manager == null)
            {
            manager = new {{eventName}}WeakEventManager();
            SetCurrentManager(managerType, manager);
            }
            return manager;
            }
            }
            protected override void StartListening(object? source)
            {
            {{source}}.{{eventName}} += On{{eventName}};
            }
            protected override void StopListening(object? source)
            {
            {{source}}.{{eventName}} -= On{{eventName}};
            }
            internal void On{{eventName}}(object? sender, {{eventArgsType}} args)
            {
            DeliverEvent(sender, {{deliverEventArg}});
            }
            }
            """);
    }

    public override void GenerateStaticConstructor(
        ref SourceWriter writer,
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> properties)
    {
        using var _ = writer.ClassScope(@class);
        using (writer.Scope($"static {@class.Name}()"))
        {
            foreach (var property in properties)
            {
                if (property.IsReadOnly)
                {
                    writer.AppendLine($"{property.Name}Property.OverrideMetadata(forType: typeof({@class.Type}), {GeneratePropertyMetadata(@class, property)}, key: {property.Name}PropertyKey);");
                }
                else
                {
                    writer.AppendLine($"{property.Name}Property.OverrideMetadata(forType: typeof({@class.Type}), {GeneratePropertyMetadata(@class, property)});");
                }
            }
        }
        writer.AppendLine();

        foreach (var property in properties)
        {
            SourceGenerationHelper.GenerateOnChangedMethods(ref writer, @class, property);
        }
    }
}
