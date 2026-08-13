using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateWeakEventSource(ClassData @class, EventData @event)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateWeakEvent(ref writer, @class, @event);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    public static void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var generator = Strategies.FrameworkGeneratorFactory.CreateWeakEventStrategy(@class.Framework);
        generator.GenerateWeakEvent(ref writer, @class, @event);
    }
}

