using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal class WpfFrameworkGenerator : FrameworkGenerator
{
    public override string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        return $"""

                                          name: "{property.Name}",
                                          propertyType: typeof({property.Type}),
                                          ownerType: typeof({@class.Type}),
                                          {GeneratePropertyMetadata(@class, property)},
                                          validateValueCallback: {GenerateValidateValueCallback(@class, property)}
                          """;
    }

    public override string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        return property is { IsReadOnly: true }
            ? property.IsAttached
                ? "RegisterAttachedReadOnly"
                : "RegisterReadOnly"
            : property.IsAttached ? "RegisterAttached" : "Register";
    }

    public override void GeneratePropertyMetadata(ref Extensions.SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var defaultValue = SourceGenerationHelper.GenerateDefaultValue(property);
        var flags = SourceGenerationHelper.GenerateOptions(property);
        var propertyChanged = GeneratePropertyChangedCallback(@class, property);
        var coerceValue = GenerateCoerceValueCallback(@class, property);
        var isAnimationProhibited = property.IsAnimationProhibited.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);

        var updateSourceTrigger = property.DefaultUpdateSourceTrigger is null
            ? ""
            : $",\n                                    defaultUpdateSourceTrigger: global::System.Windows.Data.UpdateSourceTrigger.{property.DefaultUpdateSourceTrigger}";

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

    public override string GenerateManagerType(ClassData @class)
    {
        return SourceGenerationHelper.GenerateTypeByPlatform(@class.Framework, "DependencyProperty");
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

        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation, property, isProperty: false);
        writer.AppendLine($"        public static readonly {SourceGenerationHelper.GenerateTypeByPlatform(property.Framework, "DependencyProperty")} {property.Name}Property");
        writer.AppendLine($"            = {SourceGenerationHelper.GenerateDependencyPropertyName(property)}.DependencyProperty;");
    }

    public override void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        GenerateRoutedEventInternal(ref writer, @class, @event);
    }

    public override void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        GenerateAttachedRoutedEventInternal(ref writer, @class, @event);
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
        var source = @event.IsAttached
            ? @class.Name
            : $"(source as {@class.Name})!";

        var eventHandlerType = GenerateEventHandlerType(@event);
        var eventArgsType = SourceGenerationHelper.GenerateEventArgsType(@event);

        writer.AppendLine();
        writer.AppendLine("#nullable enable");
        writer.AppendLine();
        writer.AppendLine($"namespace {@class.Namespace}");
        writer.AppendLine("{");
        writer.AppendLine($"    {SourceGenerationHelper.GenerateModifiers(@class)}partial class {@class.Name}");
        writer.AppendLine("    {");
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
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
        SourceGenerationHelper.GenerateXmlDocumentationFrom(ref writer, @event.EventXmlDocumentation, @event);
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        writer.AppendLine($"        public{modifiers} event {eventHandlerType} {@event.Name}");
        writer.AppendLine("        {");
        writer.AppendLine($"            add => {@event.Name}WeakEventManager.AddHandler(null, value);");
        writer.AppendLine($"            remove => {@event.Name}WeakEventManager.RemoveHandler(null, value);");
        writer.AppendLine("        }");
        writer.AppendLine();
        writer.AppendLine("        /// <summary>");
        writer.AppendLine($"        /// A helper method to raise the {@event.Name} event.");
        writer.AppendLine("        /// </summary>");
        SourceGenerationHelper.GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        SourceGenerationHelper.GenerateExcludeFromCodeCoverageAttribute(ref writer);
        writer.AppendLine($"        internal{modifiers} void Raise{@event.Name}Event(object? sender{additionalParameters})");
        writer.AppendLine("        {");
        writer.AppendLine($"            {@event.Name}WeakEventManager.CurrentManager.On{@event.Name}(sender, {args});");
        writer.AppendLine("        }");
        writer.AppendLine("    }");
        writer.AppendLine("}");
    }

    public override void GenerateStaticConstructor(
        ref SourceWriter writer,
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> properties)
    {
        writer.AppendLine();
        writer.AppendLine("#nullable enable");
        writer.AppendLine();
        writer.AppendLine($"namespace {@class.Namespace}");
        writer.AppendLine("{");
        writer.AppendLine($"    {SourceGenerationHelper.GenerateModifiers(@class)}partial class {@class.Name}");
        writer.AppendLine("    {");
        writer.AppendLine($"        static {@class.Name}()");
        writer.AppendLine("        {");

        foreach (var property in properties)
        {
            if (property.IsReadOnly)
            {
                writer.AppendLine($"            {property.Name}Property.OverrideMetadata(");
                writer.AppendLine($"                forType: typeof({@class.Type}),");
                writer.AppendLine($"                {GeneratePropertyMetadata(@class, property)},");
                writer.AppendLine($"                key: {property.Name}PropertyKey);");
                writer.AppendLine();
            }
            else
            {
                writer.AppendLine($"            {property.Name}Property.OverrideMetadata(");
                writer.AppendLine($"                forType: typeof({@class.Type}),");
                writer.AppendLine($"                {GeneratePropertyMetadata(@class, property)});");
                writer.AppendLine();
            }
        }
        
        writer.AppendLine("        }");
        writer.AppendLine();

        foreach (var property in properties)
        {
            SourceGenerationHelper.GenerateOnChangedMethods(ref writer, @class, property);
        }

        writer.AppendLine("    }");
        writer.AppendLine("}");
    }
}
