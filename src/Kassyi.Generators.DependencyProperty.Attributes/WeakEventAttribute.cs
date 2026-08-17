// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#nullable enable

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Kassyi.Generators.DependencyProperty;

/// <summary>Generates weak event manager and event accessors.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[Conditional("DEPENDENCY_PROPERTY_GENERATOR_ATTRIBUTES")]
internal sealed class WeakEventAttribute : Attribute
{
    /// <summary>Name of this routed event.</summary>
	public string Name { get; }

    /// <summary>Type of this routed event. Default - typeof(RoutedEventHandler).</summary>
    public Type? Type { get; set; }

    /// <summary>Generates static event. Default - <see langword="false"/>.</summary>
    public bool IsStatic { get; set; }

    /// <summary>Description of this weak event. The event will contain a <see cref="DescriptionAttribute"/> with this value. This will also be used in the xml documentation if not explicitly specified. Default - <see langword="null"/>.</summary>
    public string? Description { get; set; }

    /// <summary>Category of this weak event. The event will contain a <see cref="CategoryAttribute"/> with this value. Default - <see langword="null"/>.</summary>
    public string? Category { get; set; }

    /// <summary>The weak event xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string XmlDocumentation { get; set; } = string.Empty;

    /// <summary>The event add/remove xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string EventXmlDocumentation { get; set; } = string.Empty;
public WeakEventAttribute(
        string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>Generates weak event manager and event accessors.</summary>
/// <typeparam name="T">Type of this routed event.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[Conditional("DEPENDENCY_PROPERTY_GENERATOR_ATTRIBUTES")]
internal sealed class WeakEventAttribute<T> : Attribute
{
    /// <summary>Name of this routed event.</summary>
	public string Name { get; }

    /// <summary>Type of this routed event. Default - typeof(RoutedEventHandler).</summary>
    public Type? Type { get; set; }

    /// <summary>Generates static event. Default - <see langword="false"/>.</summary>
    public bool IsStatic { get; set; }

    /// <summary>Description of this weak event. The event will contain a <see cref="DescriptionAttribute"/> with this value. This will also be used in the xml documentation if not explicitly specified. Default - <see langword="null"/>.</summary>
    public string? Description { get; set; }

    /// <summary>Category of this weak event. The event will contain a <see cref="CategoryAttribute"/> with this value. Default - <see langword="null"/>.</summary>
    public string? Category { get; set; }

    /// <summary>The weak event xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string XmlDocumentation { get; set; } = string.Empty;

    /// <summary>The event add/remove xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string EventXmlDocumentation { get; set; } = string.Empty;
public WeakEventAttribute(
        string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = typeof(T);
    }
}
