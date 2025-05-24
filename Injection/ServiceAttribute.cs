using System;

namespace Hopeful.Injection;

/// <summary>
/// Marks a class as a service for dependency injection.
/// </summary>
/// <remarks>
/// Classes marked with this attribute can be automatically registered and resolved by the <see cref="Vault"/> DI container.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ServiceAttribute : Attribute
{
    /// <summary>
    /// Gets the optional key for the service.
    /// </summary>
    public object? Key { get; }

    public ServiceAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with a key.
    /// </summary>
    /// <param name="key">The key to distinguish this service from others of the same type.</param>
    public ServiceAttribute(object key) => Key = key;
}
