using Hopeful.Asset.Loaders;
using Hopeful.Utilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Hopeful.Asset;

[Service(typeof(IAssetManager))]
public sealed class AssetManager(Vault vault) : IAssetManager, IDisposable
{
    private readonly ConcurrentDictionary<string, object> _assetsCache = new();
    private readonly Vault _vault = vault;

    private bool _isDisposed;
    public bool IsDisposed => _isDisposed;

    public event Action? OnAssetsLoaded;

    public async Task LoadAllAssetsAsync(string assetsDirectory)
    {
        var assets = Directory.EnumerateFiles(PathHelper.GetAssetAbsolutePath(assetsDirectory));

        await Parallel.ForEachAsync(assets, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (assetPath, token) =>
        {
            await LoadAssetAsync(assetPath).ConfigureAwait(false);
        });

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
            if (raw != null)
                _assetsCache.TryAdd(Path.GetFileNameWithoutExtension(assetPath), raw);
        }
    }

    private async Task<object> LoadRaw(string assetPath)
    {
        var format = AssetDetector.DetectFormat(assetPath);
        var loader = _vault.InjectService<IAssetLoader>(format);
        object? raw = await loader.Load(assetPath);

        return raw ?? throw new AssetLoadException(assetPath, "invalid file format.");
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        foreach (var obj in _assetsCache.Values)
            if (obj is IDisposable disposable)
                disposable.Dispose();

        _assetsCache.Clear();
    }
}
