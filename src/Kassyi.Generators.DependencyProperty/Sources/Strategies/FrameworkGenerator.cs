using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal abstract class FrameworkGenerator
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

    public virtual string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property)
    {
        return $"""
                        {property.ComponentModel.FromType}.{property.Name}Property.AddOwner(
                            ownerType: typeof({@class.Type}),
                            {GeneratePropertyMetadata(@class, property)});
            """;
    }

    public string GeneratePropertyChangedCallback(ClassData @class, DependencyPropertyData property)
    {
        var (name, callbacks) = SourceGenerationHelper.CheckOnChangedMethods(@class, property);
        
        if (callbacks is { IsChanged0: false, IsChanged1: false, IsChanged2: false, IsChanged3: false, IsChangedArgs1: false, IsChangedArgs2: false })
        {
            return "null";
        }
        
        return GeneratePropertyChangedCallbackInternal(@class, property, name, callbacks);
    }

    protected virtual string PropertyChangedCallbackSignature => "static (sender, args) =>";
    protected virtual string NewValueExpression => "args.NewValue";
    protected virtual string OldValueExpression => "args.OldValue";
    protected virtual string GenerateArgsExpression(DependencyPropertyData property) => "args";

    protected virtual string GeneratePropertyChangedCallbackInternal(
        ClassData @class, DependencyPropertyData property, string name, 
        EventCallbackData callbacks)
    {
        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;
            
        var argsExpr = GenerateArgsExpression(property);

        var senderCast = $"({senderType})sender";
        var instanceCast = $"(({senderType})sender)";
        var typeCast = $"({SourceGenerationHelper.GenerateType(property)})";
        var isAttached = property.IsAttached;

        using var writer = new SourceWriter();
        writer.AppendLine(PropertyChangedCallbackSignature);
        writer.AppendLine("                    {");

        if (callbacks.IsChanged0)
        {
            writer.AppendLine($"                        {GenerateCall(name, isAttached, instanceCast)}");
        }
        if (callbacks.IsChanged1)
        {
            var args = isAttached ? new[] { senderCast } : new[] { $"{typeCast}{NewValueExpression}" };
            writer.AppendLine($"                        {GenerateCall(name, isAttached, instanceCast, args)}");
        }
        if (callbacks.IsChanged2)
        {
            var args = isAttached ? new[] { senderCast, $"{typeCast}{NewValueExpression}" } : new[] { $"{typeCast}{OldValueExpression}", $"{typeCast}{NewValueExpression}" };
            writer.AppendLine($"                        {GenerateCall(name, isAttached, instanceCast, args)}");
        }
        if (callbacks.IsChanged3)
        {
            writer.AppendLine($"                        {GenerateCall(name, isAttached, instanceCast, senderCast, $"{typeCast}{OldValueExpression}", $"{typeCast}{NewValueExpression}")}");
        }
        if (callbacks.IsChangedArgs1)
        {
            writer.AppendLine($"                        {GenerateCall(name, isAttached, instanceCast, argsExpr)}");
        }
        if (callbacks.IsChangedArgs2)
        {
            var senderArg = isAttached ? senderCast : instanceCast;
            writer.AppendLine($"                        {GenerateCall(name, true, instanceCast, senderArg, argsExpr)}");
        }

        writer.Append("                    }");
        return writer.ToString();
    }

    protected static string GenerateCall(string methodName, bool isStatic, string senderExpression, params string[] args)
    {
        var argIndent = "                            ";
        if (isStatic)
        {
            return args.Length == 0 ? $"{methodName}();" : $"""
                                                            {methodName}(
                                                            {argIndent}{string.Join($",\n{argIndent}", args)});
                                                            """;
        }
        else
        {
            return args.Length == 0 ? $"{senderExpression}.{methodName}();" : $"""
                 {senderExpression}.{methodName}(
                 {argIndent}{string.Join($",\n{argIndent}", args)});
                 """;
        }
    }
    

    public string GenerateCoerceValueCallback(ClassData @class, DependencyPropertyData property)
    {
        if (!property.ValidationAndCallbacks.Coerce)
        {
            return "null";
        }
        
        return GenerateCoerceValueCallbackInternal(@class, property);
    }
    
    protected virtual string CoerceAttachedCallbackSignature => "static (sender, args) =>";
    protected virtual string CoerceAttachedValueExpression => "args.Value";

    protected virtual string GenerateCoerceValueCallbackInternal(ClassData @class, DependencyPropertyData property)
    {
        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;

        return property.IsAttached
            ? $"""
               {CoerceAttachedCallbackSignature}
                                       Coerce{property.Name}(
                                           ({senderType})sender,
                                           ({SourceGenerationHelper.GenerateType(property, canBeNull: true)}){CoerceAttachedValueExpression})
               """
            : $"""
               static (sender, value) =>
                                       (({senderType})sender).Coerce{property.Name}(
                                           ({SourceGenerationHelper.GenerateType(property, canBeNull: true)})value)
               """;
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
        var senderType = property.IsAttached
            ? SourceGenerationHelper.GenerateBrowsableForType(property)
            : @class.Type;

        return property.IsAttached
            ? $"""
               static (sender, args) =>
                                       Is{property.Name}Valid(
                                           ({senderType})sender,
                                           ({SourceGenerationHelper.GenerateType(property, canBeNull: true)})args.Value)
               """
            : $"""
               static value =>
                                       Is{property.Name}Valid(
                                           ({SourceGenerationHelper.GenerateType(property, canBeNull: true)})value)
               """;
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

    protected static void GenerateEmptySpace(ref SourceWriter writer) =>
        writer.Append(" ");
    
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

        writer.AppendLine($$"""
        #nullable enable

        namespace {{@class.Namespace}}
        {
            {{SourceGenerationHelper.GenerateModifiers(@class)}}partial class {{@class.Name}}
            {
        """);
        
        if (!@event.WinRtEvents)
        {
            writer.AppendLine($$"""
        /// <summary>
        /// A helper method to raise the {{@event.Name}} event. <br/>
        /// WinRT events are disabled by default due to a series of issues with them in Windows 10:
        /// https://github.com/HavenDV/H.NotifyIcon/issues/36
        /// https://github.com/HavenDV/H.NotifyIcon/issues/31
        /// Use the WinRTEvents = true option to enable them.
        /// </summary>
        protected {{routedEventArgsType}}? On{{@event.Name}}()
        {
            return null;
        }
""");
        }
        else
        {
            SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
            SourceGenerationHelper.GenerateCategoryAttribute(ref writer, @event.Category);
            SourceGenerationHelper.GenerateDescriptionAttribute(ref writer, @event.Description);
            writer.AppendLine($"        public event {routerEventType}? {@event.Name};");
            writer.AppendLine();
            writer.AppendLine($$"""
        /// <summary>
        /// A helper method to raise the {{@event.Name}} event.
        /// </summary>
        protected {{routedEventArgsType}} On{{@event.Name}}()
        {
            var args = new {{routedEventArgsType}}();
            {{@event.Name}}?.Invoke(this, args);

            return args;
        }
""");
        }
        
        writer.AppendLine("    }");
        writer.AppendLine("}");
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

    protected virtual string GenerateRegisterRoutedEventMethodArguments(ClassData @class, EventData @event) => $"""

                                  name: "{@event.Name}",
                                  routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy},
                                  handlerType: typeof({GenerateRouterEventType(@class, @event)}),
                                  ownerType: typeof({@class.Type})
                  """;

    protected void GenerateRoutedEventInternal(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var routedEventType = GenerateRoutedEventType(@class);
        var eventManagerType = GenerateEventManagerType(@class);
        var registerMethod = GenerateRegisterRoutedEventMethod(@class);
        var registerArgs = GenerateRegisterRoutedEventMethodArguments(@class, @event);
        var routedEventArgsType = GenerateRoutedEventArgsType(@class);
        var routerEventType = GenerateRouterEventType(@class, @event);

        writer.AppendLine($$"""
        #nullable enable

        namespace {{@class.Namespace}}
        {
            {{SourceGenerationHelper.GenerateModifiers(@class)}}partial class {{@class.Name}}
            {
        """);
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.XmlDocumentation, @event);
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        writer.AppendLine($"        public static readonly {routedEventType} {@event.Name}Event =");
        writer.AppendLine($"            {eventManagerType}.{registerMethod}(");
        writer.AppendLine($"                {registerArgs});");
        writer.AppendLine();
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        SourceGenerationHelper.GenerateCategoryAttribute(ref writer, @event.Category);
        SourceGenerationHelper.GenerateDescriptionAttribute(ref writer, @event.Description);
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        writer.AppendLine($$"""
        public event {{routerEventType}} {{@event.Name}}
        {
            add => AddHandler({{@event.Name}}Event, value);
            remove => RemoveHandler({{@event.Name}}Event, value);
        }
""");
        writer.AppendLine();
        writer.AppendLine($$"""
        /// <summary>
        /// A helper method to raise the {{@event.Name}} event.
        /// </summary>
""");
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        writer.AppendLine($$"""
        protected {{routedEventArgsType}} On{{@event.Name}}()
        {
            var args = new {{routedEventArgsType}}({{@event.Name}}Event);
            this.RaiseEvent(args);

            return args;
        }
""");
        writer.AppendLine("    }");
        writer.AppendLine("}");
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

        writer.AppendLine($$"""

        #nullable enable

        namespace {{@class.Namespace}}
        {
            {{SourceGenerationHelper.GenerateModifiers(@class)}}partial class {{@class.Name}}
            {
        """);
        
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.XmlDocumentation, @event);
        writer.AppendLine($"        public static readonly {routedEventType} {@event.Name}Event =");
        writer.AppendLine($"            {eventManagerType}.{registerMethod}(");
        writer.AppendLine($"                {registerArgs});");
        writer.AppendLine();
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        SourceGenerationHelper.GenerateCategoryAttribute(ref writer, @event.Category);
        SourceGenerationHelper.GenerateDescriptionAttribute(ref writer, @event.Description);
        writer.AppendLine($$"""
        public static void Add{{@event.Name}}Handler({{dependencyObjectType}} element, {{routedEventHandlerType}} handler)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));

""");
        
        GenerateAddHandler(ref writer, @class, @event, uiElementType, contentElementType);
        
        writer.AppendLine("        }");
        writer.AppendLine();
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        SourceGenerationHelper.GenerateCategoryAttribute(ref writer, @event.Category);
        SourceGenerationHelper.GenerateDescriptionAttribute(ref writer, @event.Description);
        writer.AppendLine($$"""
        public static void Remove{{@event.Name}}Handler({{dependencyObjectType}} element, {{routedEventHandlerType}} handler)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));

""");
        
        GenerateRemoveHandler(ref writer, @class, @event, uiElementType, contentElementType);
        
        writer.AppendLine("        }");
        writer.AppendLine("    }");
        writer.AppendLine("}");
    }

    protected virtual void GenerateHandlerAction(ref SourceWriter writer, EventData @event, string uiElementType, string contentElementType, string action)
    {
        writer.AppendLine($$"""
            if (element is {{uiElementType}} uiElement)
            {
                uiElement.{{action}}({{@event.Name}}Event, handler);
            }
            else if (element is {{contentElementType}} contentElement)
            {
                contentElement.{{action}}({{@event.Name}}Event, handler);
            }
""");
    }

    protected virtual void GenerateAddHandler(ref SourceWriter writer, ClassData @class, EventData @event, string uiElementType, string contentElementType) =>
        GenerateHandlerAction(ref writer, @event, uiElementType, contentElementType, "AddHandler");

    protected virtual void GenerateRemoveHandler(ref SourceWriter writer, ClassData @class, EventData @event, string uiElementType, string contentElementType) =>
        GenerateHandlerAction(ref writer, @event, uiElementType, contentElementType, "RemoveHandler");
}
