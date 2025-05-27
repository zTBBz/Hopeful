using System;
using System.Threading.Tasks;

namespace Hopeful.Asset;

public interface IAssetManager : IDisposable
{
    public event Action? OnAssetsLoaded;
    public bool IsDisposed { get; }
    Task LoadAllAssetsAsync(string assetsDirectory);
    Task LoadAssetAsync(string assetPath);
    T GetAsset<T>(string assetId) where T : class;
    bool TryGetAsset<T>(string assetId, out T? asset) where T : class;
}
