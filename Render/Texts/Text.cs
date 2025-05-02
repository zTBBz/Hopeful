using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;
using System;

namespace Hopeful.Render.Texts;

public struct Text : IComponent
{
    public string TextValue;
    public string FontName;
    public int FontSize;
    public Vector2 Position;
    public float Rotation;
    public Color Color;

    public Text(string text, string fontName, Vector2? position = null, Color? color = null, int fontSize = 14, float rotation = 0f)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        ArgumentException.ThrowIfNullOrEmpty(fontName);

        TextValue = text;
        FontName = fontName;
        FontSize = fontSize;
        Position = (Vector2)(position == null ? Vector2.Zero : position);
        Color = (Color)(color == null ? Color.White : color);
        Rotation = rotation;
    }
}
