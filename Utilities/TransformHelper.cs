using Microsoft.Xna.Framework;
using System.Numerics;

namespace Hopeful.Utilities;

public static class TransformHelper
{
    public static Matrix3x2 Rotate(float rotation)
    {
        return Matrix3x2.CreateRotation(MathHelper.ToRadians(rotation));
    }

    public static Matrix3x2 CreateTransform(System.Numerics.Vector2 position, float rotation, System.Numerics.Vector2 scale)
    {
        return Matrix3x2.CreateScale(scale) *
               Matrix3x2.CreateRotation(MathHelper.ToRadians(rotation)) *
               Matrix3x2.CreateTranslation(position);
    }

    public static Matrix3x2 CreateTransform(System.Numerics.Vector2 position, float rotation)
    {
        return Matrix3x2.CreateRotation(MathHelper.ToRadians(rotation)) *
               Matrix3x2.CreateTranslation(position);
    }
}