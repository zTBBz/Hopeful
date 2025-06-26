using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Hopeful.Asset.TextureAtlas;

public class Texture2DAtlas(Texture2D atlasTexture,  Dictionary<string, Rectangle> textures) : IDisposable
{
    public Texture2D AtlasTexture { get; } = atlasTexture;

    private readonly Dictionary<string, Rectangle> _textures = textures;

    public bool TryGetTextureRegion(string textureName, out Rectangle? rect)
    {
        rect = null;
        if (_textures.TryGetValue(textureName, out var result)) rect = result;
        return rect != null;
    }

    internal void SaveToXml(string filePath)
    {
        var root = new XElement("TextureAtlas");

        foreach (var (name, region) in _textures)
        {
            var textureElement = new XElement("Texture",
                new XAttribute("name", name),
                new XAttribute("x", region.X),
                new XAttribute("y", region.Y),
                new XAttribute("width", region.Width),
                new XAttribute("height", region.Height)
            );
            root.Add(textureElement);
        }

        var document = new XDocument(root);
        document.Save(filePath);
    }

    internal static Texture2DAtlas LoadFromXml(string xmlPath, Texture2D atlasTexture)
    {
        XDocument doc = XDocument.Load(xmlPath);
        var textureElements = doc.Root?.Elements("Texture");

        if (textureElements == null || !textureElements.Any()) throw new InvalidOperationException("Invalid XML file or no textures defined.");

        var textures = new Dictionary<string, Rectangle>();
        foreach (var element in textureElements)
        {
            string? name = element.Attribute("name")?.Value;
            if (name == null) throw new XmlException($"Failed loading XML {typeof(Texture2DAtlas)} texture name is null.");
            int x = int.Parse(element.Attribute("x")?.Value ?? "0");
            int y = int.Parse(element.Attribute("y")?.Value ?? "0");
            int width = int.Parse(element.Attribute("width")?.Value ?? "0");
            int height = int.Parse(element.Attribute("height")?.Value ?? "0");

            if (!string.IsNullOrEmpty(name))
                textures[name] = new Rectangle(x, y, width, height);
        }

        return new(atlasTexture, textures);
    }

    public void Dispose()
    {
        AtlasTexture.Dispose();
        _textures.Clear();
    }
}
