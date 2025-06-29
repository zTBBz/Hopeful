using SixLabors.Fonts;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Font, typeof(IAssetLoader))]
public sealed class FontLoader : IAssetLoader
{
    public bool TryLoad(string path, [NotNullWhen(true)] out object? result)
    {
        result = null;
        if (Path.GetExtension(path).ToLowerInvariant() is ".ttf")
        {
            var fontFamily = new FontCollection().Add(path);
            result = fontFamily.CreateFont(16);
            return true;
        }

        return false;
    }
}
