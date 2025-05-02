using System.Threading.Tasks;

namespace Hopeful.Asset.Loaders;

public interface IAssetLoader
{
    Task<object> Load(string path);
}
