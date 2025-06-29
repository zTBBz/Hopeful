using Hopeful.Asset.Loaders;
using Hopeful.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset;

public sealed class AssetManager(Vault vault) : IAssetManager
{
    private readonly ConcurrentDictionary<string, object> _assetsCache = new();

    public bool IsDisposed { get; private set; }

    public event Action? OnAssetsLoaded;

    public async Task LoadAllAssetsAsync(string assetsDirectory)
    {
        var assets = SortAssets(Directory.EnumerateFiles(PathHelper.GetAbsolutePath(assetsDirectory))); // not get assets from folders in Assets

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
                default:
                    sorted.Add(file);
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

    public Task LoadAssetAsync(string assetPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(assetPath, nameof(assetPath));
        PathHelper.ValidatePath(assetPath);

        var loader = vault.InjectService<IAssetLoader>(AssetDetector.DetectFormat(assetPath));
        if (!loader.TryLoad(assetPath, out var raw)) MainThread.EnqueueException(new AssetLoadException(assetPath, "invalid file format."));

        _assetsCache.TryAdd(Path.GetFileNameWithoutExtension(assetPath), raw!);

        return Task.CompletedTask;
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
