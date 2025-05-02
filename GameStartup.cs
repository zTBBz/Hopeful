using DryIoc;
using DryIoc.MefAttributedModel;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using Hopeful.Asset.Loaders;
using Hopeful.Input;
using Hopeful.Mod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using TinyMessenger;

namespace Hopeful;

public class GameStartup
{
    [Import]
    private GameCore _game = null!;

    [Import]
    private ModLoader _modLoader = null!;

    public void Initialize()
    {
        var container = new Container();
        container.Register<GameCore>(Reuse.Singleton, Made.Of(() => new GameCore(container)));

        CoreRegister(container);
        PreRegister(container);
        container.RegisterExports(Assembly.GetExecutingAssembly());
        PostRegister(container);

        //InitializeMods(container);
    }

    private void InitializeMods(Container c)
    {
        List<string> paths = new();

        foreach (var path in paths)
        {
            if (!_modLoader.TryLoadModAssembly(path, out var assembly) || !_modLoader.TryGetBaseMod(assembly!, out var mod)) continue;

            var modContainer = c.CreateChild();
            var store = c.Resolve<EntityStore>();
            modContainer.Register<SystemRoot>(Made.Of(() => new SystemRoot(store, mod!.ModId)), serviceKey: mod!.ModId); // make Lazy

            c.RegisterExports(assembly);

            // add to some list, later execute in GameCore.Update();
        }
    }

    private void CoreRegister(Container c)
    {
        // Base register
        c.Register<GlobalGraphics>(Reuse.Singleton, Made.Of(() => new GlobalGraphics(new GraphicsDeviceManager(_game), _game.GraphicsDevice, _game.GraphicsDevice.Viewport)));
        c.Register<TinyMessengerHub>(Reuse.Singleton);
        c.Register<AssetManager>(Reuse.Singleton);
        c.Register<IInputListener, BaseInput>(Reuse.Singleton);
        c.Register<ICamera, BaseCamera>(Reuse.Singleton);

        // Factory register (maybe use Lazy)
        c.Register<ITextureAtlasFactory, TextureAtlasFactory>(Reuse.Singleton);

        // Assets loading register
        c.Register<IAssetLoader, TextureLoader>(Reuse.Singleton, serviceKey: AssetFormat.Sprite);
        c.Register<IAssetLoader, FontLoader>(Reuse.Singleton, serviceKey: AssetFormat.Font);
        c.Register<IAssetLoader, SoundLoader>(Reuse.Singleton, serviceKey: AssetFormat.Audio);
        c.Register<IAssetLoader, LocalizationLoader>(Reuse.Singleton, serviceKey: AssetFormat.Localization);
        c.Register<IAssetLoader, ShaderLoader>(Reuse.Singleton, serviceKey: AssetFormat.Shader);

        // Mod register
        c.Register<ModLoader>(Reuse.Singleton);

        // ECS register (maybe use Lazy)
        c.Register<EntityStore>(Reuse.Singleton);
        var store = c.Resolve<EntityStore>();
        c.Register<SystemRoot>(Reuse.Singleton, Made.Of(() => new SystemRoot(store, null)));

        // Utils 
        c.RegisterDelegate<ITime>(() => new Time(c.Resolve<GameCore>().CurrentGameTime), Reuse.Singleton);
    }

    public void Run() => _game.Run();
    public virtual void PreRegister(Container c) { }
    public virtual void PostRegister(Container c) { }
}
