using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using Hopeful.Utilities;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using Color = SixLabors.ImageSharp.Color;

namespace Hopeful.Render.Texts;

public class TextSystem(Vault vault) : InjectQuerySystem<Text>(vault)
{
    private static readonly GraphicsOptions _graphicsOptions = new() { Antialias = true };
    private static readonly DrawingOptions _drawingOptions = new() { GraphicsOptions = _graphicsOptions };

    [Inject]
    private readonly GlobalGraphics _globalGraphics = null!;

    [Inject]
    private readonly AssetManager _assets = null!;

    [Inject]
    private readonly ITextFormatParser _parser = null!;

    protected override void OnUpdate() // TODO: Later should use Each for performance boost
    {
        foreach (var entity in Query.Entities)
        {
            var text = entity.GetComponent<Text>();
            var cache = entity.GetOrAddComponent<TextCache>();

            ClearTextBuffer(cache);

            if (!string.IsNullOrEmpty(text.TextValue))
            {
                if (ValidateTextChange(text.TextValue, cache))
                    SetupSegmentsPositions(cache, text.Position);

                ApplyFormats(cache, Tick.deltaTime);

                DrawSegmentsToBuffer(text, cache);
            }

            if (cache.TextBuffer == null) continue;

            var textTexture = GetTextTexture(cache);
            CopyBufferData(textTexture, cache);

            // soft ensure, because basically all Entity created by builders, cant be components dependence lost
            if (!entity.TryGetComponent<SpriteCache>(out var spriteCache)) continue;

            spriteCache.AtlasRect = null;
            spriteCache.SingleTexture = textTexture;
        }
    }

    private bool ValidateTextChange(string text, TextCache cache)
    {
        var segments = _parser.ParseText(text);

        if (cache.Segments.Equals(segments)) return false;

        cache.Segments.Clear();
        cache.Segments.AddRange(segments);
        return true;
    }

    private void SetupSegmentsPositions(TextCache cache, Vector2 position) // TODO: Add Y support by /n
    {
        var currentX = position.X;
        var currentY = position.Y;

        foreach (var segment in cache.Segments)
        {
            segment.Position = new(currentX, currentY);
            currentX += MeasureString(segment.Text, segment.FontName).X;
        }
    }

    [Pure]
    private Vector2 MeasureString(string text, string fontName)
    {
        // can be cached for less allocation, because later used segment Font in DrawSegmentsToBuffer
        var size = TextMeasurer.MeasureAdvance(text, new SixLabors.Fonts.TextOptions(_assets.GetAsset<Font>(fontName))); 
        return new((int)size.Width, (int)size.Height);
    }

    private static void ApplyFormats(TextCache cache, float deltaTime)
    {
        foreach (var segment in cache.Segments)
        {
            var formats = segment.Formats;

            if (formats == null) continue;

            foreach (var format in formats)
            {
                if (!format.IsSetuped)
                {
                    format.SetupAction?.Invoke(format, segment);
                    format.IsSetuped = true;
                }
                format.UpdateAction?.Invoke(format, segment, deltaTime);
            }
        }
    }

    private void DrawSegmentsToBuffer(Text textComp, TextCache cache)
    {
        // maybe should clear previous buffer

        var sharedFont = _assets.GetAsset<Font>(textComp.FontName);

        CreateTextBuffer(textComp.TextValue, cache, sharedFont);

        foreach (var segment in cache.Segments)
        {
            var text = segment.TextToShow;
            var color = segment.Color;
            var position = segment.Position;
            var rotation = segment.Rotation;
            var font = (segment.FontName != textComp.FontName) ? _assets.GetAsset<Font>(segment.FontName) : sharedFont; 

            var drawingOptions = rotation != 0
                ? new DrawingOptions
                {
                    GraphicsOptions = _graphicsOptions,
                    Transform = TransformHelper.Rotate(rotation)
                }
                : _drawingOptions;

            cache.TextBuffer!.Mutate(ctx => ctx.DrawText(drawingOptions, text, font, color, position.ToSixLaborsPoint()));
        }
    }

    [Pure]
    private static Vector2 CalculateTextureSize(string text, Font font)
    {
        var size = TextMeasurer.MeasureAdvance(text, new SixLabors.Fonts.TextOptions(font));
        return new(size.X, size.Y);
    }

    private static void CreateTextBuffer(string text, TextCache cache, Font font)
    {
        var size = CalculateTextureSize(text, font);
        var width = (int)Math.Ceiling(size.X) + 4;
        var height = (int)Math.Ceiling(size.Y) + 4;

        // Ensure buffer not changed
        if (cache.TextBuffer?.Width == width && cache.TextBuffer?.Height == height) return;

        cache.TextBuffer?.Dispose();
        cache.TextBuffer = new(width, height);
    }

    private Texture2D GetTextTexture(TextCache cache)
    {
        var texture = cache.CacheTextTexture;
        var buffer = cache.TextBuffer!;

        // Ensure texture equal to buffer
        if (texture?.Width == buffer.Width && texture?.Height == buffer.Height) return texture;

        var newTexture = new Texture2D(_globalGraphics.GraphicsDevice, buffer.Width, buffer.Height);
        cache.CacheTextTexture = newTexture; // WTF? Not needs actually
        return newTexture;

    }

    private static void CopyBufferData(Texture2D target, TextCache cache)
    {
        var buffer = cache.TextBuffer!;

        var pixelCount = buffer.Width * buffer.Height;

        if (cache.PixelsBuffer == null || cache.PixelsBuffer.Length < pixelCount)
            cache.PixelsBuffer = new Rgba32[pixelCount];

        if (cache.ColorDataBuffer == null || cache.ColorDataBuffer.Length < pixelCount * 4)
            cache.ColorDataBuffer = new byte[pixelCount * 4];

        buffer.CopyPixelDataTo(cache.PixelsBuffer);

        for (var i = 0; i < pixelCount; i++)
        {
            var baseIndex = i * 4;
            cache.ColorDataBuffer[baseIndex] = cache.PixelsBuffer[i].R;
            cache.ColorDataBuffer[baseIndex + 1] = cache.PixelsBuffer[i].G;
            cache.ColorDataBuffer[baseIndex + 2] = cache.PixelsBuffer[i].B;
            cache.ColorDataBuffer[baseIndex + 3] = cache.PixelsBuffer[i].A;
        }

        target.SetData(cache.ColorDataBuffer);
    }

    private static void ClearTextBuffer(TextCache cache) => cache.TextBuffer?.Mutate(ctx => ctx.Clear(Color.Transparent));
}
