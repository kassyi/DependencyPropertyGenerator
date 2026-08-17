namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Contains short names of generator attributes to quickly match syntax nodes before semantic analysis.</summary>
internal static class KnownAttributeShortNames
{
    public const string DependencyProperty = "DependencyProperty";
    public const string AttachedDependencyProperty = "AttachedDependencyProperty";
    public const string RoutedEvent = "RoutedEvent";
    public const string WeakEvent = "WeakEvent";
    public const string AddOwner = "AddOwner";
    public const string OverrideMetadata = "OverrideMetadata";
}

/// <summary>Contains fully qualified names of generator attributes for exact semantic matching.</summary>
internal static class KnownAttributes
{
    public const string Namespace = "Kassyi.Generators.DependencyProperty.";

    public const string DependencyPropertyAttribute = nameof(global::Kassyi.Generators.DependencyProperty.DependencyPropertyAttribute);
    public const string AttachedDependencyPropertyAttribute = nameof(global::Kassyi.Generators.DependencyProperty.AttachedDependencyPropertyAttribute);
    public const string RoutedEventAttribute = nameof(global::Kassyi.Generators.DependencyProperty.RoutedEventAttribute);
    public const string WeakEventAttribute = nameof(global::Kassyi.Generators.DependencyProperty.WeakEventAttribute);
    public const string AddOwnerAttribute = nameof(global::Kassyi.Generators.DependencyProperty.AddOwnerAttribute);
    public const string OverrideMetadataAttribute = nameof(global::Kassyi.Generators.DependencyProperty.OverrideMetadataAttribute);

    public const string DependencyProperty = Namespace + DependencyPropertyAttribute;
    public const string AttachedDependencyProperty = Namespace + AttachedDependencyPropertyAttribute;
    public const string RoutedEvent = Namespace + RoutedEventAttribute;
    public const string WeakEvent = Namespace + WeakEventAttribute;
    public const string AddOwner = Namespace + AddOwnerAttribute;
    public const string OverrideMetadata = Namespace + OverrideMetadataAttribute;
}

/// <summary>Contains names of dependency property types used across various UI frameworks.</summary>
internal static class KnownPropertyTypes
{
    public const string DependencyProperty = "DependencyProperty";
    public const string StyledProperty = "StyledProperty";
    public const string DirectProperty = "DirectProperty";
    public const string AttachedProperty = "AttachedProperty";
    public const string BindableProperty = "BindableProperty";
    public const string DependencyPropertyKey = "DependencyPropertyKey";
    public const string BindablePropertyKey = "BindablePropertyKey";
}

/// <summary>Contains framework-specific method names used during source generation (e.g., Register, GetValue).</summary>
internal static class KnownMethodNames
{
    public const string Register = "Register";
    public const string RegisterAttached = "RegisterAttached";
    public const string RegisterReadOnly = "RegisterReadOnly";
    public const string RegisterAttachedReadOnly = "RegisterAttachedReadOnly";
    public const string RegisterDirect = "RegisterDirect";
    public const string Create = "Create";
    public const string CreateAttached = "CreateAttached";
    public const string CreateReadOnly = "CreateReadOnly";
    public const string CreateAttachedReadOnly = "CreateAttachedReadOnly";

    public const string GetValue = "GetValue";
    public const string SetValue = "SetValue";
    public const string SetAndRaise = "SetAndRaise";
}

