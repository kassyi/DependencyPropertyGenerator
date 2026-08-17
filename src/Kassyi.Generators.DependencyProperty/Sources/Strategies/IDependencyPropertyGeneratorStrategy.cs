using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Defines the contract for framework-specific dependency property source generation strategies.</summary>
internal interface IDependencyPropertyGeneratorStrategy
{
    /// <summary>Generates the argument string passed to the dependency property registration method.</summary>
    string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property);
    /// <summary>Generates the complete registration method call for a dependency property.</summary>
    string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property);
    /// <summary>Generates the AddOwner registration method call for an existing dependency property.</summary>
    string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property);
    /// <summary>Generates the framework-specific type string for a dependency property identifier.</summary>
    string GeneratePropertyType(ClassData @class, DependencyPropertyData property);
    /// <summary>Generates the framework-specific property manager type used for binding and updates.</summary>
    string GenerateManagerType(ClassData @class);
    /// <summary>Generates a static constructor for registering dependency properties if required by the framework.</summary>
    void GenerateStaticConstructor(ref SourceWriter writer, ClassData @class, IReadOnlyCollection<DependencyPropertyData> properties);
    /// <summary>Generates backing fields required specifically for Direct or Attached properties.</summary>
    void GenerateAdditionalFieldForDirectProperties(ref SourceWriter writer, DependencyPropertyData property);
    /// <summary>Generates read-only property accessors if required by the dependency property configuration.</summary>
    void GenerateAdditionalPropertyForReadOnlyProperties(ref SourceWriter writer, DependencyPropertyData property);
}
