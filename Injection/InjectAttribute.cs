using System;
using JetBrains.Annotations;

namespace Hopeful.Injection;

/// <summary>
/// Marks a field or property for dependency injection.
/// </summary>
/// <remarks>
/// The <see cref="Vault"/> will automatically inject the appropriate service into members marked with this attribute.
/// </remarks>
[MeansImplicitUse, AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class InjectAttribute : Attribute
{
    /// <summary>
    /// Gets the optional key for the injected service.
    /// </summary>
    public object? Key { get; }

    /// <summary>
    /// Gets the optional decorator target type for advanced decorator scenarios.
    /// </summary>
    public Type? DecoratorTargetType { get; }

    public InjectAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InjectAttribute"/> class with a key.
    /// </summary>
    /// <param name="key">The key to resolve a specific service instance.</param>
    public InjectAttribute(object key) => Key = key;

    /// <summary>
    /// Initializes a new instance of the <see cref="InjectAttribute"/> class with a decorator target type.
    /// </summary>
    /// <param name="decoratorTargetType">The type at which to stop applying decorators.</param>
    public InjectAttribute(Type decoratorTargetType) => DecoratorTargetType = decoratorTargetType;

    /// <summary>
    /// Initializes a new instance of the <see cref="InjectAttribute"/> class with a key and a decorator target type.
    /// </summary>
    /// <param name="key">The key to resolve a specific service instance.</param>
    /// <param name="decoratorTargetType">The type at which to stop applying decorators.</param>
    public InjectAttribute(object key, Type decoratorTargetType)
    {
        Key = key;
        DecoratorTargetType = decoratorTargetType;
    }
}
