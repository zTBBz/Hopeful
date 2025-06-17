using Friflo.Engine.ECS;

namespace Hopeful.Scripts;

public class InjectScript(Vault vault) : Script
{
    public readonly Vault Vault = vault;
    public bool IsStarted { get; internal set; } = false;

    public override void Start()
    {
        Store.OnScriptAdded += OnAdded;
        Store.OnScriptRemoved += OnRemoved;
        Vault.Inject(this);
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    protected virtual void OnAdded(ScriptChanged changed) { }
    protected virtual void OnRemoved(ScriptChanged changed) { }
}
