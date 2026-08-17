using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal interface IWeakEventGeneratorStrategy
{
    void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event);
}
