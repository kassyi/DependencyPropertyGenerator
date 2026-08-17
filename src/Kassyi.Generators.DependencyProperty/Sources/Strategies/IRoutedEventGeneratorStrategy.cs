using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal interface IRoutedEventGeneratorStrategy
{
    void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event);
    void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event);
}
