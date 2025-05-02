using DryIoc;
using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using Hopeful.Utilities;
using System;

namespace Hopeful.Mod;

public abstract class BaseMod : IGameServices
{
    // https://github.com/Dzhake/DGRFix/blob/main/DGRFix/DependencyResolver.cs

    // https://github.com/Hyyeve/MClientTemplate/blob/master/src/MClient/Core/DLLSystem/MDependencyResolver.cs

    public string ModId = null!;

    public Container Container { get; }
    public Lazy<SystemRoot>? SystemRoot { get; } = null!;
    public Lazy<AssetManager>? Assets { get; } = null!;

    public BaseMod(Container container)
    {
        Container = container;
        Assets = container.Resolve<Lazy<AssetManager>?>(ModId);
        SystemRoot = container.Resolve<Lazy<SystemRoot>?>(ModId);
    }

    public virtual void Initialize() { }

    public virtual void Update(ITime time)
    {
        SystemRoot?.Value.Update(time.ToUpdateTick());
    }

    public virtual void Unload()
    {
        if (SystemRoot is not null)
            SystemRoot.Value.Enabled = false;
        Assets?.Value.Dispose();
        Container.Dispose();
    }
}
