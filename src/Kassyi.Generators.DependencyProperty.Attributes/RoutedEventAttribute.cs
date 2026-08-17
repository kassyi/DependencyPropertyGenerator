// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#nullable enable

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Kassyi.Generators.DependencyProperty;

/// <summary>Generates routed event using EventManager.RegisterRoutedEvent.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[Conditional("DEPENDENCY_PROPERTY_GENERATOR_ATTRIBUTES")]
internal sealed class RoutedEventAttribute : Attribute
{
    /// <summary>Name of this routed event.</summary>
	public string Name { get; }

    /// <summary>Strategy of this routed event.</summary>
    public RoutedEventStrategy Strategy { get; }

    /// <summary>Type of this routed event. Default - typeof(RoutedEventHandler).</summary>
    public Type? Type { get; set; }

    /// <summary>Generates attached routed event. Default - <see langword="false"/>.</summary>
    public bool IsAttached { get; set; }

    /// <summary>Description of this routed event. The event will contain a <see cref="DescriptionAttribute"/> with this value. This will also be used in the xml documentation if not explicitly specified. Default - <see langword="null"/>.</summary>
    public string? Description { get; set; }

    /// <summary>Category of this routed event. The event will contain a <see cref="CategoryAttribute"/> with this value. Default - <see langword="null"/>.</summary>
    public string? Category { get; set; }

    /// <summary>The routed event xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string XmlDocumentation { get; set; } = string.Empty;

    /// <summary>The event add/remove xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string EventXmlDocumentation { get; set; } = string.Empty;

    /// <summary>WinRT events are disabled by default due to known event registration and lifetime issues in Windows 10. Default - <see langword="false"/>.</summary>
    public bool WinRtEvents { get; set; }
public RoutedEventAttribute(
        string name,
        RoutedEventStrategy strategy)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Strategy = strategy;
    }
}

/// <summary>Generates routed event using EventManager.RegisterRoutedEvent.</summary>
/// <typeparam name="T">Type of this routed event.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[Conditional("DEPENDENCY_PROPERTY_GENERATOR_ATTRIBUTES")]
internal sealed class RoutedEventAttribute<T> : Attribute
{
    /// <summary>Name of this routed event.</summary>
	public string Name { get; }

    /// <summary>Strategy of this routed event.</summary>
    public RoutedEventStrategy Strategy { get; }

    /// <summary>Type of this routed event. Default - typeof(RoutedEventHandler).</summary>
    public Type? Type { get; set; }

    /// <summary>Generates attached routed event. Default - <see langword="false"/>.</summary>
    public bool IsAttached { get; set; }

    /// <summary>Description of this routed event. The event will contain a <see cref="DescriptionAttribute"/> with this value. This will also be used in the xml documentation if not explicitly specified. Default - <see langword="null"/>.</summary>
    public string? Description { get; set; }

    /// <summary>Category of this routed event. The event will contain a <see cref="CategoryAttribute"/> with this value. Default - <see langword="null"/>.</summary>
    public string? Category { get; set; }

    /// <summary>The routed event xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string XmlDocumentation { get; set; } = string.Empty;

    /// <summary>The event add/remove xml documentation. Default - "&lt;summary&gt;&lt;/summary&gt;".</summary>
    public string EventXmlDocumentation { get; set; } = string.Empty;

    /// <summary>WinRT events are disabled by default due to known event registration and lifetime issues in Windows 10. Default - <see langword="false"/>.</summary>
    public bool WinRtEvents { get; set; }
public RoutedEventAttribute(
        string name,
        RoutedEventStrategy strategy)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Strategy = strategy;
        Type = typeof(T);
    }
}
