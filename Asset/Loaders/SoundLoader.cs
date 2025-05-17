using Microsoft.Xna.Framework.Audio;
using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

[Service(AssetFormat.Audio)]
public class SoundLoader : IAssetLoader
{
    public Task<object> Load(string path)
        => (Task<object>)(object)SoundEffect.FromFile(path);
}
