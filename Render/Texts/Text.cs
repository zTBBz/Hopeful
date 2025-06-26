using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;

namespace Hopeful.Render.Texts;

public struct Text(string text) : IComponent
{
    public string TextValue = text;
    public string FontName = "Default";
    public int FontSize = 14;
    public Vector2 Position = Vector2.Zero;
    public float Rotation = 0f;
    public Color Color = Color.White;
}
