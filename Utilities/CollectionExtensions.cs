using System.Collections;

namespace Hopeful.Utilities;

/// <summary>
///   <para>Provides a set of extension methods for the <see cref="ICollection"/> interface.</para>
/// </summary>
public static class CollectionExtensions
{
    /// <summary> 
    /// Returns a value indicating whether the specified collection is empty.
    /// </summary>
    public static bool IsEmpty(this ICollection collection)
        => collection.Count == 0;

    /// <summary>
    /// Returns a value indicating whether the specified collection is not empty.
    /// </summary>
    public static bool IsNotEmpty(this ICollection collection)
        => collection.Count != 0;
}
