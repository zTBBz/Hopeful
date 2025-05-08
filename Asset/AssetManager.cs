using FontStashSharp;
using Hopeful.Asset.Loaders;
using Hopeful.Injection;
using Hopeful.Utilities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset;

[Service]
public sealed class AssetManager
{
    private readonly ConcurrentDictionary<string, object> _assetsCache = new();

    public event Action? OnAssetsLoaded;

    public bool IsDisposed { get; private set; }

    internal async Task LoadAllAssetsAsync(string assetsDirectory)
    {
        var assets = Directory.EnumerateFiles(PathHelper.GetAssetAbsolutePath(assetsDirectory));

        await Parallel.ForEachAsync(assets, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (assetPath, token) =>
        {
            await LoadAssetAsync(assetPath).ConfigureAwait(false);
        });

        /*foreach (var asset in assets)
            await LoadAssetAsync(asset).ConfigureAwait(false);*/
        OnAssetsLoaded?.Invoke();
    }

    public T GetAsset<T>(string assetId) where T : class
    {
        if (TryGetAsset<T>(assetId, out var asset))
            return asset!;
        throw new AssetLoadException(assetId, $"{typeof(T)} type is not loaded yet.");
    }

    public bool TryGetAsset<T>(string assetId, out T? asset) where T : class
    {
        asset = null;
        if (_assetsCache.TryGetValue(assetId + GetAssetSuffix<T>(), out var raw))
            asset = (T)raw;
        return asset != null;
    }

    private async Task LoadAssetAsync(string assetPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(assetPath, nameof(assetPath));
        PathHelper.ValidatePath(assetPath);

        object? raw = null;
        try
        {
            raw = await LoadRaw(assetPath);
        }
        finally
        {
            if (raw != null)
                _assetsCache.TryAdd(Path.GetFileNameWithoutExtension(assetPath), raw);
        }
    }

    private void UnloadAsset(string assetId)
    {
        if (_assetsCache.TryRemove(assetId, out var obj))
            if (obj is IDisposable disposable)
                disposable.Dispose();
    }

    private static async Task<object> LoadRaw(string assetPath)
    {
        object? raw = null;
        var format = DetectFormat(assetPath);
        var loader = GameCore.RootVault.InjectService<IAssetLoader>(format);
        raw = await loader.Load(assetPath);

        return raw ?? throw new AssetLoadException(assetPath, "invalid file format.");
    }

    #region Format detecting
    private static AssetFormat DetectFormat(string assetPath)
    {
        var extension = Path.GetExtension(assetPath);
        switch (extension)
        {
            case ".png" or ".dds" or ".jpeg" or ".jpg":
                return AssetFormat.Sprite;
            case ".mp3" or ".ogg":
                return AssetFormat.Audio;
            case ".loc":
                return AssetFormat.Localization;
            case ".ttf":
                return AssetFormat.Font;
            default:
                throw new AssetLoadException(assetPath, "invalid file format.");
        }
    }

    private static string GetAssetSuffix<T>() where T : class
    {
        var type = nameof(T);
        switch (type)
        {
            case nameof(Texture2D):
                return AssetFormat.Sprite.ToString();
            case nameof(SoundEffect):
                return AssetFormat.Audio.ToString();
            case nameof(SpriteEffect):
                return AssetFormat.Shader.ToString();
            case nameof(FontSystem):
                return AssetFormat.Font.ToString();
            default:
                return string.Empty;
        }
    }
    #endregion

    internal void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;

        foreach (var obj in _assetsCache.Values)
            if (obj is IDisposable disposable)
                disposable.Dispose();

        _assetsCache.Clear();
    }
}
