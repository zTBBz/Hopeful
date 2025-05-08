using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace Hopeful.Injection.System;

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem : QuerySystem
{
    protected override void OnAddStore(EntityStore store)
        => GameCore.RootVault.Inject(this);

    protected override void OnUpdate() { }
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1> : QuerySystem<T1>
    where T1 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => GameCore.RootVault.Inject(this);

    protected override void OnUpdate() { }
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2> : QuerySystem<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => GameCore.RootVault.Inject(this);

    protected override void OnUpdate() { }
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2, T3> : QuerySystem<T1, T2, T3>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => GameCore.RootVault.Inject(this);

    protected override void OnUpdate() { }
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2, T3, T4> : QuerySystem<T1, T2, T3, T4>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => GameCore.RootVault.Inject(this);

    protected override void OnUpdate() { }
}

/// <summary>
///   <para>Represents the Dependency Injection class for QuerySystem.</para>
/// </summary>
public abstract class InjectQuerySystem<T1, T2, T3, T4, T5> : QuerySystem<T1, T2, T3, T4, T5>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
    where T4 : struct, IComponent
    where T5 : struct, IComponent
{
    protected override void OnAddStore(EntityStore store)
        => GameCore.RootVault.Inject(this);

    protected override void OnUpdate() { }
}
