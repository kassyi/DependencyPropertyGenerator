namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct FrameworkMetadataData(
    bool AffectsMeasure,
    bool AffectsArrange,
    bool AffectsParentMeasure,
    bool AffectsParentArrange,
    bool AffectsRender,
    bool Inherits,
    bool OverridesInheritanceBehavior,
    bool NotDataBindable,
    bool Journal,
    bool SubPropertiesDoNotAffectRender,
    bool IsAnimationProhibited,
    string? DefaultUpdateSourceTrigger,
    string? DefaultBindingMode);
