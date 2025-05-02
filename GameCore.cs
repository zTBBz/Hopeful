using DryIoc;
using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hopeful;

public class GameCore : Game, IGameServices
{
    public Container Container { get; }

    [Import]
    public Lazy<SystemRoot>? SystemRoot { get; } = null!;

    [Import]
    public Lazy<AssetManager>? Assets { get; } = null!;

    public GameTime CurrentGameTime { get; private set; } = null!;

    public GameCore(Container container) => Container = container;

    protected override void LoadContent()
    {
        Task.Run(LoadAssets);
    }

    private async Task LoadAssets()
    {
        try
        {
            Stopwatch sw = Stopwatch.StartNew();
            await Assets!.Value.LoadAllAssetsAsync("Assets");
            sw.Stop();

            Debug.WriteLine($"All assets loaded. Time: {sw.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading assets: {ex.Message}");
        }
        finally
        {
            Container.Resolve<GlobalGraphics>().Texture2DAtlas = Assets!.Value.GetAsset<Texture2DAtlas>("GameAtlas");
        }
    }

    protected override void Update(GameTime gameTime)
    {
        CurrentGameTime = gameTime;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        Assets?.Value.Dispose();
        Container.Dispose();
        base.UnloadContent();
    }
}
