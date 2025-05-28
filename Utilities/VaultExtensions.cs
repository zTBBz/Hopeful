using JetBrains.Annotations;
using System;

namespace Hopeful.Utilities;

/// <summary>
///   <para>Provides a set of generic extension methods for the <see cref="Vault"/> class.</para>
/// </summary>
public static class VaultExtensions
{
    /// <summary>
    /// Resolves a service of the specified type from the vault.
    /// </summary>
    /// <typeparam name="TService">The type of the service to resolve.</typeparam>
    /// <param name="key">The optional key for the service.</param>
    /// <returns>The resolved service instance.</returns>
    [Pure]
    public static TService InjectService<TService>(this Vault v, object? key = null)
        => (TService)v.InjectService(typeof(TService), key);

    /// <summary>
    /// Resolves a service of the specified type from the vault, applying decorators up to the specified decorator type.
    /// </summary>
    /// <typeparam name="TService">The type of the service to resolve.</typeparam>
    /// <typeparam name="TDecorator">The decorator type at which to stop applying decorators.</typeparam>
    /// <param name="key">The optional key for the service.</param>
    /// <returns>The resolved and decorated service instance.</returns>
    [Pure]
    public static TService InjectService<TService, TDecorator>(this Vault v, object? key = null)
        => (TService)v.InjectService(typeof(TService), key, typeof(TDecorator));

    /// <summary>
    /// Registers a service of the specified type in the vault.
    /// </summary>
    /// <typeparam name="TService">The service interface or base type.</typeparam>
    /// <param name="key">The optional key for the service.</param>
    [Pure]
    public static void ExtractService<TService>(this Vault v, object? key = null)
        => v.ExtractService(typeof(TService), typeof(TService), key);

    /// <summary>
    /// Registers a service instance in the vault for the specified service type.
    /// </summary>
    /// <typeparam name="TService">The service interface or base type.</typeparam>
    /// <param name="serviceInstance">The service instance to register.</param>
    /// <param name="key">The optional key for the service.</param>
    [Pure]
    public static void ExtractService<TService>(this Vault v, object serviceInstance, object? key = null)
    => v.ExtractService(typeof(TService), serviceInstance, key);

    /// <summary>
    /// Registers a service instance in the vault using its runtime type.
    /// </summary>
    /// <param name="serviceInstance">The service instance to register.</param>
    /// <param name="key">The optional key for the service.</param>
    [Pure]
    public static void ExtractService(this Vault v, object serviceInstance, object? key = null)
        => v.ExtractService(serviceInstance.GetType(), serviceInstance, key);

    /// <summary>
    /// Registers a service implementation type for a service interface or base type in the vault.
    /// </summary>
    /// <typeparam name="TService">The service interface or base type.</typeparam>
    /// <typeparam name="TInstance">The concrete implementation type.</typeparam>
    /// <param name="key">The optional key for the service.</param>
    [Pure]
    public static void ExtractService<TService, TInstance>(this Vault v, object? key = null)
        => v.ExtractService(typeof(TService), typeof(TInstance), key);

    /// <summary>
    /// Registers a decorator type for a service type in the vault.
    /// </summary>
    /// <typeparam name="TService">The service type to decorate.</typeparam>
    /// <typeparam name="TDecorator">The decorator type (must have a constructor accepting <typeparamref name="TService"/>).</typeparam>
    /// <param name="serviceKey">The optional key for the service.</param>
    /// <exception cref="InvalidOperationException">Thrown if the decorator type does not have a suitable constructor.</exception>
    [Pure]
    public static void ExtractDecorator<TService, TDecorator>(this Vault v, object? serviceKey = null) where TDecorator : TService
    {
        System.Reflection.ConstructorInfo ctor = typeof(TDecorator).GetConstructor([typeof(TService)]) ?? throw new InvalidOperationException($"Decorator Type {typeof(TDecorator)} must have constructor accepting {typeof(TService)}");
        v.ExtractDecorator<TService>(s => (TService)ctor.Invoke([s]), serviceKey);
    }

    /// <summary>
    /// Registers a decorator for a service type in the vault.
    /// </summary>
    /// <typeparam name="TService">The service type to decorate.</typeparam>
    /// <param name="decoratorFactory">The factory function that creates the decorator.</param>
    /// <param name="serviceKey">The optional key for the service.</param>
    [Pure]
    public static void ExtractDecorator<TService>(this Vault v, Func<TService, TService> decoratorFactory, object? serviceKey = null)
        => v.ExtractDecorator(typeof(TService), inner => decoratorFactory((TService)inner!)!, serviceKey);
}
