using FontStashSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

public class FontLoader : IAssetLoader
{
    public Task<object> Load(string path)
    {
        if (Path.GetExtension(path).ToLowerInvariant() is ".ttf")
        {
            var fontSystemSettings = new FontSystemSettings();

            var fontSystem = new FontSystem(fontSystemSettings);
            BinaryReader reader = new(File.Open(path, FileMode.Open));
            var data = reader.ReadBytes((int)reader.BaseStream.Length);
            fontSystem.AddFont(data);

            return (Task<object>)(object)fontSystem;
        }

        throw new Exception();
    }
}
