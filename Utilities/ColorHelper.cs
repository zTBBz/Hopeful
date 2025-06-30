using System;
using SixLabors.ImageSharp;

namespace Hopeful.Utilities;

public static class ColorHelper
{
    public static Color FromHsl(float h, float s, float l)
    {
        if (h <= 0 || h > 1 || s <= 0 || s > 1 || l <= 0 || l > 1)
            throw new ArgumentOutOfRangeException("HSL convertion to Color: incorrect parameter");

        float c = (1 - Math.Abs(2 * l - 1)) * s;
        float x = c * (1 - Math.Abs(h * 6 % 2 - 1));
        float m = l - c / 2;

        float r, g, b;
        if (h < 1f / 6f) { r = c; g = x; b = 0; }
        else if (h < 2f / 6f) { r = x; g = c; b = 0; }
        else if (h < 3f / 6f) { r = 0; g = c; b = x; }
        else if (h < 4f / 6f) { r = 0; g = x; b = c; }
        else if (h < 5f / 6f) { r = x; g = 0; b = c; }
        else { r = c; g = 0; b = x; }

        return Color.FromRgb(
            (byte)((r + m) * 255),
            (byte)((g + m) * 255),
            (byte)((b + m) * 255));
    }

    public static float HueToRgb(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1f / 6f) return p + (q - p) * 6 * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6;
        return p;
    }

    public static Color FromHsv(float h, float s, float v)
    {
        if (h <= 0 || h > 1 || s <= 0 || s > 1 || v <= 0 || v > 1)
            throw new ArgumentOutOfRangeException("HSV convertion to Color: incorrect parameter");

        float r = 0, g = 0, b = 0;

        float x = 1.0f - Math.Abs(h * 6.0f % 2.0f - 1.0f);
        int hi = (int)(h * 6.0f);

        switch (hi)
        {
            case 0: r = 1.0f; g = x; b = 0; break;
            case 1: r = x; g = 1.0f; b = 0; break;
            case 2: r = 0; g = 1.0f; b = x; break;
            case 3: r = 0; g = x; b = 1.0f; break;
            case 4: r = x; g = 0; b = 1.0f; break;
            case 5: r = 1.0f; g = 0; b = x; break;
        }

        r = (r * s + (1 - s)) * v;
        g = (g * s + (1 - s)) * v;
        b = (b * s + (1 - s)) * v;

        return Color.FromRgb(
            (byte)(r * 255),
            (byte)(g * 255),
            (byte)(b * 255));
    }

    public static Color Lerp(this Color a, Color b, float t)
    {
        if (t <= 0 || t > 1)
            throw new ArgumentOutOfRangeException("Lerp time parameter more or less than 0 and 1");

        var aPixel = a.ToPixel<SixLabors.ImageSharp.PixelFormats.Rgba32>();
        var bPixel = b.ToPixel<SixLabors.ImageSharp.PixelFormats.Rgba32>();

        return Color.FromRgba(
            (byte)(aPixel.R + (bPixel.R - aPixel.R) * t),
            (byte)(aPixel.G + (bPixel.G - aPixel.G) * t),
            (byte)(aPixel.B + (bPixel.B - aPixel.B) * t),
            (byte)(aPixel.A + (bPixel.A - aPixel.A) * t));
    }
}
