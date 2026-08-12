using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var generator = Strategies.FrameworkGeneratorFactory.Create(@class.Framework);
        
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

