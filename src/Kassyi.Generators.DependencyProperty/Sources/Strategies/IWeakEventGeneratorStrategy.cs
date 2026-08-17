using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Defines the contract for generating weak event patterns.</summary>
internal interface IWeakEventGeneratorStrategy
{
    /// <summary>Generates the weak event manager and accessors for a specified event.</summary>
    void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event);
}
