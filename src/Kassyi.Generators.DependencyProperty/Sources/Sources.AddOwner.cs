using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    private static void GenerateAddOwnerCreateCall(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        if (@class.Framework == Framework.Avalonia)
        {
            if (property.IsDirect)
            {
                writer.AppendLine();
                writer.Append($"""
                        {property.FromType}.{property.Name}Property.AddOwner<{@class.Type}>(
                            {GenerateAvaloniaRegisterMethodArguments(@class, property)});
            """);
            }
            else
            {
                writer.AppendLine();
                writer.Append($"""
                        {property.FromType}.{property.Name}Property.AddOwner<{@class.Type}>(
                            {GeneratePropertyMetadata(@class, property)});
            """);
            }
        }
        else
        {
            writer.AppendLine();
            writer.Append($"""
                        {property.FromType}.{property.Name}Property.AddOwner(
                            ownerType: typeof({@class.Type}),
                            {GeneratePropertyMetadata(@class, property)});
            """);
        }
    }
}

