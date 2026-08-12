using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        switch (@event.IsAttached)
        {
            case true:
                GenerateAttachedRoutedEvent(ref writer, @class, @event);
                return;
        }

        var routedEventArgsType = GenerateRoutedEventArgsType(@class);
        var routerEventType = GenerateRouterEventType(@class, @event);

        switch (@class.Framework)
        {
            // https://docs.avaloniaui.net/docs/input/routed-events
            case Framework.Wpf:
            case Framework.Avalonia:
            {
                var routedEventType = GenerateRoutedEventType(@class);
                var eventManagerType = GenerateEventManagerType(@class);
                var registerMethod = GenerateRegisterMethod(@class);
                var registerArgs = GenerateRegisterRoutedEventMethodArguments(@class, @event);

                writer.AppendLine("#nullable enable");
                writer.AppendLine();
                writer.AppendLine($"namespace {@class.Namespace}");
                writer.AppendLine("{");
                writer.AppendLine($"    {@class.Modifiers}partial class {@class.Name}");
                writer.AppendLine("    {");
                GenerateXmlDocumentationFrom(ref writer, @event.XmlDocumentation, @event);
                GenerateGeneratedCodeAttribute(ref writer, @class.Version);
                writer.AppendLine($"        public static readonly {routedEventType} {@event.Name}Event =");
                writer.AppendLine($"            {eventManagerType}.{registerMethod}(");
                writer.AppendLine($"                {registerArgs});");
                writer.AppendLine();
                GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
                GenerateCategoryAttribute(ref writer, @event.Category);
                GenerateDescriptionAttribute(ref writer, @event.Description);
                GenerateGeneratedCodeAttribute(ref writer, @class.Version);
                GenerateExcludeFromCodeCoverageAttribute(ref writer);
                writer.AppendLine($"        public event {routerEventType} {@event.Name}");
                writer.AppendLine("        {");
                writer.AppendLine($"            add => AddHandler({@event.Name}Event, value);");
                writer.AppendLine($"            remove => RemoveHandler({@event.Name}Event, value);");
                writer.AppendLine("        }");
                writer.AppendLine();
                writer.AppendLine("        /// <summary>");
                writer.AppendLine($"        /// A helper method to raise the {@event.Name} event.");
                writer.AppendLine("        /// </summary>");
                GenerateGeneratedCodeAttribute(ref writer, @class.Version);
                GenerateExcludeFromCodeCoverageAttribute(ref writer);
                writer.AppendLine($"        protected {routedEventArgsType} On{@event.Name}()");
                writer.AppendLine("        {");
                writer.AppendLine($"            var args = new {routedEventArgsType}({@event.Name}Event);");
                writer.AppendLine("            this.RaiseEvent(args);");
                writer.AppendLine();
                writer.AppendLine("            return args;");
                writer.AppendLine("        }");
                writer.AppendLine("    }");
                writer.AppendLine("}");
                break;
            }
            default:
                writer.AppendLine("#nullable enable");
                writer.AppendLine();
                writer.AppendLine($"namespace {@class.Namespace}");
                writer.AppendLine("{");
                writer.AppendLine($"    {@class.Modifiers}partial class {@class.Name}");
                writer.AppendLine("    {");
                
                if (!@event.WinRtEvents)
                {
                    writer.AppendLine("        /// <summary>");
                    writer.AppendLine($"        /// A helper method to raise the {@event.Name} event. <br/>");
                    writer.AppendLine("        /// WinRT events are disabled by default due to a series of issues with them in Windows 10:");
                    writer.AppendLine("        /// https://github.com/HavenDV/H.NotifyIcon/issues/36");
                    writer.AppendLine("        /// https://github.com/HavenDV/H.NotifyIcon/issues/31");
                    writer.AppendLine("        /// Use the WinRTEvents = true option to enable them.");
                    writer.AppendLine("        /// </summary>");
                    writer.AppendLine($"        protected {routedEventArgsType}? On{@event.Name}()");
                    writer.AppendLine("        {");
                    writer.AppendLine("            return null;");
                    writer.AppendLine("        }");
                }
                else
                {
                    GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
                    GenerateCategoryAttribute(ref writer, @event.Category);
                    GenerateDescriptionAttribute(ref writer, @event.Description);
                    writer.AppendLine($"        public event {routerEventType}? {@event.Name};");
                    writer.AppendLine();
                    writer.AppendLine("        /// <summary>");
                    writer.AppendLine($"        /// A helper method to raise the {@event.Name} event.");
                    writer.AppendLine("        /// </summary>");
                    writer.AppendLine($"        protected {routedEventArgsType} On{@event.Name}()");
                    writer.AppendLine("        {");
                    writer.AppendLine($"            var args = new {routedEventArgsType}();");
                    writer.AppendLine($"            {@event.Name}?.Invoke(this, args);");
                    writer.AppendLine();
                    writer.AppendLine("            return args;");
                    writer.AppendLine("        }");
                }
                
                writer.AppendLine("    }");
                writer.AppendLine("}");
                break;
        }
    }
    
    public static void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var routedEventType = GenerateRoutedEventType(@class);
        var eventManagerType = GenerateEventManagerType(@class);
        var registerMethod = GenerateRegisterMethod(@class);
        var registerArgs = GenerateRegisterRoutedEventMethodArguments(@class, @event);
        
        var dependencyObjectType = GenerateDependencyObjectType(@class.Framework);
        var routedEventHandlerType = GenerateRoutedEventHandlerType(@class);
        var uiElementType = GenerateTypeByPlatform(@class.Framework, "UIElement");
        var contentElementType = GenerateTypeByPlatform(@class.Framework, "ContentElement");

        writer.AppendLine();
        writer.AppendLine("#nullable enable");
        writer.AppendLine();
        writer.AppendLine($"namespace {@class.Namespace}");
        writer.AppendLine("{");
        writer.AppendLine($"    {@class.Modifiers}partial class {@class.Name}");
        writer.AppendLine("    {");
        
        GenerateXmlDocumentationFrom(ref writer, @event.XmlDocumentation, @event);
        writer.AppendLine($"        public static readonly {routedEventType} {@event.Name}Event =");
        writer.AppendLine($"            {eventManagerType}.{registerMethod}(");
        writer.AppendLine($"                {registerArgs});");
        writer.AppendLine();
        GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        GenerateCategoryAttribute(ref writer, @event.Category);
        GenerateDescriptionAttribute(ref writer, @event.Description);
        writer.AppendLine($"        public static void Add{@event.Name}Handler({dependencyObjectType} element, {routedEventHandlerType} handler)");
        writer.AppendLine("        {");
        writer.AppendLine("            element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
        writer.AppendLine();
        if (@class.Framework == Framework.Avalonia)
        {
            writer.AppendLine($"            if (element is {uiElementType} uiElement)");
            writer.AppendLine("            {");
            writer.AppendLine($"                uiElement.AddHandler({@event.Name}Event, handler);");
            writer.AppendLine("            }");
        }
        else
        {
            writer.AppendLine($"            if (element is {uiElementType} uiElement)");
            writer.AppendLine("            {");
            writer.AppendLine($"                uiElement.AddHandler({@event.Name}Event, handler);");
            writer.AppendLine("            }");
            writer.AppendLine($"            else if (element is {contentElementType} contentElement)");
            writer.AppendLine("            {");
            writer.AppendLine($"                contentElement.AddHandler({@event.Name}Event, handler);");
            writer.AppendLine("            }");
        }
        writer.AppendLine("        }");
        writer.AppendLine();
        GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        GenerateCategoryAttribute(ref writer, @event.Category);
        GenerateDescriptionAttribute(ref writer, @event.Description);
        writer.AppendLine($"        public static void Remove{@event.Name}Handler({dependencyObjectType} element, {routedEventHandlerType} handler)");
        writer.AppendLine("        {");
        writer.AppendLine("            element = element ?? throw new global::System.ArgumentNullException(nameof(element));");
        writer.AppendLine();
        if (@class.Framework == Framework.Avalonia)
        {
            writer.AppendLine($"            if (element is {uiElementType} uiElement)");
            writer.AppendLine("            {");
            writer.AppendLine($"                uiElement.RemoveHandler({@event.Name}Event, handler);");
            writer.AppendLine("            }");
        }
        else
        {
            writer.AppendLine($"            if (element is {uiElementType} uiElement)");
            writer.AppendLine("            {");
            writer.AppendLine($"                uiElement.RemoveHandler({@event.Name}Event, handler);");
            writer.AppendLine("            }");
            writer.AppendLine($"            else if (element is {contentElementType} contentElement)");
            writer.AppendLine("            {");
            writer.AppendLine($"                contentElement.RemoveHandler({@event.Name}Event, handler);");
            writer.AppendLine("            }");
        }
        writer.AppendLine("        }");
        writer.AppendLine("    }");
        writer.AppendLine("}");
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
        return @class.Framework switch
        {
            Framework.Avalonia =>
                $"{GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent")}<{GenerateRoutedEventArgsType(@class)}>",
            _ => GenerateTypeByPlatform(@class.Framework, "RoutedEvent")
        };
    }

    private static string GenerateRoutedEventArgsType(ClassData @class)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEventArgs"),
            _ => GenerateTypeByPlatform(@class.Framework, "RoutedEventArgs")
        };
    }

    private static string GenerateRoutedEventHandlerType(ClassData @class)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => $"global::System.EventHandler<{GenerateRoutedEventArgsType(@class)}>",
            _ => GenerateTypeByPlatform(@class.Framework, "RoutedEventHandler")
        };
    }
    
    private static string GenerateRegisterRoutedEventMethodArguments(ClassData @class, EventData @event)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => $"""

                                                   name: "{@event.Name}",
                                                   routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy}
                                   """,
            _ => $"""

                                  name: "{@event.Name}",
                                  routingStrategy: {GenerateRoutingStrategyType(@class)}.{@event.Strategy},
                                  handlerType: typeof({GenerateRouterEventType(@class, @event)}),
                                  ownerType: typeof({@class.Type})
                  """
        };
    }

    private static string GenerateRoutingStrategyType(ClassData @class)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => GenerateTypeByPlatform(@class.Framework, $"Interactivity.RoutingStrategies"),
            _ => GenerateTypeByPlatform(@class.Framework, "RoutingStrategy")
        };
    }

    private static string GenerateEventManagerType(ClassData @class)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => GenerateTypeByPlatform(@class.Framework, "Interactivity.RoutedEvent"),
            _ => GenerateTypeByPlatform(@class.Framework, "EventManager")
        };
    }

    private static string GenerateRegisterMethod(ClassData @class)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => $"Register<{@class.Type}, {GenerateRoutedEventArgsType(@class)}>",
            _ => "RegisterRoutedEvent"
        };
    }
}

