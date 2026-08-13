using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources.Strategies;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateStaticConstructor(
        ref SourceWriter writer,
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> properties)
    {
        var generator = FrameworkGeneratorFactory.CreateDependencyPropertyStrategy(@class.Framework);
        generator.GenerateStaticConstructor(ref writer, @class, properties);
    }
}

