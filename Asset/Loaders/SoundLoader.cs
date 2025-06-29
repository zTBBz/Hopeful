using System;
using System.Diagnostics.CodeAnalysis;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Audio, typeof(IAssetLoader))]
public sealed class SoundLoader : IAssetLoader
{
    public bool TryLoad(string path, [NotNullWhen(true)] out object? result)
        => throw new NotImplementedException();
}
