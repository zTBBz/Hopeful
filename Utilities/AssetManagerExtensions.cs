using Hopeful.Asset;
using Hopeful.Asset.TextureAtlas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hopeful.Utilities;

public static class AssetManagerExtensions
{
    /// <summary>
    /// Attempts to retrieve a texture and its associated rectangle (if it exists in an atlas).
    /// </summary>
    /// <param name="textureId">The unique identifier of the texture to retrieve.</param>
    /// <param name="rect">
    ///     When this method returns, contains the rectangle defining the texture's region in the atlas (if found in the atlas);
    ///     otherwise, <see langword="null"/>. This parameter is passed uninitialized.
    /// </param>
    /// <param name="texture">
    ///     When this method returns, contains the texture (either the atlas texture or a standalone texture);
    ///     otherwise, <see langword="null"/>. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the texture was found (either in the atlas or as a standalone texture);
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// If the texture exists in the atlas, <paramref name="rect"/> will contain its region, and <paramref name="texture"/> will be the atlas texture.
    /// If the texture is standalone (not in the atlas), <paramref name="rect"/> will be <see langword="null"/>, and <paramref name="texture"/> will be the standalone texture.
    /// If the texture is not found, both <paramref name="rect"/> and <paramref name="texture"/> will be <see langword="null"/>.
    /// </remarks>
    public static bool TryGetTexture(this IAssetManager manager, string textureId, out Rectangle? rect, out Texture2D? texture)
    {
        rect = null;
        texture = null;

        if (manager.TryGetAsset<Texture2DAtlas>("GameAtlas", out var atlas))
        {
            if (atlas.TryGetTextureRegion(textureId, out var region))
            {
                rect = region;
                texture = atlas.AtlasTexture;
                return true;
            }
        }

        if (manager.TryGetAsset<Texture2D>(textureId, out var asset))
        {
            texture = asset;
            return true;
        }

        return false;
    }

    public static Rectangle GetTextureRect(this IAssetManager manager, string textureId)
    {
        if (manager.GetAsset<Texture2DAtlas>("GameAtlas").TryGetTextureRegion(textureId, out var region)) return region!.Value;
        throw new AssetLoadException(textureId, "is not exist in texture atlas.");
    }
}
