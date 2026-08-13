using Kassyi.Generators.Extensions;
using Kassyi.Generators.DependencyProperty.Models;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

internal interface IDependencyPropertyGeneratorStrategy
{
    string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property);
    string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property);
    string GenerateAddOwnerCreateCall(ClassData @class, DependencyPropertyData property);
    string GeneratePropertyType(ClassData @class, DependencyPropertyData property);
    string GenerateManagerType(ClassData @class);
    void GenerateStaticConstructor(ref SourceWriter writer, ClassData @class, IReadOnlyCollection<DependencyPropertyData> properties);
    void GenerateAdditionalFieldForDirectProperties(ref SourceWriter writer, DependencyPropertyData property);
    void GenerateAdditionalPropertyForReadOnlyProperties(ref SourceWriter writer, DependencyPropertyData property);
}
