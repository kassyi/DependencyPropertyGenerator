//HintName: WeakEventAttribute.g.cs
// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#nullable enable

namespace Kassyi.Generators.DependencyProperty;

/// <summary>Generates weak event manager and event accessors.</summary>
[global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true)]
[global::System.Diagnostics.Conditional("DEPENDENCY_PROPERTY_GENERATOR_ATTRIBUTES")]
internal sealed class WeakEventAttribute : global::System.Attribute
{
    /// <summary>Name of this routed event.</summary>
	public string Name { get; }

    /// <summary>Type of this routed event. Default - typeof(RoutedEventHandler).</summary>
    public global::System.Type? Type { get; set; }

    /// <summary>Generates static event. Default - <see langword="false"/>.</summary>
    public bool IsStatic { get; set; }

    /// <summary>Description of this weak event. The event will contain a <see cref="global::System.ComponentModel.DescriptionAttribute"/> with this value. This will also be used in the xml documentation if not explicitly specified. Default - <see langword="null"/>.</summary>
    public string? Description { get; set; }

    /// <summary>Category of this weak event. The event will contain a <see cref="global::System.ComponentModel.CategoryAttribute"/> with this value. Default - <see langword="null"/>.</summary>
    public string? Category { get; set; }

    /// <summary>The weak event xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string XmlDocumentation { get; set; } = string.Empty;

    /// <summary>The event add/remove xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string EventXmlDocumentation { get; set; } = string.Empty;
public WeakEventAttribute(
        string name)
    {
        Name = name ?? throw new global::System.ArgumentNullException(nameof(name));
    }
}

/// <summary>Generates weak event manager and event accessors.</summary>
/// <typeparam name="T">Type of this routed event.</typeparam>
[global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true)]
[global::System.Diagnostics.Conditional("DEPENDENCY_PROPERTY_GENERATOR_ATTRIBUTES")]
internal sealed class WeakEventAttribute<T> : global::System.Attribute
{
    /// <summary>Name of this routed event.</summary>
	public string Name { get; }

    /// <summary>Type of this routed event. Default - typeof(RoutedEventHandler).</summary>
    public global::System.Type? Type { get; set; }

    /// <summary>Generates static event. Default - <see langword="false"/>.</summary>
    public bool IsStatic { get; set; }

    /// <summary>Description of this weak event. The event will contain a <see cref="global::System.ComponentModel.DescriptionAttribute"/> with this value. This will also be used in the xml documentation if not explicitly specified. Default - <see langword="null"/>.</summary>
    public string? Description { get; set; }

    /// <summary>Category of this weak event. The event will contain a <see cref="global::System.ComponentModel.CategoryAttribute"/> with this value. Default - <see langword="null"/>.</summary>
    public string? Category { get; set; }

    /// <summary>The weak event xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string XmlDocumentation { get; set; } = string.Empty;

    /// <summary>The event add/remove xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string EventXmlDocumentation { get; set; } = string.Empty;
public WeakEventAttribute(
        string name)
    {
        Name = name ?? throw new global::System.ArgumentNullException(nameof(name));
        Type = typeof(T);
    }
}
