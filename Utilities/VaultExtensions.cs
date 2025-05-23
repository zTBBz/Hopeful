using JetBrains.Annotations;
using System;

namespace Hopeful.Utilities;

public static class VaultExtensions
{
    [Pure]
    public static TService InjectService<TService>(this Vault v, object? key = null)
        => (TService)v.InjectService(typeof(TService), key);

    [Pure]
    public static TService InjectService<TService, TDecorator>(this Vault v, object? key = null)
        => (TService)v.InjectService(typeof(TService), key, typeof(TDecorator));

    [Pure]
    public static void ExtractService<TService>(this Vault v, object? key = null)
        => v.ExtractService(typeof(TService), typeof(TService), key);

    [Pure]
    public static void ExtractService<TService>(this Vault v, object instance, object? key = null)
    => v.ExtractService(typeof(TService), instance, key);

    [Pure]
    public static void ExtractService(this Vault v, object instance, object? key = null)
        => v.ExtractService(instance.GetType(), instance, key);

    [Pure]
    public static void ExtractService<TService, TInstance>(this Vault v, object? key = null)
        => v.ExtractService(typeof(TService), typeof(TInstance), key);

    [Pure]
    public static void ExtractDecorator<TService, TDecorator>(this Vault v, object? serviceKey = null) where TDecorator : TService
    {
        System.Reflection.ConstructorInfo ctor = typeof(TDecorator).GetConstructor([typeof(TService)]) ?? throw new InvalidOperationException($"Decorator Type {typeof(TDecorator)} must have constructor accepting {typeof(TService)}");
        v.ExtractDecorator<TService>(s => (TService)ctor.Invoke([s]), serviceKey);
    }

    [Pure]
    public static void ExtractDecorator<TService>(this Vault v, Func<TService, TService> decorator, object? serviceKey = null)
        => v.ExtractDecorator(typeof(TService), inner => decorator((TService)inner!)!, serviceKey);
}
