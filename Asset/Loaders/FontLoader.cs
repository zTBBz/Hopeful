using FontStashSharp;
using Hopeful.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Font)]
public class FontLoader : IAssetLoader
{
    private readonly FontSystemSettings settings = new();

    public Task<object> Load(string path)
    {
        if (Path.GetExtension(path).ToLowerInvariant() is ".ttf")
        {
            // Add ExistingTexture and ExistingTextureUsedSpace for more perfomance (https://discord.com/channels/766725034445635634/781597343387746355/1373375795476562011)
            var fontSystem = new FontSystem(settings);
            BinaryReader reader = new(File.Open(path, FileMode.Open));
            var data = reader.BaseStream.ToByteArray();
            fontSystem.AddFont(data);

            return (Task<object>)(object)fontSystem;
        }

        throw new Exception();
    }
}
