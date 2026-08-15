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
                
                using (writer.Scope($"_ = this.RegisterPropertyChangedCallback(dp: {property.Name}Property, callback: static (sender, dependencyProperty) =>", "});"))
                {
                    var senderCast = $"(({senderType})sender)";
                    var getValue = $"({type})sender.GetValue(dependencyProperty)";

                    if (signatures.HasFlag(CallbackSignature.NoParameters))
                    {
                        writer.AppendLine($"{senderCast}.{name}();");
                    }

                    if (signatures.HasFlag(CallbackSignature.NewValue))
                    {
                        writer.AppendLine($"{senderCast}.{name}({getValue});");
                    }

                    if (signatures.HasFlag(CallbackSignature.OldAndNewValue))
                    {
                        writer.AppendLine($"{senderCast}.{name}(default({type}), {getValue});");
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

