using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateRoutedEventSource(ClassData @class, EventData @event)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateRoutedEvent(ref writer, @class, @event);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    public static void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var generator = Strategies.FrameworkGeneratorFactory.CreateRoutedEventStrategy(@class.Framework);
        
        switch (@event.IsAttached)
        {
            case true:
                generator.GenerateAttachedRoutedEvent(ref writer, @class, @event);
                return;
            default:
                generator.GenerateRoutedEvent(ref writer, @class, @event);
                return;
        }
    }
}

