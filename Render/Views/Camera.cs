using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hopeful.Render.Views;

public struct Camera(Viewport viewport, int mask = 0b_0001) : IComponent
{
    public Vector2 Position = new(0, 0);
    public float Zoom = 0f;
    public float MaxZoom = 0f;
    public float MinZoom = 0f;
    public Matrix TransformMatrix;
    public Viewport Viewport = viewport;
    public bool IsActive = true;

    public readonly int Mask = mask;
}
