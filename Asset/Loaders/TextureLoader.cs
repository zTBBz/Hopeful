using DdsKtxXna;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Sprite)]
public class TextureLoader : IAssetLoader
{
    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    public Task<object> Load(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".png" or ".jpeg" or ".jpg" => Task.Run(() => (object)Texture2D.FromFile(_graphics.GraphicsDevice, path)),
            ".dds" => Task.Run(() =>
            {
                using var stream = File.OpenRead(path);
                return (object)(Texture2D)DdsKtxLoader.FromStream(_graphics.GraphicsDevice, stream);
            }),
            _ => throw new Exception(),
        };
    }
}
