using Hopeful.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Shader)]
public class ShaderLoader : IAssetLoader
{
    private readonly GlobalGraphics _graphics = GameCore.RootVault.InjectService<GlobalGraphics>();

    public Task<object> Load(string path)
    {
        BinaryReader reader = new(File.Open(path, FileMode.Open));
        return (Task<object>)(object)new Effect(_graphics.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
    }
}
