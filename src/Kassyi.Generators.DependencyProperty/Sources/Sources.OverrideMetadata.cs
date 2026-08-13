using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    private static IEnumerable<string[]> GetCallbackArgumentSets(CallbackSignature signatures, string getValue)
    {
        if (signatures.HasFlag(CallbackSignature.NoParameters))
        {
            yield return [];
        }

        if (signatures.HasFlag(CallbackSignature.NewValue))
        {
            yield return [getValue];
        }

        if (signatures.HasFlag(CallbackSignature.OldAndNewValue))
        {
            yield return [getValue, getValue];
        }
    }

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
            var signatures = callbacks.ChangedSignatures;
            if (signatures == CallbackSignature.None)
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
            var senderCast = $"(({senderType})sender)";
            var getValue = $"({type})sender.GetValue(dependencyProperty)";

            foreach (var args in GetCallbackArgumentSets(signatures, getValue))
            {
                if (args.Length == 0)
                {
                    writer.AppendLine($"                    {senderCast}.{name}();");
                }
                else
                {
                    var argsString = string.Join(",\n                                                        ", args);
                    writer.AppendLine($"                    {senderCast}.{name}(\n                                                        {argsString});");
                }
            }
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

