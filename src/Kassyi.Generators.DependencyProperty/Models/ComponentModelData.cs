namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct ComponentModelData(
    string? Description,
    string? Category,
    string? TypeConverter,
    bool? Bindable,
    bool? Browsable,
    string? DesignerSerializationVisibility,
    bool? ClsCompliant,
    string? Localizability,
    string? BrowsableForType,
    string? FromType);
