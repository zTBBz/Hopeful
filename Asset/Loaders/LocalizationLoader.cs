using System;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Localization)]
public class LocalizationLoader : IAssetLoader
{
    public Task<object> Load(string path)
    {
        throw new NotImplementedException();
    }
}