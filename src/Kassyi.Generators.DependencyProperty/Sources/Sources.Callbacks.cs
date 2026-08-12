using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{

    internal static void GenerateOnChangedMethods(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        GenerateOnMethods(ref writer, @class, property, "Changed", true);
    }

    internal static void GenerateOnChangingMethods(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        GenerateOnMethods(ref writer, @class, property, "Changing", false);
    }

    private static void GenerateOnMethods(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string suffix, bool checkExists)
    {
        if (checkExists)
        {
            switch (string.IsNullOrWhiteSpace(property.OnChanged))
            {
                case false:
                    var (_, isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2) = CheckOnChangedMethods(@class, property);
                    if (!isChanged0 && !isChanged1 && !isChanged2 && !isChanged3 && !isChangedArgs1 && !isChangedArgs2)
                    {
                        writer.AppendLine($"#error DPG0001: The specified OnChanged method '{property.OnChanged}' was not found or has an unsupported signature on '{@class.FullName}'.");
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
        if (property.IsAttached)
        {
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"        static partial void On{name}{suffix}();");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"        static partial void On{name}{suffix}({browsable} {browsableName});");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"        static partial void On{name}{suffix}({browsable} {browsableName}, {type} newValue);");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"        static partial void On{name}{suffix}({browsable} {browsableName}, {type} oldValue, {type} newValue);");
        }
        else
        {
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"        partial void On{name}{suffix}();");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"        partial void On{name}{suffix}({type} newValue);");
            GenerateGeneratedCodeAttribute(ref writer, property.Version);
            writer.AppendLine($"        partial void On{name}{suffix}({type} oldValue, {type} newValue);");
        }
    }
    
    internal static (string Name, bool IsChanged0, bool IsChanged1, bool IsChanged2, bool IsChanged3, bool IsChangedArgs1, bool IsChangedArgs2) CheckOnChangedMethods(ClassData @class, DependencyPropertyData property)
    {
        var isCustom = !string.IsNullOrWhiteSpace(property.OnChanged);
        var name = isCustom
            ? property.OnChanged
            : $"On{property.Name}Changed";

        return (name, property.IsChanged0, property.IsChanged1, property.IsChanged2, property.IsChanged3, property.IsChangedArgs1, property.IsChangedArgs2);
    }

}

