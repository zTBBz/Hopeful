using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hopeful.Asset;

public sealed class AssetPerformanceTracker(IAssetManager inner) : Decorator<IAssetManager>(inner), IAssetManager
{
    public Task LoadAllAssetsAsync(string assetsDirectory)
    {
        var sw = Stopwatch.StartNew();
        var task = Inner.LoadAllAssetsAsync(assetsDirectory);
        sw.Stop();
        Debug.WriteLine($"[AssetPerformanceTracker] Loaded all assets in Directory {assetsDirectory} with {sw.ElapsedMilliseconds:F1} ms.");
        return task;
    }

    public Task LoadAssetAsync(string assetPath)
    {
        var sw = Stopwatch.StartNew();
        var task = Inner.LoadAssetAsync(assetPath);
        sw.Stop();
        Debug.WriteLine($"[AssetPerformanceTracker] Loaded Asset in Path {assetPath} with {sw.ElapsedMilliseconds:F1} ms.");
        return task;
    }

    public bool IsDisposed => Inner.IsDisposed;
    public event Action? OnAssetsLoaded;
    public void Dispose() => Inner.Dispose();
    public IEnumerable<string> SortAssets(IEnumerable<string> files) => Inner.SortAssets(files);
    public T GetAsset<T>(string assetId) where T : class => Inner.GetAsset<T>(assetId);
    public bool TryGetAsset<T>(string assetId, out T? asset) where T : class => Inner.TryGetAsset<T>(assetId, out asset);
}
