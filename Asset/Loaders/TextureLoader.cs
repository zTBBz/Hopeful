using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

public class TextureLoader : IAssetLoader
{
    [Import]
    private static GlobalGraphics _graphics = null!;

    public Task<object> Load(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        if (extension is ".dds")
        {
            return Task.Run(async () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var raw = await Task.Run(() =>
                    TextureConverter.TextureFromDDS(_graphics.GraphicsDevice, path)
                );
                stopwatch.Stop();
                Debug.WriteLine($"Loaded DDS {path}: {stopwatch.ElapsedMilliseconds} ms");
                return (object)raw;
            });
        }

        if (extension is ".png" or ".jpg" or ".jpeg")
        {
            return Task.Run(() =>
                (object)Texture2D.FromFile(_graphics.GraphicsDevice, path)
            );
        }

        throw new Exception();
    }
}
