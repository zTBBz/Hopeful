using FontStashSharp;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Hopeful.Asset;

public static class AssetDetector
{
    public static AssetFormat DetectFormat(string assetPath)
    {
        var extension = Path.GetExtension(assetPath);
        return extension switch
        {
            ".png" or ".dds" or ".jpeg" or ".jpg" => AssetFormat.Sprite,
            ".mp3" or ".ogg" => AssetFormat.Audio,
            ".loc" => AssetFormat.Localization,
            ".ttf" => AssetFormat.Font,
            _ => throw new AssetLoadException(assetPath, "invalid file format."),
        };
    }

    public static string GetAssetSuffix<T>() where T : class
    {
        var type = nameof(T);
        return type switch
        {
            nameof(Texture2D) => AssetFormat.Sprite.ToString(),
            nameof(SoundEffect) => AssetFormat.Audio.ToString(),
            nameof(SpriteEffect) => AssetFormat.Shader.ToString(),
            nameof(FontSystem) => AssetFormat.Font.ToString(),
            _ => string.Empty,
        };
    }
}
