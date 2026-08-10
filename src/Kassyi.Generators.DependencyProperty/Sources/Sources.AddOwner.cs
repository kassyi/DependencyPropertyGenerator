using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    private static string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => property.IsDirect
                ? $"""

                               {property.FromType}.{property.Name}Property.AddOwner<{@class.Type}>(
                                   {GenerateAvaloniaRegisterMethodArguments(@class, property)});
                   """
                : $"""

                               {property.FromType}.{property.Name}Property.AddOwner<{@class.Type}>(
                                   {GeneratePropertyMetadata(@class, property)});
                   """,
            _ => $"""

                              {property.FromType}.{property.Name}Property.AddOwner(
                                  ownerType: typeof({@class.Type}),
                                  {GeneratePropertyMetadata(@class, property)});
                      
                  """.RemoveBlankLinesWhereOnlyWhitespaces()
        };
    }
}

