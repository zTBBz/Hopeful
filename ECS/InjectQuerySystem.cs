#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Friflo.Engine.ECS.Systems;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem(Vault vault) : QuerySystem
{
    protected override void OnAddStore(EntityStore store)
        => vault.Inject(this);
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1>(Vault vault) : QuerySystem<T1>
    where T1 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => vault.Inject(this);
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2>(Vault vault) : QuerySystem<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => vault.Inject(this);
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2, T3>(Vault vault) : QuerySystem<T1, T2, T3>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => vault.Inject(this);
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2, T3, T4>(Vault vault) : QuerySystem<T1, T2, T3, T4>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => vault.Inject(this);
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2, T3, T4, T5>(Vault vault) : QuerySystem<T1, T2, T3, T4, T5>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => vault.Inject(this);
}
