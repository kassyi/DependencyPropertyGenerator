// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Kassyi.Generators.Extensions;

/// <summary>Extensions for <see cref="EquatableArray{T}"/>.</summary>
public static class EquatableArray
{
    /// <summary>Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</summary>
    public static EquatableArray<T> AsEquatableArray<T>(this ImmutableArray<T> array)
        where T : IEquatable<T> => [with(array)];
}

/// <summary>An immutable, equatable array wrapper providing value equality support for incremental generators.</summary>
public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly T[]? _array;

    /// <summary>Creates a new <see cref="EquatableArray{T}"/> instance.</summary>
    public EquatableArray(ImmutableArray<T> array)
    {
        this._array = Unsafe.As<ImmutableArray<T>, T[]?>(ref array);
    }

    /// <summary>Gets a reference to an item at a specified position within the array.</summary>
    public ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref AsImmutableArray().ItemRef(index);
    }

    /// <summary>Gets a value indicating whether the current array is empty.</summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AsImmutableArray().IsEmpty;
    }

    /// <inheritdoc/>
    public bool Equals(EquatableArray<T> other) => AsSpan().SequenceEqual(other.AsSpan());

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(this, other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (_array is not { } other)
        {
            return 0;
        }

        HashCode hashCode = default;

        foreach (var item in other)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>Gets the underlying <see cref="ImmutableArray{T}"/> instance.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableArray<T> AsImmutableArray() => Unsafe.As<T[]?, ImmutableArray<T>>(ref Unsafe.AsRef(in _array));

    /// <summary>Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
        Justification = "Factory method pattern for EquatableArray<T> struct.")]
    public static EquatableArray<T> FromImmutableArray(ImmutableArray<T> array) => [with(array)];

    /// <summary>Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.</summary>
    public ReadOnlySpan<T> AsSpan() => AsImmutableArray().AsSpan();

    /// <summary>Copies the contents of this instance to a new mutable array.</summary>
    public T[] ToArray() => [.. AsSpan()];

    /// <summary>Gets an enumerator to traverse items in the current array.</summary>
    public ImmutableArray<T>.Enumerator GetEnumerator() => AsImmutableArray().GetEnumerator();

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)AsImmutableArray()).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)AsImmutableArray()).GetEnumerator();

    /// <summary>Implicitly converts an <see cref="ImmutableArray{T}"/> to <see cref="EquatableArray{T}"/>.</summary>
    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => FromImmutableArray(array);

    /// <summary>Implicitly converts an <see cref="EquatableArray{T}"/> to <see cref="ImmutableArray{T}"/>.</summary>
    public static implicit operator ImmutableArray<T>(EquatableArray<T> array) => array.AsImmutableArray();

    /// <summary>Checks whether two <see cref="EquatableArray{T}"/> values are equal.</summary>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);

    /// <summary>Checks whether two <see cref="EquatableArray{T}"/> values are not equal.</summary>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

    /// <summary>Converts the current instance to an <see cref="ImmutableArray{T}"/>.</summary>
    public ImmutableArray<T> ToImmutableArray() => throw new NotImplementedException();
}
