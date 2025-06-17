using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;

namespace Hopeful.UI;

public struct Visual() : IComponent
{
    public bool IsVisible = true;
    public bool IsDirty = false;

    public Vector2 Position = Vector2.Zero;
    public Vector2 Size = Vector2.One;
    public readonly Rectangle Bounds => new((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    public Unit LeftUnit
    {
        get;
        set
        {
            field = value;
            IsDirty = true;
        }
    } = Unit.Pixels(0);

    public Unit TopUnit
    {
        get;
        set
        {
            field = value;
            IsDirty = true;
        }
    } = Unit.Pixels(0);

    public Unit WidthUnit
    {
        get;
        set
        {
            field = value;
            IsDirty = true;
        }
    } = Unit.Pixels(100);

    public Unit HeightUnit
    {
        get;
        set
        {
            field = value;
            IsDirty = true;
        }
    } = Unit.Pixels(100);
}
