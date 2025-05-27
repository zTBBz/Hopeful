using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hopeful.Asset;

public sealed class AssetPerfomanceTracker(IAssetManager inner) : Decorator<IAssetManager>(inner), IAssetManager
{
    public Task LoadAllAssetsAsync(string assetsDirectory)
    {
        var sw = Stopwatch.StartNew();
        var task = _inner.LoadAllAssetsAsync(assetsDirectory);
        sw.Stop();
        Debug.WriteLine($"[AssetPerfomanceTracker] Loaded all assets in Directory {assetsDirectory} with {sw.ElapsedMilliseconds:F1} ms.");
        return task;
    }

    public Task LoadAssetAsync(string assetPath)
    {
        var sw = Stopwatch.StartNew();
        var task = _inner.LoadAssetAsync(assetPath);
        sw.Stop();
        Debug.WriteLine($"[AssetPerfomanceTracker] Loaded Asset in Path {assetPath} with {sw.ElapsedMilliseconds:F1} ms.");
        return task;
    }

    public bool IsDisposed => _inner.IsDisposed;
    public event Action? OnAssetsLoaded;
    public void Dispose() => _inner.Dispose();
    public T GetAsset<T>(string assetId) where T : class => _inner.GetAsset<T>(assetId);
    public bool TryGetAsset<T>(string assetId, out T? asset) where T : class => _inner.TryGetAsset<T>(assetId, out asset);
}
