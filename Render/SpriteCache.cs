using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hopeful.Render;

public struct SpriteCache : IComponent
{
    public Rectangle? AtlasRect;
    public Texture2D? SingleTexture;
}