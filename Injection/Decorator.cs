namespace Hopeful.Injection;

/// <summary>
/// Base class for implementing decorators for a specific service type.
/// </summary>
/// <typeparam name="T">The type of the service being decorated.</typeparam>
/// <remarks>
/// Any class inheriting from <c>Decorator&lt;T&gt;</c> must also implement or inherit <typeparamref name="T"/> itself,
/// so that it can be used transparently as a decorated service.
/// </remarks>
public abstract class Decorator<T>(T inner)
{
    /// <summary>
    /// The decorated service instance.
    /// </summary>
    protected readonly T _inner = inner;
}
