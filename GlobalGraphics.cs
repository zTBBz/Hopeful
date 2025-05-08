using Hopeful.Asset;
using Hopeful.Injection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hopeful;

[Service]
public class GlobalGraphics : IDisposable
{
    public GraphicsDeviceManager GraphicsDeviceManager { get; private set; } = null!;
    public GraphicsDevice GraphicsDevice { get; private set; } = null!;
    public Viewport Viewport { get; private set; }
    public SpriteBatch SpriteBatch { get; private set; } = null!;
    public Texture2D Pixel { get; private set; } = null!;
    public Texture2DAtlas? Texture2DAtlas { get; internal set; }

    internal GlobalGraphics(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, Viewport viewport)
    {
        GraphicsDeviceManager = graphicsDeviceManager;
        GraphicsDevice = graphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Viewport = viewport;
        Pixel = new(GraphicsDevice, 1, 1);
        Pixel.SetData([Color.White]);
    }

    internal void WindowSizeChanged(Viewport newViewport)
    {
        Viewport = newViewport;
        OnWindowSizeChanged?.Invoke();
    }

    public void Dispose()
    {
        Pixel.Dispose();
        // Texutre2DAtlas disposing by AssetManager
    }

    public event Action? OnWindowSizeChanged;
}
