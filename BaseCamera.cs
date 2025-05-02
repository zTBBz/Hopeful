using Hopeful.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel.Composition;

namespace Hopeful;

public interface ICamera
{
    Vector2 Position { get; }
    float Zoom { get; }
    float MaxZoom { get; } 
    float MinZoom { get; }
    Matrix TransformMatrix { get; }
    void UpdateMatrix();
    void Move(Vector2 amount);
    Vector2 GetMousePosition();
    Vector2 GetPreviousMousePosition();
}

public class BaseCamera : ICamera
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Zoom { get; set; } = 1.0f;
    public float MaxZoom { get; set; } = 8.0f;
    public float MinZoom { get; set; } = 0.1f;
    public Matrix TransformMatrix { get; private set; }

    private Viewport _viewport;

    [Import]
    private Lazy<BaseInput> _input = null!;

    protected BaseCamera(Viewport viewport)
    {
        _viewport = viewport;
        UpdateMatrix();
    }

    public void Move(Vector2 amount)
        => Position += amount;

    public void UpdateMatrix()
    {
        Zoom = MathHelper.Clamp(Zoom, MinZoom, MaxZoom);

        TransformMatrix = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                         Matrix.CreateScale(Zoom) *
                         Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0));
    }

    public Vector2 GetMousePosition()
        => Vector2.Transform(new Vector2(_input.Value.MousePosition.X, _input.Value.MousePosition.Y), Matrix.Invert(TransformMatrix));

    public Vector2 GetPreviousMousePosition()
    => Vector2.Transform(new Vector2(_input.Value.PreviousMousePosition.X, _input.Value.PreviousMousePosition.Y), Matrix.Invert(TransformMatrix));
}
