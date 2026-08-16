using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal abstract class FrameworkGenerator : 
    IDependencyPropertyGeneratorStrategy,
    IRoutedEventGeneratorStrategy,
    IWeakEventGeneratorStrategy
{
    public abstract string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property);
    public abstract string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property);
    public abstract void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName);
    
    public virtual string GeneratePropertyMetadata(ClassData @class, DependencyPropertyData property)
    {
        if (property is { IsAddOwner: true, DefaultValue: null })
        {
            return "null";
        }

        var parameterName = (@class.Framework, property.IsAttached) switch
        {
            (Framework.Wpf, true) or (Framework.Uwp, true) or (Framework.WinUi, true) => "defaultMetadata: ",
            (Framework.Avalonia, _) => "metadata: ",
            (Framework.Uno, true) or (Framework.UnoWinUi, true) => string.Empty,
            _ => "typeMetadata: ",
        };
        var writer = new SourceWriter();
        try
        {
            GeneratePropertyMetadata(ref writer, @class, property, parameterName);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    public virtual string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property) =>
        $"{property.ComponentModel.FromType}.{property.Name}Property.AddOwner(ownerType: typeof({@class.Type}), {GeneratePropertyMetadata(@class, property)});";

    public virtual string GeneratePropertyChangedCallback(ClassData @class, DependencyPropertyData property)
    {
        var (name, callbacks) = SourceGenerationHelper.CheckOnChangedMethods(@class, property);
        
        return callbacks.ChangedSignatures == CallbackSignature.None ? "null" : GeneratePropertyChangedCallbackInternal(@class, property, name, callbacks);
    }

    protected virtual string PropertyChangedCallbackSignature => "static (sender, args) =>";
    protected virtual string NewValueExpression => "args.NewValue";
    protected virtual string OldValueExpression => "args.OldValue";
    protected virtual string GenerateArgsExpression(DependencyPropertyData property) => "args";

    protected virtual string GeneratePropertyChangedCallbackInternal(
        ClassData @class, DependencyPropertyData property, string name, 
        EventCallbackData callbacks) =>
        GenerateCallbackInternal(@class, property, name, callbacks.ChangedSignatures, PropertyChangedCallbackSignature);

    protected string GenerateCallbackInternal(
        ClassData @class,
        DependencyPropertyData property,
        string name,
        CallbackSignature signatures,
        string callbackSignature)
    {
        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;

        var senderCast = $"({senderType})sender";
        var instanceCast = $"(({senderType})sender)";
        var typeCast = $"({SourceGenerationHelper.GenerateType(property)})";
        var isAttached = property.IsAttached;

        var oldVal = $"{typeCast}{OldValueExpression}";
        var newVal = $"{typeCast}{NewValueExpression}";
        var argsExpr = GenerateArgsExpression(property);

        using var writer = new SourceWriter();
        writer.AppendLine(callbackSignature);
        writer.AppendLine("{");

        (CallbackSignature Flag, bool IsStatic, string[] Args)[] mappings =
        [
            (CallbackSignature.NoParameters, isAttached, []),
            (CallbackSignature.NewValue, isAttached, isAttached ? [senderCast] : [newVal]),
            (CallbackSignature.OldAndNewValue, isAttached, isAttached ? [senderCast, newVal] : [oldVal, newVal]),
            (CallbackSignature.SenderAndOldAndNewValue, isAttached, [senderCast, oldVal, newVal]),
            (CallbackSignature.EventArgs, isAttached, [argsExpr]),
            (CallbackSignature.SenderAndEventArgs, true, [isAttached ? senderCast : instanceCast, argsExpr]),
        ];

        foreach (var (flag, isStatic, args) in mappings)
        {
            if (signatures.HasFlag(flag))
            {
                writer.AppendLine(GenerateCall(name, isStatic, instanceCast, args));
            }
        }

        writer.Append("}");
        return writer.ToString();
    }

    protected static string GenerateCall(string methodName, bool isStatic, string senderExpression, params string[] args)
    {
        var target = isStatic ? methodName : $"{senderExpression}.{methodName}";
        return $"{target}({string.Join(", ", args)});";
    }
    

    public string GenerateCoerceValueCallback(ClassData @class, DependencyPropertyData property)
    {
        if (!property.ValidationAndCallbacks.Coerce)
        {
            return "null";
        }
        
        return GenerateCoerceValueCallbackInternal(@class, property);
    }

    protected virtual string CoerceAttachedCallbackSignature => "static (sender, value) =>";
    protected virtual string CoerceAttachedValueExpression => "value";

    protected virtual string GenerateCoerceValueCallbackInternal(ClassData @class, DependencyPropertyData property)
    {
        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;
        var propertyType = SourceGenerationHelper.GenerateType(property, canBeNull: true);

        return property.IsAttached
            ? $"{CoerceAttachedCallbackSignature} Coerce{property.Name}(({senderType})sender, ({propertyType}){CoerceAttachedValueExpression})"
            : $"static (sender, value) => (({senderType})sender).Coerce{property.Name}(({propertyType})value)";
    }
    
    public string GenerateValidateValueCallback(ClassData @class, DependencyPropertyData property)
    {
        if (!property.ValidationAndCallbacks.Validate)
        {
            return "null";
        }

        return GenerateValidateValueCallbackInternal(@class, property);
    }
    
    protected virtual string GenerateValidateValueCallbackInternal(ClassData @class, DependencyPropertyData property)
    {
        var propertyType = SourceGenerationHelper.GenerateType(property, canBeNull: true);

        return $"static value => Is{property.Name}Valid(({propertyType})value)";
    }
    
    public virtual string GenerateCreateDefaultValueCallback(DependencyPropertyData property) =>
        property.ValidationAndCallbacks.CreateDefaultValueCallback ? $"static () => Get{property.Name}DefaultValue()" : "null";

    public abstract string GeneratePropertyType(ClassData @class, DependencyPropertyData property);
    public abstract string GenerateManagerType(ClassData @class);

    public virtual void GenerateStaticConstructor(
        ref SourceWriter writer,
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> properties)
    {
    }
    
    public virtual void GenerateAdditionalFieldForDirectProperties(
        ref SourceWriter writer,
        DependencyPropertyData property) =>
        GenerateEmptySpace(ref writer);

    public virtual void GenerateAdditionalPropertyForReadOnlyProperties(
        ref SourceWriter writer,
        DependencyPropertyData property) =>
        GenerateEmptySpace(ref writer);

    protected static void GenerateEmptySpace(ref SourceWriter writer)
    {
    }
    
    public virtual string GenerateCreateDefaultValueCallbackValueCallback(DependencyPropertyData property)
    {
        return property.ValidationAndCallbacks.CreateDefaultValueCallback switch
        {
            false => "null",
            _ => property.Framework switch
            {
                Framework.Maui => "static _ => Get" + property.Name + "DefaultValue()",
                _ => "static () => Get" + property.Name + "DefaultValue()"
            }
        };
    }

    public virtual void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var routedEventArgsType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "RoutedEventArgs");
        var routerEventType = @event.Type;
        if (string.IsNullOrWhiteSpace(routerEventType))
        {
            routerEventType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "RoutedEventHandler");
        }

        using var _ = writer.ClassScope(@class);
        
        if (!@event.WinRtEvents)
        {
            writer.AppendLine("/// <summary>");
            writer.AppendLine($"/// A helper method to raise the {@event.Name} event. <br/>");
            writer.AppendLine("/// WinRT events are disabled by default due to known event registration and lifetime issues in Windows 10.<br/>");
            writer.AppendLine("/// Use the WinRtEvents = true option to enable them.");
            writer.AppendLine("/// </summary>");
            using (writer.Scope($"protected {routedEventArgsType}? On{@event.Name}()"))
            {
                writer.AppendLine("return null;");
            }
        }
        else
        {
            SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
            SourceGenerationHelper.GenerateCategoryAttribute(ref writer, @event.Category);
            SourceGenerationHelper.GenerateDescriptionAttribute(ref writer, @event.Description);
            writer.AppendLine($"public event {routerEventType}? {@event.Name};");
            writer.AppendLine();
            writer.AppendLine("/// <summary>");
            writer.AppendLine($"/// A helper method to raise the {@event.Name} event.");
            writer.AppendLine("/// </summary>");
            using (writer.Scope($"protected {routedEventArgsType} On{@event.Name}()"))
            {
                writer.AppendLine($"var args = new {routedEventArgsType}();");
                writer.AppendLine($"{@event.Name}?.Invoke(this, args);");
                writer.AppendLine();
                writer.AppendLine("return args;");
            }
        }
    }

    public virtual void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
    }

    public virtual void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
    }

    protected static string GenerateEventHandlerType(EventData @event, bool nullable = true, bool nullableType = true)
    {
        var eventHandler = (string.IsNullOrWhiteSpace(@event.Type)
            ? "System.EventHandler"
            : $"System.EventHandler<{SourceGenerationHelper.GenerateType(@event, nullable: nullableType)}>").WithGlobalPrefix();
        if (nullable)
        {
            eventHandler += "?";
        }

        return eventHandler;
    }

    protected virtual string GenerateRoutedEventType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "RoutedEvent");
    protected virtual string GenerateRoutedEventArgsType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "RoutedEventArgs");
    protected virtual string GenerateRoutedEventHandlerType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "RoutedEventHandler");
    protected virtual string GenerateRoutingStrategyType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "RoutingStrategy");
    protected virtual string GenerateEventManagerType(ClassData @class) => SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "EventManager");
    protected virtual string GenerateRegisterRoutedEventMethod(ClassData @class) => "RegisterRoutedEvent";
    
    protected virtual string GenerateRouterEventType(ClassData @class, EventData @event) =>
        string.IsNullOrWhiteSpace(@event.Type) ? GenerateRoutedEventHandlerType(@class) : @event.Type;

    protected virtual string GenerateRegisterRoutedEventMethodArguments(ClassData @class, EventData @event) =>
        $"name: \"{@event.Name}\", routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy}, handlerType: typeof({GenerateRouterEventType(@class, @event)}), ownerType: typeof({@class.Type})";

    protected void GenerateRoutedEventInternal(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var routedEventType = GenerateRoutedEventType(@class);
        var eventManagerType = GenerateEventManagerType(@class);
        var registerMethod = GenerateRegisterRoutedEventMethod(@class);
        var registerArgs = GenerateRegisterRoutedEventMethodArguments(@class, @event);
        var routedEventArgsType = GenerateRoutedEventArgsType(@class);
        var routerEventType = GenerateRouterEventType(@class, @event);

        using var _ = writer.ClassScope(@class);
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.XmlDocumentation, @event);
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        writer.AppendLine($"public static readonly {routedEventType} {@event.Name}Event = {eventManagerType}.{registerMethod}({registerArgs});");
        writer.AppendLine();
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        SourceGenerationHelper.GenerateCategoryAttribute(ref writer, @event.Category);
        SourceGenerationHelper.GenerateDescriptionAttribute(ref writer, @event.Description);
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        using (writer.Scope($"public event {routerEventType} {@event.Name}"))
        {
            writer.AppendLine($"add => AddHandler({@event.Name}Event, value);");
            writer.AppendLine($"remove => RemoveHandler({@event.Name}Event, value);");
        }
        writer.AppendLine();
        writer.AppendLine("/// <summary>");
        writer.AppendLine($"/// A helper method to raise the {@event.Name} event.");
        writer.AppendLine("/// </summary>");
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        using (writer.Scope($"protected {routedEventArgsType} On{@event.Name}()"))
        {
            writer.AppendLine($"var args = new {routedEventArgsType}({@event.Name}Event);");
            writer.AppendLine("this.RaiseEvent(args);");
            writer.AppendLine();
            writer.AppendLine("return args;");
        }
    }

    protected void GenerateAttachedRoutedEventInternal(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var routedEventType = GenerateRoutedEventType(@class);
        var eventManagerType = GenerateEventManagerType(@class);
        var registerMethod = GenerateRegisterRoutedEventMethod(@class);
        var registerArgs = GenerateRegisterRoutedEventMethodArguments(@class, @event);
        
        var dependencyObjectType = SourceGenerationHelper.GenerateDependencyObjectType(@class.Framework);
        var routedEventHandlerType = GenerateRoutedEventHandlerType(@class);
        var uiElementType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "UIElement");
        var contentElementType = SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "ContentElement");

        using var _ = writer.ClassScope(@class);
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.XmlDocumentation, @event);
        writer.AppendLine($"public static readonly {routedEventType} {@event.Name}Event = {eventManagerType}.{registerMethod}({registerArgs});");
        writer.AppendLine();

        writeMethodSignature(ref writer, "Add");
        using (writer.Scope())
        {
            writer.AppendLine("element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
            GenerateAddHandler(ref writer, @class, @event, uiElementType, contentElementType);
        }
        writer.AppendLine();

        writeMethodSignature(ref writer, "Remove");
        using (writer.Scope())
        {
            writer.AppendLine("element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
            GenerateRemoveHandler(ref writer, @class, @event, uiElementType, contentElementType);
        }
        return;

        void writeMethodSignature(ref SourceWriter w, string prefix)
        {
            SourceGenerationHelper.GenerateXmlDocumentationFrom(ref w, @event.EventXmlDocumentation, @event);
            SourceGenerationHelper.GenerateCategoryAttribute(ref w, @event.Category);
            SourceGenerationHelper.GenerateDescriptionAttribute(ref w, @event.Description);
            w.AppendLine($"public static void {prefix}{@event.Name}Handler({dependencyObjectType} element, {routedEventHandlerType} handler)");
        }
    }

    protected virtual void GenerateHandlerAction(ref SourceWriter writer, EventData @event, string uiElementType, string contentElementType, string action)
    {
        writer.AppendLine($"if (element is {uiElementType} uiElement)");
        using (writer.Scope())
        {
            writer.AppendLine($"uiElement.{action}({@event.Name}Event, handler);");
        }
        writer.AppendLine($"else if (element is {contentElementType} contentElement)");
        using (writer.Scope())
        {
            writer.AppendLine($"contentElement.{action}({@event.Name}Event, handler);");
        }
    }

    protected virtual void GenerateAddHandler(ref SourceWriter writer, ClassData @class, EventData @event, string uiElementType, string contentElementType) =>
        GenerateHandlerAction(ref writer, @event, uiElementType, contentElementType, "AddHandler");

    protected virtual void GenerateRemoveHandler(ref SourceWriter writer, ClassData @class, EventData @event, string uiElementType, string contentElementType) =>
        GenerateHandlerAction(ref writer, @event, uiElementType, contentElementType, "RemoveHandler");
}
