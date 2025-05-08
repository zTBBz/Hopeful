using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using Hopeful.Injection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using TinyMessenger;

namespace Hopeful;

public class GameCore : Game
{
    public static Vault RootVault { get; private set; } = null!;

    [Inject]
    public SystemRoot SystemRoot { get; } = null!;

    [Inject]
    public AssetManager Assets { get; } = null!;

    public GameTime CurrentGameTime { get; private set; } = null!;

    protected override void Initialize()
    {
        RootVault = new();
        RootVault.ExtractService<TinyMessengerHub>();
        RootVault.ExtractService<EntityStore>();
        RootVault.ExtractService(new SystemRoot(RootVault.InjectService<EntityStore>()));

        RootVault.LoadServices(Assembly.GetExecutingAssembly());
        RootVault.LoadInjections(Assembly.GetExecutingAssembly());

        base.Initialize();
    }

    /*private void InitializeMods()
    {
        List<string> paths = new();

        foreach (var path in paths)
        {
        if (!_modLoader.TryLoadModAssembly(path, out var assembly) || !_modLoader.TryGetBaseMod(assembly!, out var mod)) continue;

        var modContainer = c.CreateChild();
        var store = InjectionData.GetService<EntityStore>();
        modContainer.Register<SystemRoot>(Made.Of(() => new SystemRoot(store, mod!.ModId)), serviceKey: mod!.ModId); // make Lazy

        c.RegisterExports(assembly);

        // add to some list, later execute in GameCore.Update();
        }
    }*/

    protected override void LoadContent()
    {
        Task.Run(LoadAssets);
    }

    private async Task LoadAssets()
    {
        try
        {
            Stopwatch sw = Stopwatch.StartNew();
            await Assets.LoadAllAssetsAsync("Assets");
            sw.Stop();

            Debug.WriteLine($"All assets loaded. Time: {sw.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading assets: {ex.Message}");
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
        Assets.Dispose();
        RootVault.Dispose();
        base.UnloadContent();
    }
}
