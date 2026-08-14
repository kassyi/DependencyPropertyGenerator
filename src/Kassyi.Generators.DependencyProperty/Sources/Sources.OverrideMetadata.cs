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
        using var _ = writer.ClassScope(@class);
        using (writer.Scope("private void RegisterPropertyChangedCallbacks()"))
        {
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
                
                writer.AppendLine($"_ = this.RegisterPropertyChangedCallback(dp: {property.Name}Property, callback: static (sender, dependencyProperty) =>");
                using (writer.Scope("{", "});"))
                {
                    var senderCast = $"(({senderType})sender)";
                    var getValue = $"({type})sender.GetValue(dependencyProperty)";

                    foreach (var args in GetCallbackArgumentSets(signatures, getValue))
                    {
                        var argsString = string.Join(", ", args);
                        writer.AppendLine($"{senderCast}.{name}({argsString});");
                    }
                }
            }
        }

        foreach (var property in overrideMetadata)
        {
            GenerateOnChangedMethods(ref writer, @class, property);
        }
    }
}

