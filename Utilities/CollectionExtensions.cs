using System.Collections;

namespace Hopeful.Utilities;

/// <summary>
///   <para>Provides a set of extension methods for the <see cref="ICollection"/> interface.</para>
/// </summary>
public static class CollectionExtensions
{
    public static bool IsEmpty(this ICollection collection)
        => collection.Count == 0;

    public static bool IsNotEmpty(this ICollection collection)
        => collection.Count != 0;
}
