using H.Generators.Extensions;

namespace H.Generators;

internal static partial class Sources
{
    public static string GenerateRoutedEvent(ClassData @class, EventData @event)
    {
        if (@event.IsAttached)
        {
            return GenerateAttachedRoutedEvent(@class, @event);
        }

        var categoryAttr = GenerateCategoryAttribute(@event.Category);
        var descriptionAttr = GenerateDescriptionAttribute(@event.Description);
        var routedEventArgsType = GenerateRoutedEventArgsType(@class);
        var routerEventType = GenerateRouterEventType(@class, @event);
        var eventXmlDoc = GenerateXmlDocumentationFrom(@event.EventXmlDocumentation, @event);

        // https://docs.avaloniaui.net/docs/input/routed-events
        if (@class.Framework == Framework.Wpf || @class.Framework == Framework.Avalonia)
        {
            var fieldXmlDoc = GenerateXmlDocumentationFrom(@event.XmlDocumentation, @event);
            var generatedCodeAttr = GenerateGeneratedCodeAttribute(@class.Version);
            var routedEventType = GenerateRoutedEventType(@class);
            var eventManagerType = GenerateEventManagerType(@class);
            var registerMethod = GenerateRegisterMethod(@class);
            var registerArgs = GenerateRegisterRoutedEventMethodArguments(@class, @event);
            var excludeFromCoverageAttr = GenerateExcludeFromCodeCoverageAttribute();

            return $$"""
            #nullable enable

            namespace {{@class.Namespace}}
            {
                {{@class.Modifiers}}partial class {{@class.Name}}
                {
            {{fieldXmlDoc}}
            {{generatedCodeAttr}}
                    public static readonly {{routedEventType}} {{@event.Name}}Event =
                        {{eventManagerType}}.{{registerMethod}}(
                            {{registerArgs}});

            {{eventXmlDoc}}
            {{categoryAttr}}
            {{descriptionAttr}}
            {{generatedCodeAttr}}
            {{excludeFromCoverageAttr}}
                    public event {{routerEventType}} {{@event.Name}}
                    {
                        add => AddHandler({{@event.Name}}Event, value);
                        remove => RemoveHandler({{@event.Name}}Event, value);
                    }

                    /// <summary>
                    /// A helper method to raise the {{@event.Name}} event.
                    /// </summary>
            {{generatedCodeAttr}}
            {{excludeFromCoverageAttr}}
                    protected {{routedEventArgsType}} On{{@event.Name}}()
                    {
                        var args = new {{routedEventArgsType}}({{@event.Name}}Event);
                        this.RaiseEvent(args);

                        return args;
                    }
                }
            }
            """.RemoveBlankLinesWhereOnlyWhitespaces();
        }

        if (!@event.WinRtEvents)
        {
            return $$"""
            #nullable enable

            namespace {{@class.Namespace}}
            {
                {{@class.Modifiers}}partial class {{@class.Name}}
                {
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
                }
            }
            """.RemoveBlankLinesWhereOnlyWhitespaces();
        }

        return $$"""
            #nullable enable

            namespace {{@class.Namespace}}
            {
                {{@class.Modifiers}}partial class {{@class.Name}}
                {
            {{eventXmlDoc}}
            {{categoryAttr}}
            {{descriptionAttr}}
                    public event {{routerEventType}}? {{@event.Name}};

                    /// <summary>
                    /// A helper method to raise the {{@event.Name}} event.
                    /// </summary>
                    protected {{routedEventArgsType}} On{{@event.Name}}()
                    {
                        var args = new {{routedEventArgsType}}();
                        {{@event.Name}}?.Invoke(this, args);

                        return args;
                    }
                }
            }
            """.RemoveBlankLinesWhereOnlyWhitespaces();
    }
    
    public static string GenerateAttachedRoutedEvent(ClassData @class, EventData @event)
    {
        var xmlDoc = GenerateXmlDocumentationFrom(@event.XmlDocumentation, @event);
        var routedEventType = GenerateRoutedEventType(@class);
        var eventManagerType = GenerateEventManagerType(@class);
        var registerMethod = GenerateRegisterMethod(@class);
        var registerArgs = GenerateRegisterRoutedEventMethodArguments(@class, @event);

        var eventXmlDoc = GenerateXmlDocumentationFrom(@event.EventXmlDocumentation, @event);
        var categoryAttr = GenerateCategoryAttribute(@event.Category);
        var descriptionAttr = GenerateDescriptionAttribute(@event.Description);
        
        var dependencyObjectType = GenerateDependencyObjectType(@class.Framework);
        var routedEventHandlerType = GenerateRoutedEventHandlerType(@class);
        var uiElementType = GenerateTypeByPlatform(@class.Framework, "UIElement");
        var contentElementType = GenerateTypeByPlatform(@class.Framework, "ContentElement");

        return $$"""

            #nullable enable

            namespace {{@class.Namespace}}
            {
                {{@class.Modifiers}}partial class {{@class.Name}}
                {
            {{xmlDoc}}
                    public static readonly {{routedEventType}} {{@event.Name}}Event =
                        {{eventManagerType}}.{{registerMethod}}(
                            {{registerArgs}});

            {{eventXmlDoc}}
            {{categoryAttr}}
            {{descriptionAttr}}
                    public static void Add{{@event.Name}}Handler({{dependencyObjectType}} element, {{routedEventHandlerType}} handler)
                    {
                        element = element ?? throw new global::System.ArgumentNullException(nameof(element));

                        if (element is {{uiElementType}} uiElement)
                        {
                            uiElement.AddHandler({{@event.Name}}Event, handler);
                        }
                        else if (element is {{contentElementType}} contentElement)
                        {
                            contentElement.AddHandler({{@event.Name}}Event, handler);
                        }
                    }

            {{eventXmlDoc}}
            {{categoryAttr}}
            {{descriptionAttr}}
                    public static void Remove{{@event.Name}}Handler({{dependencyObjectType}} element, {{routedEventHandlerType}} handler)
                    {
                        element = element ?? throw new global::System.ArgumentNullException(nameof(element));

                        if (element is {{uiElementType}} uiElement)
                        {
                            uiElement.RemoveHandler({{@event.Name}}Event, handler);
                        }
                        else if (element is {{contentElementType}} contentElement)
                        {
                            contentElement.RemoveHandler({{@event.Name}}Event, handler);
                        }
                    }
                }
            }
            """.RemoveBlankLinesWhereOnlyWhitespaces();
    }

    private static string GenerateRouterEventType(ClassData @class, EventData @event)
    {
        if (string.IsNullOrWhiteSpace(@event.Type))
        {
            return GenerateRoutedEventHandlerType(@class);
        }

        return @event.Type;
    }

    private static string GenerateRoutedEventType(ClassData @class)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return
                $"{GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent")}<{GenerateRoutedEventArgsType(@class)}>";
        }

        return GenerateTypeByPlatform(@class.Framework, "RoutedEvent");
    }

    private static string GenerateRoutedEventArgsType(ClassData @class)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEventArgs");
        }

        return GenerateTypeByPlatform(@class.Framework, "RoutedEventArgs");
    }

    private static string GenerateRoutedEventHandlerType(ClassData @class)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return $"global::System.EventHandler<{GenerateRoutedEventArgsType(@class)}>";
        }

        return GenerateTypeByPlatform(@class.Framework, "RoutedEventHandler");
    }
    
    private static string GenerateRegisterRoutedEventMethodArguments(ClassData @class, EventData @event)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return $"""

                                    name: "{@event.Name}",
                                    routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy}
                    """;
        }

        return $"""

                                name: "{@event.Name}",
                                routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy},
                                handlerType: typeof({GenerateRouterEventType(@class, @event)}),
                                ownerType: typeof({@class.Type})
                """;
    }

    private static string GenerateRoutingStrategyType(ClassData @class)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return GenerateTypeByPlatform(@class.Framework, $"Interactivity.RoutingStrategies");
        }

        return GenerateTypeByPlatform(@class.Framework, "RoutingStrategy");
    }

    private static string GenerateEventManagerType(ClassData @class)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent");
        }

        return GenerateTypeByPlatform(@class.Framework, "EventManager");
    }

    private static string GenerateRegisterMethod(ClassData @class)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            return $"Register<{@class.Type}, {GenerateRoutedEventArgsType(@class)}>";
        }

        return "RegisterRoutedEvent";
    }
}
