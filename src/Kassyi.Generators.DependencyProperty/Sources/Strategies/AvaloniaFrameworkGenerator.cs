using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Implements dependency property and routed event source generation for the Avalonia framework.</summary>
internal sealed class AvaloniaFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.FrameworkMetadata.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.FrameworkMetadata.DefaultBindingMode;

        if (!property.Modifiers.IsDirect)
        {
            return $"""
                name: "{property.Name}",
                defaultValue: {SourceGenerationHelper.GenerateDefaultValue(property)},
                inherits: {(property.FrameworkMetadata.Inherits ? "true" : "false")},
                defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                validate: {GenerateValidateValueCallback(@class, property)},
                coerce: {GenerateCoerceValueCallback(@class, property)}
                """;
        }
        var nameArgument = property.Modifiers.IsAddOwner ? "" : $"name: \"{property.Name}\", ";
        return $"""
            {nameArgument}getter: static sender => sender.{property.Name},
            setter: {(property.Modifiers.IsReadOnly ? "null" : $"static (sender, value) => sender.{property.Name} = value")},
            unsetValue: {SourceGenerationHelper.GenerateDefaultValue(property)},
            defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
            enableDataValidation: {(property.ValidationAndCallbacks.EnableDataValidation ? "true" : "false")}
            """;
    }

    public override string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property)
    {
        var arguments = property.Modifiers.IsDirect
            ? GenerateRegisterMethodArguments(@class, property)
            : GeneratePropertyMetadata(@class, property);

        return $"{property.ComponentModel.FromType}.{property.Name}Property.AddOwner<{@class.Type}>({arguments});";
    }

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        var type = SourceGenerationHelper.GenerateType(property);
        return property switch
        {
            { Modifiers.IsDirect: true } => $"RegisterDirect<{@class.Type}, {type}>",
            { Modifiers.IsAttached: true } => $"RegisterAttached<{@class.Type}, {SourceGenerationHelper.GenerateBrowsableForType(property)}, {type}>",
            _ => $"Register<{@class.Type}, {type}>",
        };
    }

    public override void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var defaultValue = SourceGenerationHelper.GenerateDefaultValue(property);
        var defaultBindingMode = property.FrameworkMetadata.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.FrameworkMetadata.DefaultBindingMode;

        if (property.Modifiers.IsDirect)
        {
            writer.Append($"""
                {parameterName}new global::Avalonia.Data.Core.TargetNullValuePropertyMetadata<{SourceGenerationHelper.GenerateType(property)}>(
                    unsetValue: {defaultValue},
                    defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                    enableDataValidation: {(property.ValidationAndCallbacks.EnableDataValidation ? "true" : "false")})
                """);
        }
        else
        {
            var metadataType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, $"StyledPropertyMetadata<{property.Type}>");
            writer.Append($"""
                {parameterName}new {metadataType}(
                    defaultValue: {defaultValue},
                    defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode})
                """);
        }
    }

    public override string GeneratePropertyType(ClassData @class, DependencyPropertyData property)
    {
        var type = SourceGenerationHelper.GenerateType(property);
        var propertyKind = property switch
        {
            { Modifiers.IsDirect: true } => $"DirectProperty<{@class.Type}, {type}>",
            { Modifiers.IsAttached: true } => $"AttachedProperty<{type}>",
            _ => $"StyledProperty<{type}>",
        };

        return SourceGenerationHelper.GenerateTypeByPlatform(property.Framework, propertyKind);
    }

    public override string GenerateManagerType(ClassData @class) =>
        SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "AvaloniaProperty");

    public override void GenerateAdditionalFieldForDirectProperties(
        ref SourceWriter writer,
        DependencyPropertyData property)
    {
        if (!property.Modifiers.IsDirect)
        {
            return;
        }

        writer.AppendLine($"private {SourceGenerationHelper.GenerateType(property)} _{property.Name.ToParameterName()} = {SourceGenerationHelper.GenerateDefaultValue(property)};");
    }

    public override void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event) =>
        GenerateRoutedEventInternal(ref writer, @class, @event);

    public override void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event) =>
        GenerateAttachedRoutedEventInternal(ref writer, @class, @event);

    protected override void GenerateHandlerAction(ref SourceWriter writer, EventData @event, string uiElementType, string contentElementType, string action)
    {
        using (writer.Scope($"if (element is {uiElementType} uiElement)"))
        {
            writer.AppendLine($"uiElement.{action}({@event.Name}Event, handler);");
        }
    }

    protected override string GenerateRoutedEventType(ClassData @class) => $"{SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent")}<{GenerateRoutedEventArgsType(@class)}>";
    protected override string GenerateRoutedEventArgsType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEventArgs");
    protected override string GenerateRoutedEventHandlerType(ClassData @class) => $"global::System.EventHandler<{GenerateRoutedEventArgsType(@class)}>";
    protected override string GenerateRoutingStrategyType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutingStrategies");
    protected override string GenerateEventManagerType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent");
    protected override string GenerateRegisterRoutedEventMethod(ClassData @class) => $"Register<{@class.Type}, {GenerateRoutedEventArgsType(@class)}>";
    
    protected override string GenerateRegisterRoutedEventMethodArguments(ClassData @class, EventData @event) =>
        $"name: \"{@event.Name}\", routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy}";

    public override void GenerateStaticConstructor(
        ref SourceWriter writer,
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> properties)
    {
        var tempWriter = new SourceWriter();
        try
        {
            foreach (var property in properties)
            {
                GenerateAvaloniaStaticConstructorAffects(ref tempWriter, @class, property);
            }
            foreach (var property in properties.OrderBy(static p => p.Modifiers.IsAttached))
            {
                GenerateAvaloniaStaticConstructorPropertyChanged(ref tempWriter, @class, property);
            }

            if (tempWriter.Length == 0)
            {
                return;
            }

            using var _ = writer.ClassScope(@class);
            using (writer.Scope($"static {@class.Name}()"))
            {
                writer.Append(tempWriter.ToString());
            }
        }
        finally
        {
            tempWriter.Dispose();
        }
    }

    private static void GenerateAvaloniaStaticConstructorAffects(
        ref SourceWriter writer,
        ClassData @class,
        DependencyPropertyData property)
    {
        writer.LineIf(property.FrameworkMetadata.AffectsRender, $"AffectsRender<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.FrameworkMetadata.AffectsMeasure, $"AffectsMeasure<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.FrameworkMetadata.AffectsArrange, $"AffectsArrange<{@class.Type}>({property.Name}Property);");
    }

    private static void GenerateAvaloniaStaticConstructorPropertyChanged(
        ref SourceWriter writer,
        ClassData @class,
        DependencyPropertyData property)
    {
        if (SourceGenerationHelper.CheckOnChangedMethods(@class, property) is not (var name, { ChangedSignatures: not CallbackSignature.None and var signatures }))
        {
            return;
        }

        var propertyType = SourceGenerationHelper.GenerateType(property);
        var observerType = $"global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<{propertyType}>>";
        using (writer.Scope($"{property.Name}Property.Changed.Subscribe(new {observerType}(static x =>", "}));"))
        {
            writer.AppendLine("#pragma warning disable CS8600, CS8604");
            var senderType = property.Modifiers.IsAttached
                ? SourceGenerationHelper.GenerateBrowsableForType(property)
                : @class.Type;
                
            var senderCast = $"({senderType})x.Sender";
            var instanceCast = $"(({@class.Type})x.Sender)";
            var typeCast = $"({propertyType})";
            var isAttached = property.Modifiers.IsAttached;
            
            var oldVal = $"{typeCast}x.OldValue.GetValueOrDefault()";
            var newVal = $"{typeCast}x.NewValue.GetValueOrDefault()";

            GenerateCallbackCalls(writer, signatures, name, isAttached, senderCast, instanceCast, oldVal, newVal, "x");
            writer.AppendLine("#pragma warning restore CS8600, CS8604");
        }
    }
}
