using System;
using System.Collections.Generic;

namespace Hopeful.Utilities;

/// <summary>
///   <para>Provides a set of extension methods for the <see cref="List{T}"/> class.</para>
/// </summary>
public static class ListExtensions
{
    /// <summary>
    ///   <para>Removes the element at the specified index by swapping it with the last element and removing the last element. This operation is O(1) but does not preserve element order.</para>
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to remove an element from.</param>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to the number of elements in <paramref name="list"/>.</exception>
    public static void RemoveWithSwapAt<T>(this List<T> list, int index)
    {
        ArgumentNullException.ThrowIfNull(list, nameof(list));
        if (index < 0 || index >= list.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        list[index] = list[^1];
        list.RemoveAt(list.Count - 1);
    }
}