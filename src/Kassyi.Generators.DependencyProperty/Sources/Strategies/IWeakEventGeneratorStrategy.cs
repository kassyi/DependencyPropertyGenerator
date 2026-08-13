using Kassyi.Generators.Extensions;
using Kassyi.Generators.DependencyProperty.Models;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal interface IWeakEventGeneratorStrategy
{
    void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event);
}
