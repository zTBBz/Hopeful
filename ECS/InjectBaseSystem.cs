#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Friflo.Engine.ECS.Systems;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///   <para>Represents the Dependency Injection class for BaseSystem.</para>
/// </summary>
public abstract class InjectBaseSystem(Vault vault) : BaseSystem
{
    protected EntityStore Store = null!;

    protected override void OnAddStore(EntityStore store)
    {
        Store = store;
        vault.Inject(this);
    }
}