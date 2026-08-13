using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateRegisterPropertyChangedCallbacksMethod(
        ref SourceWriter writer,
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> overrideMetadata)
    {
        writer.AppendLine($$"""

        #nullable enable

        namespace {{@class.Namespace}}
        {
            {{GenerateModifiers(@class)}}partial class {{@class.Name}}
            {
                private void RegisterPropertyChangedCallbacks()
                {
        """);

        foreach (var property in overrideMetadata)
        {
            var senderType = property.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type;

            var (name, callbacks) = CheckOnChangedMethods(@class, property);
            if (callbacks is { IsChanged0: false, IsChanged1: false, IsChanged2: false, IsChanged3: false })
            {
                continue;
            }

            var type = GenerateType(property);
            
            writer.AppendLine($$"""
            _ = this.RegisterPropertyChangedCallback(
                dp: {{property.Name}}Property,
                callback: static (sender, dependencyProperty) =>
                {
""");
            writer.LineIf(callbacks.IsChanged0, $"                    (({senderType})sender).{name}();");
            writer.LineIf(callbacks.IsChanged1, $$"""
                                (({{senderType}})sender).{{name}}(
                                                        ({{type}})sender.GetValue(dependencyProperty));
                """);
            writer.LineIf(callbacks.IsChanged2, $$"""
                                (({{senderType}})sender).{{name}}(
                                                        ({{type}})sender.GetValue(dependencyProperty),
                                                        ({{type}})sender.GetValue(dependencyProperty));
                """);
            writer.AppendLine("""
                });

""");
        }

        writer.AppendLine("        }");

        foreach (var property in overrideMetadata)
        {
            GenerateOnChangedMethods(ref writer, @class, property);
        }

        writer.AppendLine("""
            }
        }
        """);
    }
}

