using Microsoft.Xna.Framework.Graphics;

namespace Hopeful.Utilities;

public static class TextureExtensions
{
    public static Texture2D WithData<T>(this Texture2D texture, T[] data) where T : struct
    {
        texture.SetData(data);
        return texture;
    }
}