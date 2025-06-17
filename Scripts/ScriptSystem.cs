using Friflo.Engine.ECS.Systems;

namespace Hopeful.Scripts;

public sealed class ScriptSystem(Vault vault) : InjectBaseSystem(vault)
{
    protected override void OnUpdateGroup()
    {
        var scriptsSpan = Store.EntityScripts;

        foreach (var scripts in scriptsSpan)
        {
            foreach (var script in scripts)
            {
                if (script is null) continue;

                if (script is InjectScript injectScript)
                {
                    if (!injectScript.IsStarted)
                    {
                        injectScript.Start();
                        injectScript.IsStarted = true;
                    }
                }

                script.Update();
            }
        }
    }
}
