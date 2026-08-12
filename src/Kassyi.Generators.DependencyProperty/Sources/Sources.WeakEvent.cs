using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(@class.Framework);
        generator.GenerateWeakEvent(ref writer, @class, @event);
    }
}

