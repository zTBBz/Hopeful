using System;
using System.Diagnostics.CodeAnalysis;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Localization, typeof(IAssetLoader))]
public sealed class LocalizationLoader : IAssetLoader
{
    public bool TryLoad(string path, [NotNullWhen(true)] out object? result)
    {
        throw new NotImplementedException();
    }
}