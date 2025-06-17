using Friflo.Engine.ECS;

namespace Hopeful.Utilities;

public static class EntityExtensions
{
    /// <summary>
    /// Ensures that the specified component of type <typeparamref name="T"/> exists on the entity.
    /// If the component is not present, it is added to the entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to ensure. Must be a struct and implement <see cref="IComponent"/>.</typeparam>
    /// <param name="e">The entity to check and modify.</param>
    /// <remarks>
    /// This method is useful for guaranteeing that an entity has a required component without redundant checks or additions.
    /// </remarks>
    public static void EnsureComponentExists<T>(this Entity e) where T : struct, IComponent
    {
        if (!e.HasComponent<T>()) e.AddComponent<T>();
    }

    public static ref T GetOrAddComponent<T>(this Entity e) where T : struct, IComponent
    {
        if (!e.HasComponent<T>())
            e.AddComponent<T>();
        return ref e.GetComponent<T>();
    }
}
