using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace Hopeful.Injection.System;

/// <summary>
///   <para>Represents the Dependency Injection class for BaseSystem.</para>
/// </summary>
public abstract class InjectBaseSystem : BaseSystem
{
    protected override void OnAddStore(EntityStore store)
        => GameCore.RootVault.Inject(this);
}
