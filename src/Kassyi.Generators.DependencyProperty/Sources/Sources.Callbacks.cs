using Kassyi.Generators.DependencyProperty.Diagnostics;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{

    internal static void GenerateOnChangedMethods(ref SourceWriter writer, ClassData @class, DependencyPropertyData property) =>
        GenerateOnMethods(ref writer, @class, property, "Changed", true);

    internal static void GenerateOnChangingMethods(ref SourceWriter writer, ClassData @class, DependencyPropertyData property) =>
        GenerateOnMethods(ref writer, @class, property, "Changing", false);

    private static void GenerateOnMethods(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string suffix, bool checkExists)
    {
        if (checkExists)
        {
            if (!string.IsNullOrWhiteSpace(property.ValidationAndCallbacks.OnChanged))
            {
                var (_, callbacks) = CheckOnChangedMethods(@class, property);
                if (callbacks.ChangedSignatures == CallbackSignature.None)
                {
                    writer.AppendLine(
                        DiagnosticDescriptors.FormatDpg0001Error(property.ValidationAndCallbacks.OnChanged, @class.FullName));
                }

                return;
            }
        }
        else if (property.Framework != Framework.Maui)
        {
            return;
        }

        var type = GenerateType(property);
        var browsable = GenerateBrowsableForType(property);
        var browsableName = GenerateBrowsableForTypeParameterName(property);
        var name = property.Name;

        writer.AppendLine();
        if (property.Modifiers.IsAttached)
        {
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"static partial void On{name}{suffix}();");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"static partial void On{name}{suffix}({browsable} {browsableName});");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"static partial void On{name}{suffix}({browsable} {browsableName}, {type} newValue);");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"static partial void On{name}{suffix}({browsable} {browsableName}, {type} oldValue, {type} newValue);");
        }
        else
        {
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"partial void On{name}{suffix}();");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"partial void On{name}{suffix}({type} newValue);");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"partial void On{name}{suffix}({type} oldValue, {type} newValue);");
        }
    }
    
    internal static (string Name, EventCallbackData Callbacks) CheckOnChangedMethods(ClassData @class, DependencyPropertyData property)
    {
        var isCustom = !string.IsNullOrWhiteSpace(property.ValidationAndCallbacks.OnChanged);
        var name = isCustom
            ? property.ValidationAndCallbacks.OnChanged
            : $"On{property.Name}Changed";

        return (name, property.ValidationAndCallbacks.Callbacks);
    }

}

