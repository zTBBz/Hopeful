using DdsKtxXna;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Sprite, typeof(IAssetLoader))]
public sealed class TextureLoader : IAssetLoader
{
    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    public bool TryLoad(string path, [NotNullWhen(true)] out object? result)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        result = null;

        using var stream = File.OpenRead(path);
        {
            switch (extension)
            {
                case ".png" or ".jpeg" or ".jpg":
                    result = Texture2D.FromStream(_graphics.GraphicsDevice, stream);
                    return true;
                case ".dds":
                    result = (Texture2D)DdsKtxLoader.FromStream(_graphics.GraphicsDevice, stream);
                    return true;
                default:
                    return false;
            }
        }
    }
}
