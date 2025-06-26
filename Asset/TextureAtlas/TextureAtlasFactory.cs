using Hopeful.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hopeful.Asset.TextureAtlas;

[Service(typeof(ITextureAtlasFactory))]
public class TextureAtlasFactory : ITextureAtlasFactory
{
    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    public List<Texture2DAtlas> PackTextures(List<(string name, Texture2D texture)> textures, int maxAtlasSize)
    {
        var atlases = new List<Texture2DAtlas>();
        var currentAtlas = new Dictionary<string, Rectangle>();
        var currentAtlasData = new List<(string name, Texture2D texture, Rectangle region)>();
        int currentX = 0, currentY = 0, currentRowHeight = 0;

        foreach (var (name, texture) in textures)
        {
            if (currentX + texture.Width > maxAtlasSize)
            {
                currentX = 0;
                currentY += currentRowHeight;
                currentRowHeight = 0;
            }

            if (currentY + texture.Height > maxAtlasSize)
            {
                atlases.Add(CreateAtlas(currentAtlasData, maxAtlasSize));
                currentAtlas.Clear();
                currentAtlasData.Clear();
                currentX = 0;
                currentY = 0;
                currentRowHeight = 0;
            }

            currentAtlas[name] = new Rectangle(currentX, currentY, texture.Width, texture.Height);
            currentAtlasData.Add((name, texture, new Rectangle(currentX, currentY, texture.Width, texture.Height)));
            currentX += texture.Width;
            currentRowHeight = Math.Max(currentRowHeight, texture.Height);
        }

        if (currentAtlasData.IsNotEmpty())
        {
            var atlas = CreateAtlas(currentAtlasData, maxAtlasSize);
            atlases.Add(atlas);

            for (int i = 0; i < atlases.Count; i++)
            {
                var path = PathHelper.GetAssetAbsolutePath($"Atlases/TextureAtlas{i}.xml");
                atlases[i].SaveToXml(path);
            }
        }

        return atlases;
    }

    public Texture2DAtlas CreateAtlas(List<(string name, Texture2D texture, Rectangle region)> data, int size)
    {
        var atlasTexture = new Texture2D(_graphics.GraphicsDevice, size, size);
        var colors = new Color[size * size];
        Array.Fill(colors, Color.Transparent);

        foreach (var (_, texture, region) in data)
        {
            var textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);

            for (int y = 0; y < texture.Height; y++)
                for (int x = 0; x < texture.Width; x++)
                    colors[(region.Y + y) * size + region.X + x] = textureData[y * texture.Width + x];
        }

        atlasTexture.SetData(colors);

        var regions = data.ToDictionary(d => d.name, d => d.region);
        return new(atlasTexture, regions);
    }
}
