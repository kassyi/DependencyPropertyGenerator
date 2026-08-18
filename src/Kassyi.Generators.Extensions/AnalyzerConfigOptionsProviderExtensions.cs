using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides extension methods for <see cref="AnalyzerConfigOptionsProvider"/> to resolve MSBuild build properties and options.</summary>
public static class AnalyzerConfigOptionsProviderExtensions
{
    private static string GetFullName(string name, string? prefix = null)
    {
        return prefix == null
            ? name
            : $"{prefix}_{name}";
    }

    /// <summary>Returns the value of the option, or <see langword="null"/> if missing or whitespace.</summary>
    public static string? GetOption(
        this AnalyzerConfigOptions options,
        string key)
    {
        options = options ?? throw new ArgumentNullException(nameof(options));
        key = key ?? throw new ArgumentNullException(nameof(key));

        return
            options.TryGetValue(key, out var result) &&
            !string.IsNullOrWhiteSpace(result)
                ? result
                : null;
    }

    /// <summary>Returns the value of the global MSBuild property, or <see langword="null"/> if missing or whitespace.</summary>
    public static string? GetGlobalOption(
        this AnalyzerConfigOptionsProvider provider,
        string name,
        string? prefix = null)
    {
        provider = provider ?? throw new ArgumentNullException(nameof(provider));
        name = name ?? throw new ArgumentNullException(nameof(name));

        return provider.GlobalOptions.GetOption($"build_property.{GetFullName(name, prefix)}");
    }

    /// <summary>Returns the value of the <see cref="AdditionalText"/> metadata option, or <see langword="null"/> if missing or whitespace.</summary>
    public static string? GetOption(
        this AnalyzerConfigOptionsProvider provider,
        AdditionalText text,
        string name,
        string? group = null,
        string? prefix = null)
    {
        provider = provider ?? throw new ArgumentNullException(nameof(provider));
        name = name ?? throw new ArgumentNullException(nameof(name));
        group ??= "AdditionalFiles";

        return provider.GetOptions(text).GetOption($"build_metadata.{group}.{GetFullName(name, prefix)}");
    }

    /// <summary>Returns the value of the required global MSBuild property, throwing <see cref="InvalidOperationException"/> if missing.</summary>
    public static string GetRequiredGlobalOption(
        this AnalyzerConfigOptionsProvider provider,
        string name,
        string? prefix = null)
    {
        return
            provider.GetGlobalOption(name, prefix) ??
            throw new InvalidOperationException($"{GetFullName(name, prefix)} MSBuild property is required.");
    }

    /// <summary>Returns the value of the required <see cref="AdditionalText"/> option, throwing <see cref="InvalidOperationException"/> if missing.</summary>
    public static string GetRequiredOption(
        this AnalyzerConfigOptionsProvider provider,
        AdditionalText text,
        string name,
        string? prefix = null)
    {
        return
            provider.GetOption(text, name, prefix) ??
            throw new InvalidOperationException($"{GetFullName(name, prefix)} metadata for AdditionalText is required.");
    }

    /// <summary>Determines whether the generator is executing within a design-time build context.</summary>
    public static bool IsDesignTime(this AnalyzerConfigOptionsProvider provider)
    {
        var isBuildingProjectValue = provider.GetGlobalOption("BuildingProject"); // [WHY] Legacy MSBuild projects set BuildingProject=false during design-time builds
        var isDesignTimeBuildValue = provider.GetGlobalOption("DesignTimeBuild"); // [WHY] SDK-style projects set DesignTimeBuild=true during design-time builds

        return string.Equals(isBuildingProjectValue, "false", StringComparison.OrdinalIgnoreCase)
            || string.Equals(isDesignTimeBuildValue, "true", StringComparison.OrdinalIgnoreCase);
    }
}

