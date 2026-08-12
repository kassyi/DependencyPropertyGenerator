using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources.Strategies;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    private static void GenerateAddOwnerCreateCall(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        writer.AppendLine();
        writer.Append(FrameworkGeneratorFactory.Create(@class.Framework).GenerateAddOwnerCreateCall(@class, property));
    }
}

