using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using Hopeful.Render.View;
using Hopeful.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hopeful.Render;

public class SpriteRenderSystem : QuerySystem
{
    [Inject]
    private readonly AssetManager _assets = null!;

    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    protected override void OnAddStore(EntityStore store)
    {
        GameCore.RootVault.Inject(this);
        base.OnAddStore(store);
    }

    protected override void OnUpdate()
    {
        var cameraQuery = Query.Store.Query<Camera>();

        foreach (var cameraEntity in cameraQuery.Entities)
        {
           var camera = cameraEntity.GetComponent<Camera>();

            if (!camera.IsActive) continue;

            var viewedQuery = Query.Store.Query<Sprite, Viewed>();

            _graphics.SpriteBatch.Begin(transformMatrix: camera.TransformMatrix);

            foreach (var entity in viewedQuery.Entities)
            {
                if ((entity.GetComponent<Viewed>().CameraMask & camera.Mask) == 0) continue;

                Draw(ref entity.GetComponent<Sprite>(), entity);
            }

            _graphics.SpriteBatch.End();
        }

        // Draw "static" textures, without Camera Matrix
        _graphics.SpriteBatch.Begin();

        foreach (var entity in Query.Entities)
            Draw(ref entity.GetComponent<Sprite>(), entity);

        _graphics.SpriteBatch.End();
    }

    private void Draw(ref Sprite sprite, Entity entity)
    {
        if (!sprite.IsVisible) return;

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

    private static void UpdateSpriteCache(Rectangle? rect, Texture2D? texture, Sprite sprite, Entity entity)
        => entity.AddComponent<SpriteCache>(new() { AtlasRect = rect, SingleTexture = texture });
}
