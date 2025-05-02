using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;
using System;

namespace Hopeful.Render;

public struct Sprite : IComponent
{
    public string SpriteName;
    public Vector2 Offset;
    public float Scale;
    public float Layer;
    public bool IsVisible;

    public Sprite(string spriteName, Vector2 offset, float scale = 1f, float layer = 1f, bool isVisible = true)
    {
        ArgumentNullException.ThrowIfNull(spriteName);
        ArgumentNullException.ThrowIfNull(offset);
        SpriteName = spriteName;
        Offset = offset;
        Scale = scale;
        Layer = layer;
        IsVisible = isVisible;
    }
}
