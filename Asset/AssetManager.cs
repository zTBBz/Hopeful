using Hopeful.Asset.Loaders;
using Hopeful.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset;

[Service(typeof(IAssetManager))]
public sealed class AssetManager(Vault vault) : IAssetManager
{
    private readonly ConcurrentDictionary<string, object> _assetsCache = new();

    public bool IsDisposed { get; private set; }

    public event Action? OnAssetsLoaded;

    public async Task LoadAllAssetsAsync(string assetsDirectory)
    {
        var assets = SortAssets(Directory.EnumerateFiles(PathHelper.GetAssetAbsolutePath(assetsDirectory)));

        await Parallel.ForEachAsync(assets, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (assetPath, token) =>
        {
            await LoadAssetAsync(assetPath).ConfigureAwait(false);
        });

        OnAssetsLoaded?.Invoke();
    }

    public IEnumerable<string> SortAssets(IEnumerable<string> files)
    {
        List<string> sorted = [];
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file);

            switch (extension)
            {
                case ".atlas":
                    sorted.Insert(0, file);
                    break;
            }
        }

        return sorted;
    }

    public T GetAsset<T>(string assetId) where T : class
    {
        if (TryGetAsset<T>(assetId, out var asset))
            return asset;
        throw new AssetLoadException(assetId, $"{typeof(T)} type is not loaded yet.");
    }

    public bool TryGetAsset<T>(string assetId, [NotNullWhen(true)] out T? asset) where T : class
    {
        asset = null;
        if (IsDisposed) return false;

        if (_assetsCache.TryGetValue(assetId + AssetDetector.GetAssetSuffix<T>(), out var raw))
            asset = (T)raw;
        return asset != null;
    }

    public async Task LoadAssetAsync(string assetPath)
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
            if (raw != null) _assetsCache.TryAdd(Path.GetFileNameWithoutExtension(assetPath), raw);
        }
    }

    private async Task<object> LoadRaw(string assetPath)
    {
        var format = AssetDetector.DetectFormat(assetPath);
        var loader = vault.InjectService<IAssetLoader>(format);
        var raw = await loader.Load(assetPath);

        return raw ?? throw new AssetLoadException(assetPath, "invalid file format.");
    }

    public void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;

        foreach (var obj in _assetsCache.Values)
            if (obj is IDisposable disposable)
                disposable.Dispose();

        _assetsCache.Clear();
    }
}
