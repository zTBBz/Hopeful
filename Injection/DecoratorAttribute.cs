using System;

namespace Hopeful.Injection;

/// <summary>
/// Marks a class as a decorator for a specific service type.
/// </summary>
/// <remarks>
/// Classes marked with this attribute will be registered as decorators in the <see cref="Vault"/> DI container.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DecoratorAttribute : Attribute
{
    /// <summary>
    /// Gets the optional key for the decorated service.
    /// </summary>
    public object? Key { get; }

    /// <summary>
    /// Gets the type of the service to be decorated.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecoratorAttribute"/> class for the specified service type.
    /// </summary>
    /// <param name="type">The type of the service to be decorated.</param>
    public DecoratorAttribute(Type type) => Type = type;

    /// <summary>
    /// Initializes a new instance of the <see cref="DecoratorAttribute"/> class for the specified service type and key.
    /// </summary>
    /// <param name="key">The key to distinguish the decorated service.</param>
    /// <param name="type">The type of the service to be decorated.</param>
    public DecoratorAttribute(object key, Type type)
    {
        Key = key;
        Type = type;
    }
}
