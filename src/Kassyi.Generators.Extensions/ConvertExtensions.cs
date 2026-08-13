using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides conversion helpers for extracting typed values from <see cref="TypedConstant"/>.</summary>
public static class ConvertExtensions
{
    /// <summary>Converts the constant value to a boolean, returning the specified default if <see langword="null"/>.</summary>
    public static bool ToBoolean(this TypedConstant typedConstant, bool defaultValue = false)
    {
        if (typedConstant.Value == null)
        {
            return defaultValue;
        }

        return (bool)typedConstant.Value!;
    }

    /// <summary>Converts the constant value to a nullable boolean.</summary>
    public static bool? ToNullableBoolean(this TypedConstant typedConstant)
    {
        if (typedConstant.Value == null)
        {
            return null;
        }

        return (bool)typedConstant.Value!;
    }

    /// <summary>Converts the constant value to the specified enum type, returning the default if <see langword="null"/>.</summary>
    public static T ToEnum<T>(this TypedConstant typedConstant, T defaultValue) where T : Enum => (T)(typedConstant.Value ?? defaultValue);

    /// <summary>Converts the constant value to a nullable enum type.</summary>
    public static T? ToEnum<T>(this TypedConstant typedConstant) where T : struct, Enum
    {
        if (typedConstant.Value == null)
        {
            return null;
        }

        return (T)typedConstant.Value;
    }
}
