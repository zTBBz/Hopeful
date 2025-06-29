using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Friflo.Engine.ECS;

namespace Hopeful.Utilities;

public static class EntityExtensions
{
    /// <summary>
    /// Ensures that the specified component of type <typeparamref name="T"/> exists on the entity.
    /// If the component is not exists, it is added to the entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to ensure. Must be a struct and implement <see cref="IComponent"/>.</typeparam>
    /// <param name="e">The entity to check.</param>
    /// <remarks>
    /// This method is useful for guaranteeing that an entity has a required component without redundant checks or additions.
    /// </remarks>
    public static void EnsureComponentExists<T>(this Entity e) where T : struct, IComponent
    {
        if (!e.HasComponent<T>()) e.AddComponent<T>();
    }

    /// <summary>
    /// Add or get a component of type <typeparamref name="T"/> on the entity.
    /// Return a reference to the component, if does not exist, it is added to the entity.
    /// </summary> 
    public static ref T GetOrAddComponent<T>(this Entity e) where T : struct, IComponent
    {
        if (!e.HasComponent<T>()) e.AddComponent<T>();
        return ref e.GetComponent<T>();
    }

    /// <summary>
    /// Add or get a component of type <typeparamref name="T"/> on the entity.
    /// Return a reference to the component, if does not exist, it is added to the entity.
    /// </summary> 
    public static ref T GetOrAddComponent<T>(this Entity e, T instance) where T : struct, IComponent
    {
        if (!e.HasComponent<T>()) e.AddComponent(instance);
        return ref e.GetComponent<T>();
    }

    /// <summary>
    /// Add or get a component of type <typeparamref name="T"/> on the entity.
    /// Return a reference to the component, if does not exist, it is added to the entity.
    /// </summary> 
    public static bool TryGetOrAddComponent<T>(this Entity e, CommandBuffer buffer, [NotNullWhen(true)] out T? component) where T : struct, IComponent
    {
        component = null;
        if (!e.HasComponent<T>()) buffer.AddComponent<T>(e.Id);
        if (e.TryGetComponent<T>(out var result)) component = result;
        return component != null;
    }

    /// <summary>
    /// Add or get a component of type <typeparamref name="T"/> on the entity.
    /// Return a reference to the component, if does not exist, it is added to the entity.
    /// </summary> 
    public static bool TryGetOrAddComponent<T>(this Entity e, T instance, CommandBuffer buffer, [NotNullWhen(true)] out T? component) where T : struct, IComponent
    {
        component = null;
        if (!e.HasComponent<T>()) buffer.AddComponent(e.Id, instance);
        if (e.TryGetComponent<T>(out var result)) component = result;
        return component != null;
    }
}
