using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Hopeful.Asset.TextureAtlas;

public interface ITextureAtlasFactory
{
    Texture2DAtlas CreateAtlas(List<(string name, Texture2D texture, Rectangle region)> data, int size);
    List<Texture2DAtlas> PackTextures(List<(string name, Texture2D texture)> textures, int maxAtlasSize);
}
