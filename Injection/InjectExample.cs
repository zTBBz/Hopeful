using Hopeful.Asset;
using Hopeful.Asset.Loaders;

namespace Hopeful.Injection;

public class InjectExample
{
    [Inject("Old")]
    public GlobalGraphics GraphicsOld = null!; // is GlobalGraphics

    [Inject(AssetFormat.Sprite)]
    public IAssetLoader Loader = null!; // is TextureLoader

    [Inject]
    public GlobalGraphics Graphics = null!; // is just GlobalGraphics

    public InjectExample() => GameCore.RootVault.Inject(this);
}