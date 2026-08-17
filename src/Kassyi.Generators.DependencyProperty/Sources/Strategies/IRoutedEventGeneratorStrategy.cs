using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Defines the contract for framework-specific routed event source generation strategies.</summary>
internal interface IRoutedEventGeneratorStrategy
{
    /// <summary>Generates the registration and accessors for a standard routed event.</summary>
    void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event);
    /// <summary>Generates the registration and accessors for an attached routed event.</summary>
    void GenerateAttachedRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event);
}
