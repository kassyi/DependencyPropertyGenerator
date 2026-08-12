using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal class AvaloniaFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.DefaultBindingMode;

        if (property.IsDirect)
        {
            var nameArgument = property.IsAddOwner ? "" : $"name: \"{property.Name}\",\n                                                                         ";
            return $"""
                                                                         {nameArgument}getter: static sender => sender.{property.Name},
                                                                         setter: {(property.IsReadOnly ? "null" : $"static (sender, value) => sender.{property.Name} = value")},
                                                                         unsetValue: {SourceGenerationHelper.GenerateDefaultValue(property)},
                                                                         defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                                                         enableDataValidation: {(property.EnableDataValidation ? "true" : "false")}
                                                         """;
        }

        return $"""

                                             name: "{property.Name}",
                                             defaultValue: {SourceGenerationHelper.GenerateDefaultValue(property)},
                                             inherits: {(property.Inherits ? "true" : "false")},
                                             defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                             validate: {GenerateValidateValueCallback(@class, property)},
                                             coerce: {GenerateCoerceValueCallback(@class, property)}
                             """;
    }

    public override string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property)
    {
        var arguments = property.IsDirect
            ? GenerateRegisterMethodArguments(@class, property)
            : GeneratePropertyMetadata(@class, property);

        return $"""
                        {property.FromType}.{property.Name}Property.AddOwner<{@class.Type}>(
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
        var defaultBindingMode = property.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.DefaultBindingMode;

        if (property.IsDirect)
        {
            writer.Append($"""
                {parameterName}new global::Avalonia.Data.Core.TargetNullValuePropertyMetadata<{SourceGenerationHelper.GenerateType(property)}>(
                                    unsetValue: {defaultValue},
                                    defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                    enableDataValidation: {(property.EnableDataValidation ? "true" : "false")})
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

    public override string GenerateManagerType(ClassData @class)
    {
        return SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "AvaloniaProperty");
    }

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

    public override void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        GenerateRoutedEventInternal(ref writer, @class, @event);
    }

    public override void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        GenerateAttachedRoutedEventInternal(ref writer, @class, @event);
    }

    protected override void GenerateHandlerAction(ref SourceWriter writer, EventData @event, string uiElementType, string contentElementType, string action)
    {
        writer.AppendLine($"            if (element is {uiElementType} uiElement)");
        writer.AppendLine("            {");
        writer.AppendLine($"                uiElement.{action}({@event.Name}Event, handler);");
        writer.AppendLine("            }");
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

            writer.AppendLine();
            writer.AppendLine("#nullable enable");
            writer.AppendLine();
            writer.AppendLine($"namespace {@class.Namespace}");
            writer.AppendLine("{");
            writer.AppendLine($"    {SourceGenerationHelper.GenerateModifiers(@class)}partial class {@class.Name}");
            writer.AppendLine("    {");
            writer.AppendLine($"        static {@class.Name}()");
            writer.AppendLine("        {");
            
            writer.Append(tempWriter.ToString());

            writer.AppendLine("        }");
            writer.AppendLine("    }");
            writer.AppendLine("}");
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
        writer.LineIf(property.AffectsRender, $"            AffectsRender<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.AffectsMeasure, $"            AffectsMeasure<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.AffectsArrange, $"            AffectsArrange<{@class.Type}>({property.Name}Property);");
    }

    private static void GenerateAvaloniaStaticConstructorPropertyChanged(
        ref SourceWriter writer,
        ClassData @class,
        DependencyPropertyData property)
    {
        var (name, isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2) 
            = SourceGenerationHelper.CheckOnChangedMethods(@class, property);
        
        if (!isChanged0 && !isChanged1 && !isChanged2 && !isChanged3 && !isChangedArgs1 && !isChangedArgs2)
        {
            return;
        }
        
        writer.AppendLine($"            {property.Name}Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<{SourceGenerationHelper.GenerateType(property)}>>(static x =>");
        writer.AppendLine("            {");
        
        if (property.IsAttached)
        {
            writer.LineIf(isChanged0, $"                {name}();");
            writer.LineIf(isChanged1, $"""
                                {name}(
                                    ({SourceGenerationHelper.GenerateBrowsableForType(property)})x.Sender);
                """);
            writer.LineIf(isChanged2, $"""
                                {name}(
                                    ({SourceGenerationHelper.GenerateBrowsableForType(property)})x.Sender,
                                    ({SourceGenerationHelper.GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChanged3, $"""
                                {name}(
                                    ({SourceGenerationHelper.GenerateBrowsableForType(property)})x.Sender,
                                    ({SourceGenerationHelper.GenerateType(property)})x.OldValue.GetValueOrDefault(),
                                    ({SourceGenerationHelper.GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChangedArgs1, $"""
                                {name}(
                                    x);
                """);
            writer.LineIf(isChangedArgs2, $"""
                                {name}(
                                    ({SourceGenerationHelper.GenerateBrowsableForType(property)})x.Sender,
                                    x);
                """);
        }
        else
        {
            writer.LineIf(isChanged0, $"                (({@class.Type})x.Sender).{name}();");
            writer.LineIf(isChanged1, $"""
                                (({@class.Type})x.Sender).{name}(
                                    ({SourceGenerationHelper.GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChanged2, $"""
                                (({@class.Type})x.Sender).{name}(
                                    ({SourceGenerationHelper.GenerateType(property)})x.OldValue.GetValueOrDefault(),
                                    ({SourceGenerationHelper.GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChangedArgs1, $"""
                                (({@class.Type})x.Sender).{name}(
                                    x);
                """);
            writer.LineIf(isChangedArgs2, $"""
                                {name}(
                                    (({@class.Type})x.Sender),
                                    x);
                """);
        }

        writer.AppendLine("            }));");
    }
}
