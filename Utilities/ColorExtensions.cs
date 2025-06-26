using SixLabors.ImageSharp;

namespace Hopeful.Render.Texts.Formats;

public static class ColorExtensions
{
    public static Microsoft.Xna.Framework.Color ToXnaColor(this Color color)
    {
        var rgba = color.ToPixel<SixLabors.ImageSharp.PixelFormats.Rgba32>();
        return new Microsoft.Xna.Framework.Color(rgba.R, rgba.G, rgba.B, rgba.A);
    }

    public static Color ToSixLaborsColor(this Microsoft.Xna.Framework.Color color)
        => Color.FromRgb(color.R, color.G, color.B);
}