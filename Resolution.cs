using System;
using System.Collections.Generic;
using System.Linq;

namespace Hopeful;

public readonly struct Resolution
{
    public int Width { get; }
    public int Height { get; }
    public float AspectRatio => (float)Width / Height;

    private Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public static readonly Resolution HD = new(1280, 720);        // 720p
    public static readonly Resolution FullHD = new(1920, 1080);   // 1080p
    public static readonly Resolution QHD = new(2560, 1440);      // 1440p
    public static readonly Resolution UHD = new(3840, 2160);      // 4K

    // 4:3
    public static readonly Resolution R_1024x768 = new(1024, 768);
    public static readonly Resolution R_1280x960 = new(1280, 960);
    public static readonly Resolution R_1600x1200 = new(1600, 1200);

    // 16:9
    public static readonly Resolution R_1366x768 = new(1366, 768);
    public static readonly Resolution R_1600x900 = new(1600, 900);

    public static IReadOnlyList<Resolution> GetAvailableResolutions()
    {
        return new[]
        {
        HD,
        FullHD,
        QHD,
        UHD,
        R_1024x768,
        R_1280x960,
        R_1600x1200,
        R_1366x768,
        R_1600x900
    }.OrderBy(r => r.Width * r.Height).ToList();
    }

    public static Resolution GetNearestResolution(int width, int height)
    {
        var targetPixels = width * height;
        return GetAvailableResolutions()
            .OrderBy(r => Math.Abs(r.Width * r.Height - targetPixels))
            .First();
    }

    public override string ToString() => $"{Width}x{Height}";
}
