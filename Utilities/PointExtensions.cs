using Microsoft.Xna.Framework;
using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Hopeful.Utilities;

public static class PointExtensions
{
    public static bool IsPointOverRectangle(this Vector2 point, Rectangle rectangle)
        => rectangle.Contains(point);

    public static Vector2 ToVector2(this Point point)
        => new(point.X, point.Y);

    public static Point ToPoint(this Vector2 vector)
        => new((int)vector.X, (int)vector.Y);

    public static float Distance(this Point point, Point other)
    {
        float dx = point.X - other.X;
        float dy = point.Y - other.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    public static Point Clamp(this Point point, Point min, Point max)
        => new(Math.Clamp(point.X, min.X, max.X), Math.Clamp(point.Y, min.Y, max.Y));
}
