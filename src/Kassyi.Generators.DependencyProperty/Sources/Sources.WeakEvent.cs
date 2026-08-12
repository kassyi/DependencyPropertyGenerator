using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var additionalParameters = string.IsNullOrWhiteSpace(@event.Type)
            ? string.Empty
            : $", {GenerateEventArgsType(@event)} args";
        var args = string.IsNullOrWhiteSpace(@event.Type)
            ? "System.EventArgs.Empty".WithGlobalPrefix()
            : "args";
        var modifiers = @event.IsAttached
            ? " static"
            : string.Empty;

        switch (@class.Framework)
        {
            // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/events/weak-event-patterns
            case Framework.Wpf:
            {
                var source = @event.IsAttached
                    ? @class.Name
                    : $"(source as {@class.Name})!";

                var eventHandlerType = GenerateEventHandlerType(@event);
                var eventArgsType = GenerateEventArgsType(@event);

                writer.AppendLine();
                writer.AppendLine("#nullable enable");
                writer.AppendLine();
                writer.AppendLine($"namespace {@class.Namespace}");
                writer.AppendLine("{");
                writer.AppendLine($"    {@class.Modifiers}partial class {@class.Name}");
                writer.AppendLine("    {");
                GenerateGeneratedCodeAttribute(ref writer, @class.Version);
                GenerateExcludeFromCodeCoverageAttribute(ref writer);
                writer.AppendLine($"        private class {@event.Name}WeakEventManager : global::System.Windows.WeakEventManager");
                writer.AppendLine("        {");
                writer.AppendLine($"            private {@event.Name}WeakEventManager()");
                writer.AppendLine("            {");
                writer.AppendLine("            }");
                writer.AppendLine();
                writer.AppendLine($"            public static void AddHandler(object? source, {eventHandlerType} handler)");
                writer.AppendLine("            {");
                writer.AppendLine("                if (source == null)");
                writer.AppendLine("                    throw new global::System.ArgumentNullException(nameof(source));");
                writer.AppendLine("                if (handler == null)");
                writer.AppendLine("                    throw new global::System.ArgumentNullException(nameof(handler));");
                writer.AppendLine();
                writer.AppendLine("                CurrentManager.ProtectedAddHandler(source, handler);");
                writer.AppendLine("            }");
                writer.AppendLine();
                writer.AppendLine($"            public static void RemoveHandler(object? source, {eventHandlerType} handler)");
                writer.AppendLine("            {");
                writer.AppendLine("                if (source == null)");
                writer.AppendLine("                    throw new global::System.ArgumentNullException(nameof(source));");
                writer.AppendLine("                if (handler == null)");
                writer.AppendLine("                    throw new global::System.ArgumentNullException(nameof(handler));");
                writer.AppendLine();
                writer.AppendLine("                CurrentManager.ProtectedRemoveHandler(source, handler);");
                writer.AppendLine("            }");
                writer.AppendLine();
                writer.AppendLine($"            internal static {@event.Name}WeakEventManager CurrentManager");
                writer.AppendLine("            {");
                writer.AppendLine("                get");
                writer.AppendLine("                {");
                writer.AppendLine($"                    var managerType = typeof({@event.Name}WeakEventManager);");
                writer.AppendLine($"                    var manager = ({@event.Name}WeakEventManager)GetCurrentManager(managerType);");
                writer.AppendLine("                    if (manager == null)");
                writer.AppendLine("                    {");
                writer.AppendLine($"                        manager = new {@event.Name}WeakEventManager();");
                writer.AppendLine("                        SetCurrentManager(managerType, manager);");
                writer.AppendLine("                    }");
                writer.AppendLine();
                writer.AppendLine("                    return manager;");
                writer.AppendLine("                }");
                writer.AppendLine("            }");
                writer.AppendLine();
                writer.AppendLine("            protected override void StartListening(object? source)");
                writer.AppendLine("            {");
                writer.AppendLine($"                {source}.{@event.Name} += On{@event.Name};");
                writer.AppendLine("            }");
                writer.AppendLine();
                writer.AppendLine("            protected override void StopListening(object? source)");
                writer.AppendLine("            {");
                writer.AppendLine($"                {source}.{@event.Name} -= On{@event.Name};");
                writer.AppendLine("            }");
                writer.AppendLine();
                writer.AppendLine($"            internal void On{@event.Name}(object? sender, {eventArgsType} args)");
                writer.AppendLine("            {");
                writer.AppendLine("                DeliverEvent(sender, args);");
                writer.AppendLine("            }");
                writer.AppendLine("        }");
                writer.AppendLine();
                GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
                GenerateGeneratedCodeAttribute(ref writer, @class.Version);
                GenerateExcludeFromCodeCoverageAttribute(ref writer);
                writer.AppendLine($"        public{modifiers} event {eventHandlerType} {@event.Name}");
                writer.AppendLine("        {");
                writer.AppendLine($"            add => {@event.Name}WeakEventManager.AddHandler(null, value);");
                writer.AppendLine($"            remove => {@event.Name}WeakEventManager.RemoveHandler(null, value);");
                writer.AppendLine("        }");
                writer.AppendLine();
                writer.AppendLine("        /// <summary>");
                writer.AppendLine($"        /// A helper method to raise the {@event.Name} event.");
                writer.AppendLine("        /// </summary>");
                GenerateGeneratedCodeAttribute(ref writer, @class.Version);
                GenerateExcludeFromCodeCoverageAttribute(ref writer);
                writer.AppendLine($"        internal{modifiers} void Raise{@event.Name}Event(object? sender{additionalParameters})");
                writer.AppendLine("        {");
                writer.AppendLine($"            {@event.Name}WeakEventManager.CurrentManager.On{@event.Name}(sender, {args});");
                writer.AppendLine("        }");
                writer.AppendLine("    }");
                writer.AppendLine("}");
                break;
            }

            // https://github.com/dotnet/maui/issues/2703
            // https://github.com/dotnet/maui/pull/12950
            case Framework.Maui:
            {
                var nullable = !@event.Type.Contains("EventArgs");

                var eventHandlerType = GenerateEventHandlerType(@event, nullable: nullable, nullableType: nullable);

                writer.AppendLine();
                writer.AppendLine("#nullable enable");
                writer.AppendLine();
                writer.AppendLine($"namespace {@class.Namespace}");
                writer.AppendLine("{");
                writer.AppendLine($"    {@class.Modifiers}partial class {@class.Name}");
                writer.AppendLine("    {");
                writer.AppendLine($"        private{modifiers} global::Microsoft.Maui.WeakEventManager {@event.Name}WeakEventManager {{ get; }} = new global::Microsoft.Maui.WeakEventManager();");
                writer.AppendLine();
                GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
                writer.AppendLine($"        public{modifiers} event {eventHandlerType} {@event.Name}");
                writer.AppendLine("        {");
                writer.AppendLine($"            add => {@event.Name}WeakEventManager.AddEventHandler(value);");
                writer.AppendLine($"            remove => {@event.Name}WeakEventManager.RemoveEventHandler(value);");
                writer.AppendLine("        }");
                writer.AppendLine();
                writer.AppendLine("        /// <summary>");
                writer.AppendLine($"        /// A helper method to raise the {@event.Name} event.");
                writer.AppendLine("        /// </summary>");
                writer.AppendLine($"        internal{modifiers} void Raise{@event.Name}Event(object? sender{additionalParameters})");
                writer.AppendLine("        {");
                writer.AppendLine($"            {@event.Name}WeakEventManager.HandleEvent(sender!, {args}!, eventName: nameof({@event.Name}));");
                writer.AppendLine("        }");
                writer.AppendLine("    }");
                writer.AppendLine("}");
                break;
            }

            case Framework.Uwp:
            case Framework.WinUi:
            case Framework.Uno:
            case Framework.UnoWinUi:
            case Framework.Avalonia:
            default:
                break;
        }
    }

    private static string GenerateEventHandlerType(EventData @event, bool nullable = true, bool nullableType = true)
    {
        var eventHandler = (string.IsNullOrWhiteSpace(@event.Type)
            ? "System.EventHandler"
            : $"System.EventHandler<{GenerateType(@event, nullable: nullableType)}>").WithGlobalPrefix();
        if (nullable) eventHandler += "?";

        return eventHandler;
    }
}

