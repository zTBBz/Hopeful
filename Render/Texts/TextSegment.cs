using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Color = SixLabors.ImageSharp.Color;

namespace Hopeful.Render.Texts;

public class TextSegment : ICloneable
{
    public string Text = string.Empty;
    public string TextToShow = string.Empty;
    public string FontName = "Default";
    public Vector2 Position = Vector2.Zero;
    public float Rotation = 0f;

    #region ICloneable Memberss

    public Color Color = Color.White;
    public List<TextFormat>? Formats;

    public object Clone() => MemberwiseClone();

    #endregion
}
