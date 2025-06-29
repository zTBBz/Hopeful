using System;
using System.Diagnostics.CodeAnalysis;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Shader, typeof(IAssetLoader))]
public sealed class ShaderLoader : IAssetLoader
{
    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    public bool TryLoad(string path, [NotNullWhen(true)] out object? result)
    {
        throw new NotImplementedException();

        /*BinaryReader reader = new(File.Open(path, FileMode.Open));
        return (Task<object>)(object)new Effect(_graphics.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));*/
    }
}
