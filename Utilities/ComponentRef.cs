using Friflo.Engine.ECS;

namespace Hopeful.Utilities;

/// <summary>
/// A reference wrapper for an ECS component that allows safe modification of the component
/// via a <see cref="CommandBuffer"/>. Changes are deferred until the buffer is executed.
/// </summary>
/// <typeparam name="T">The component type. Must be a struct implementing <see cref="IComponent"/>.</typeparam>
/// <param name="entity">The target entity whose component will be referenced.</param>
/// <param name="commandBuffer">The command buffer used to defer component modifications.</param>
public class ComponentRef<T>(Entity entity, CommandBuffer commandBuffer) where T : struct, IComponent
{
    private readonly Entity _entity = entity;
    private readonly CommandBuffer _commandBuffer = commandBuffer;

    /// <summary>
    /// Gets or sets the component value for the referenced entity.
    /// </summary>
    /// <remarks>
    /// - <b>Getting</b> retrieves the current component value directly from the entity.<br/>
    /// - <b>Setting</b> schedules an update via the <see cref="CommandBuffer"/>, which will be applied later.
    /// </remarks>
    public T Value
    {
        get => _entity.GetComponent<T>();
        set => _commandBuffer.AddComponent(_entity.Id, value);
    }
}