using Friflo.Engine.ECS.Systems;

namespace Hopeful.Render;

public sealed class WindowChangeSystem(Vault vault) : InjectBaseSystem(vault)
{
    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    protected override void OnUpdateGroup()
        => _graphics.IsWindowSizeChanged = false;
}
