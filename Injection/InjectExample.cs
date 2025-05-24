using Hopeful.Asset;
using Hopeful.Asset.Loaders;

namespace Hopeful.Injection;

public class InjectExample
{
    [Inject("Old")]
    public GlobalGraphics GraphicsOld = null!; // is GlobalGraphics with key Old

    [Inject(AssetFormat.Sprite)]
    public IAssetLoader Loader = null!; // is TextureLoader (Sprite)

    [Inject]
    public GlobalGraphics Graphics = null!; // is just GlobalGraphics

    public InjectExample()
    {
        GameCore.RootVault.Inject(this);
        // OR
        var vault = new Vault();
        vault.Inject(this);
        // OR
        var instanceWithInject = vault.CreateWithInjection<InjectExample>();
    }
}
