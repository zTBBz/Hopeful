using System.Diagnostics.CodeAnalysis;

namespace Hopeful.Asset.Loaders;

public interface IAssetLoader
{
    public bool TryLoad(string path, [NotNullWhen(true)] out object? result);
}
