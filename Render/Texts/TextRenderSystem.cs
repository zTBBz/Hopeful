using Friflo.Engine.ECS;
using Hopeful.Asset;
using Hopeful.Injection;
using Hopeful.Injection.System;
using Hopeful.Utilities;

namespace Hopeful.Render.Texts;

public class TextRenderSystem : InjectQuerySystem<Text>
{
    [Inject] // make another render system for mods
    private readonly AssetManager _assets = null!;

    [Inject]
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
