using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

public class ShaderLoader : IAssetLoader
{
    [Import]
    private static GlobalGraphics _graphics = null!;

    public Task<object> Load(string path)
    {
        BinaryReader reader = new(File.Open(path, FileMode.Open));
        return (Task<object>)(object)new Effect(_graphics.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
    }
}
