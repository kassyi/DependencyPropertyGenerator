using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions;

/// <summary>
/// 
/// </summary>
public static class ConvertExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="typedConstant"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static bool ToBoolean(this TypedConstant typedConstant, bool defaultValue = false)
    {
        switch (typedConstant.Value)
        {
            case null:
                return defaultValue;
            default:
                return (bool)typedConstant.Value!;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typedConstant"></param>
    /// <returns></returns>
    public static bool? ToNullableBoolean(this TypedConstant typedConstant)
    {
        switch (typedConstant.Value)
        {
            case null:
                return null;
            default:
                return (bool)typedConstant.Value!;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typedConstant"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ToEnum<T>(this TypedConstant typedConstant, T defaultValue) where T : Enum
    {
        return (T)(typedConstant.Value ?? defaultValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typedConstant"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? ToEnum<T>(this TypedConstant typedConstant) where T : struct, Enum
    {
        switch (typedConstant.Value)
        {
            case null:
                return null;
            default:
                return (T)typedConstant.Value;
        }
    }
}
