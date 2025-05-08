using JetBrains.Annotations;
using System;

namespace Hopeful.Injection;

public static class VaultExtensions
{
    [Pure]
    public static T InjectService<T>(this Vault v, object? key = null)
        => (T)v.InjectService(typeof(T), key);

    [Pure]
    public static void ExtractService<T>(this Vault v, object? key = null)
        => v.ExtractService(typeof(T), typeof(T), key);

    [Pure]
    public static void ExtractService(this Vault v, object instance, object? key = null)
        => v.ExtractService(instance.GetType(), instance, key);

    [Pure]
    public static void ExtractService<T>(this Vault v, Type type, object? key = null)
        => v.ExtractService(typeof(T), type, key);
}
