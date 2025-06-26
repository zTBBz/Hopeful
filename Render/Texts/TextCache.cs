using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using Friflo.Engine.ECS;

namespace Hopeful.Render.Texts;

public struct TextCache : IComponent
{
    public List<TextSegment> Segments;
    public Image<Rgba32>? TextBuffer;
    public Texture2D? CacheTextTexture;

    public Rgba32[]? PixelsBuffer;
    public byte[]? ColorDataBuffer;
}
