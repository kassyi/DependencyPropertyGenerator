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
        writer.AppendLine();
        writer.AppendLine("#nullable enable");
        writer.AppendLine();
        writer.AppendLine($"namespace {@class.Namespace}");
        writer.AppendLine("{");
        writer.AppendLine($"    {GenerateModifiers(@class)}partial class {@class.Name}");
        writer.AppendLine("    {");
        writer.AppendLine("        private void RegisterPropertyChangedCallbacks()");
        writer.AppendLine("        {");

        foreach (var property in overrideMetadata)
        {
            var senderType = property.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type;

            var (name, isChanged0, isChanged1, isChanged2, isChanged3, _, _) = CheckOnChangedMethods(@class, property);
            if (!isChanged0 && !isChanged1 && !isChanged2 && !isChanged3)
            {
                continue;
            }

            var type = GenerateType(property);
            
            writer.AppendLine("            _ = this.RegisterPropertyChangedCallback(");
            writer.AppendLine($"                dp: {property.Name}Property,");
            writer.AppendLine("                callback: static (sender, dependencyProperty) =>");
            writer.AppendLine("                {");
            if (isChanged0) writer.AppendLine($"                    (({senderType})sender).{name}();");
            if (isChanged1)
            {
                writer.AppendLine($"                    (({senderType})sender).{name}(");
                writer.AppendLine($"                                            ({type})sender.GetValue(dependencyProperty));");
            }
            if (isChanged2)
            {
                writer.AppendLine($"                    (({senderType})sender).{name}(");
                writer.AppendLine($"                                            ({type})sender.GetValue(dependencyProperty),");
                writer.AppendLine($"                                            ({type})sender.GetValue(dependencyProperty));");
            }
            writer.AppendLine("                });");
            writer.AppendLine();
        }

        writer.AppendLine("        }");

        foreach (var property in overrideMetadata)
        {
            GenerateOnChangedMethods(ref writer, @class, property);
        }

        writer.AppendLine("    }");
        writer.AppendLine("}");
    }
}

