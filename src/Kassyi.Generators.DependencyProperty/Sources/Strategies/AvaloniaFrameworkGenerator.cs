using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal sealed class AvaloniaFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.FrameworkMetadata.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.FrameworkMetadata.DefaultBindingMode;

        if (!property.IsDirect)
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
        var nameArgument = property.IsAddOwner ? "" : $"name: \"{property.Name}\",\n                                                                         ";
        return $"""
                                {nameArgument}getter: static sender => sender.{property.Name},
                                setter: {(property.IsReadOnly ? "null" : $"static (sender, value) => sender.{property.Name} = value")},
                                unsetValue: {SourceGenerationHelper.GenerateDefaultValue(property)},
                                defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                enableDataValidation: {(property.ValidationAndCallbacks.EnableDataValidation ? "true" : "false")}
                """;

    }

    public override string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property)
    {
        var arguments = property.IsDirect
            ? GenerateRegisterMethodArguments(@class, property)
            : GeneratePropertyMetadata(@class, property);

        return $"""
                        {property.ComponentModel.FromType}.{property.Name}Property.AddOwner<{@class.Type}>(
                            {arguments});
            """;
    }

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        return property.IsDirect
            ? $"RegisterDirect<{@class.Type}, {SourceGenerationHelper.GenerateType(property)}>"
            :
            property.IsAttached
                ? $"RegisterAttached<{@class.Type}, {SourceGenerationHelper.GenerateBrowsableForType(property)}, {SourceGenerationHelper.GenerateType(property)}>"
                : $"Register<{@class.Type}, {SourceGenerationHelper.GenerateType(property)}>";
    }

    public override void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var defaultValue = SourceGenerationHelper.GenerateDefaultValue(property);
        var defaultBindingMode = property.FrameworkMetadata.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.FrameworkMetadata.DefaultBindingMode;

        if (property.IsDirect)
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
        return property.IsDirect
            ? SourceGenerationHelper.GenerateTypeByPlatform(
                property.Framework,
                $"DirectProperty<{@class.Type}, {SourceGenerationHelper.GenerateType(property)}>")
            : property.IsAttached
                ? SourceGenerationHelper.GenerateTypeByPlatform(
                    property.Framework,
                    $"AttachedProperty<{SourceGenerationHelper.GenerateType(property)}>")
                : SourceGenerationHelper.GenerateTypeByPlatform(
                    property.Framework,
                    $"StyledProperty<{SourceGenerationHelper.GenerateType(property)}>");
    }

    public override string GenerateManagerType(ClassData @class) =>
        SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "AvaloniaProperty");

    public override void GenerateAdditionalFieldForDirectProperties(
        ref SourceWriter writer,
        DependencyPropertyData property)
    {
        if (!property.IsDirect)
        {
            writer.Append(" ");
            return;
        }

        writer.Append($"        private {SourceGenerationHelper.GenerateType(property)} _{property.Name.ToParameterName()} = {SourceGenerationHelper.GenerateDefaultValue(property)};");
        writer.AppendLine();
    }

    public override void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event) =>
        GenerateRoutedEventInternal(ref writer, @class, @event);

    public override void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event) =>
        GenerateAttachedRoutedEventInternal(ref writer, @class, @event);

    protected override void GenerateHandlerAction(ref SourceWriter writer, EventData @event, string uiElementType, string contentElementType, string action)
    {
        writer.AppendLine($$"""
            if (element is {{uiElementType}} uiElement)
            {
                uiElement.{{action}}({{@event.Name}}Event, handler);
            }
""");
    }

    protected override string GenerateRoutedEventType(ClassData @class) => $"{SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent")}<{GenerateRoutedEventArgsType(@class)}>";
    protected override string GenerateRoutedEventArgsType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEventArgs");
    protected override string GenerateRoutedEventHandlerType(ClassData @class) => $"global::System.EventHandler<{GenerateRoutedEventArgsType(@class)}>";
    protected override string GenerateRoutingStrategyType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutingStrategies");
    protected override string GenerateEventManagerType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent");
    protected override string GenerateRegisterRoutedEventMethod(ClassData @class) => $"Register<{@class.Type}, {GenerateRoutedEventArgsType(@class)}>";
    
    protected override string GenerateRegisterRoutedEventMethodArguments(ClassData @class, EventData @event) => $"""

                                                   name: "{@event.Name}",
                                                   routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy}
                                   """;

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
            foreach (var property in properties.OrderBy(static p => p.IsAttached))
            {
                GenerateAvaloniaStaticConstructorPropertyChanged(ref tempWriter, @class, property);
            }

            if (tempWriter.Length == 0)
            {
                return;
            }

            writer.AppendLine($$"""

            #nullable enable

            namespace {{@class.Namespace}}
            {
                {{SourceGenerationHelper.GenerateModifiers(@class)}}partial class {{@class.Name}}
                {
                    static {{@class.Name}}()
                    {
            """);
            
            writer.Append(tempWriter.ToString());

            writer.AppendLine("""
                    }
                }
            }
            """);
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
        writer.LineIf(property.FrameworkMetadata.AffectsRender, $"            AffectsRender<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.FrameworkMetadata.AffectsMeasure, $"            AffectsMeasure<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.FrameworkMetadata.AffectsArrange, $"            AffectsArrange<{@class.Type}>({property.Name}Property);");
    }

    private static void GenerateAvaloniaStaticConstructorPropertyChanged(
        ref SourceWriter writer,
        ClassData @class,
        DependencyPropertyData property)
    {
        var (name, callbacks) 
            = SourceGenerationHelper.CheckOnChangedMethods(@class, property);
        
        if (callbacks is { IsChanged0: false, IsChanged1: false, IsChanged2: false, IsChanged3: false, IsChangedArgs1: false, IsChangedArgs2: false })
        {
            return;
        }
        
        writer.AppendLine($"            {property.Name}Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<{SourceGenerationHelper.GenerateType(property)}>>(static x =>");
        writer.AppendLine("            {");

        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;
            
        var instanceCast = property.IsAttached ? "" : $"(({@class.Type})x.Sender).";
        var senderCast = $"({senderType})x.Sender";
        var typeCast = $"({SourceGenerationHelper.GenerateType(property)})";
        
        writer.LineIf(callbacks.IsChanged0, $"                {instanceCast}{name}();");
        writer.LineIf(callbacks.IsChanged1, $"""
                            {instanceCast}{name}(
                                {(property.IsAttached ? senderCast : $"{typeCast}x.NewValue.GetValueOrDefault()")});
            """);
        writer.LineIf(callbacks.IsChanged2, $"""
                            {instanceCast}{name}(
                                {(property.IsAttached ? senderCast : $"{typeCast}x.OldValue.GetValueOrDefault()")},
                                {typeCast}x.NewValue.GetValueOrDefault());
            """);
        writer.LineIf(callbacks.IsChanged3, $"""
                            {instanceCast}{name}(
                                {senderCast},
                                {typeCast}x.OldValue.GetValueOrDefault(),
                                {typeCast}x.NewValue.GetValueOrDefault());
            """);
        writer.LineIf(callbacks.IsChangedArgs1, $"""
                            {instanceCast}{name}(
                                x);
            """);
        writer.LineIf(callbacks.IsChangedArgs2, $"""
                            {instanceCast}{name}(
                                {senderCast},
                                x);
            """);

        writer.AppendLine("            }));");
    }
}
