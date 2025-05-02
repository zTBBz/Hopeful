using DryIocAttributes;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using Hopeful.Utilities;
using System.ComponentModel.Composition;

namespace Hopeful.Render.Texts;

public class TextRenderSystem : QuerySystem<Text>
{
    [Import] // make another render system for mods
    private readonly AssetManager _assets = null!;

    [Import]
    private readonly GlobalGraphics _graphics = null!;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref Text text, Entity entity) =>
        {
            if (_assets.TryGetFont(text.FontName, text.FontSize, out var font))
            {
                
            }
        });
    }
}
