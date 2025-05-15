using System.Collections;
using System.Collections.Generic;

namespace Hopeful.Injection;

public static class CollectionExtensions
{
    public static bool IsEmpty(this ICollection collection)
        => collection.Count == 0;
}
