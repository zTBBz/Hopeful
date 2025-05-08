using Friflo.Engine.ECS;
using Hopeful.Asset;
using Hopeful.Injection;
using Hopeful.Injection.System;
using Hopeful.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hopeful.Render;

public class SpriteRenderSystem : InjectQuerySystem<Sprite>
{
    [Inject] // make another render system for mods
    private readonly AssetManager _assets = null!;

    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    protected override void OnUpdate()
    {
        _graphics.SpriteBatch.Begin();

        Query.ForEachEntity((ref Sprite sprite, Entity entity) =>
        {
            if (!sprite.IsVisible || !entity.Enabled) return;

            var isAtlasSprite = GetSpriteData(sprite, entity, out var textureRect, out var texture);

            UpdateSpriteCache(textureRect, isAtlasSprite ? null : texture, sprite, entity);

            //if (texture is null) return;

            _graphics.SpriteBatch.Draw(
                texture,
                sprite.Offset,
                textureRect,
                Color.White,
                0f,
                Vector2.Zero,
                sprite.Scale,
                SpriteEffects.None,
                sprite.Layer);
        });

        _graphics.SpriteBatch.End();
    }

    private bool GetSpriteData(Sprite sprite, Entity entity, out Rectangle? rect, out Texture2D? texture)
    {
        rect = null;
        texture = null;

        if (entity.TryGetComponent<SpriteCache>(out var cache))
        {
            rect = cache.AtlasRect;
            texture = cache.SingleTexture;
        }
        else if (_assets.TryGetTexture(sprite.SpriteName, out var rectangle, out var textureResult))
        {
            rect = rectangle;
            texture = textureResult;
        }
        else
            throw new InvalidOperationException($"Sprite '{sprite.SpriteName}' not found in atlas or as a separate texture.");

        if (rect is null) return false;
        else return true;
    }

    private void UpdateSpriteCache(Rectangle? rect, Texture2D? texture, Sprite sprite, Entity entity)
        => entity.AddComponent<SpriteCache>(new() { AtlasRect = rect, SingleTexture = texture });
}
