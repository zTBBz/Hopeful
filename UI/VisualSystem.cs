using Friflo.Engine.ECS.Systems;
using Microsoft.Xna.Framework;

namespace Hopeful.UI;

public sealed class VisualSystem(Vault vault) : InjectBaseSystem(vault)
{
    [Inject]
    private readonly GlobalGraphics _graphics = null!;

    protected override void OnUpdateGroup()
    {
        var visualQuery = Store.Query<Visual>();

        foreach (var entity in visualQuery.Entities)
        {
            var visual = entity.GetComponent<Visual>();

            if (_graphics.IsWindowSizeChanged) visual.IsDirty = true;

            if (visual.IsDirty)
            {
                float parentWidth = _graphics.Viewport.Width;
                float parentHeight = _graphics.Viewport.Height;
                Vector2 parentPosition = Vector2.Zero;

                var parent = entity.Parent;
                if (parent.TryGetComponent<Visual>(out var parentVisual))
                {
                    parentWidth = parentVisual.Size.X;
                    parentHeight = parentVisual.Size.Y;
                    parentPosition = parentVisual.Position;
                }

                float x = parentPosition.X + visual.LeftUnit.ToPixels(parentWidth, _graphics.Viewport);
                float y = parentPosition.Y + visual.TopUnit.ToPixels(parentHeight, _graphics.Viewport);

                float width = visual.WidthUnit.ToPixels(parentWidth, _graphics.Viewport);
                float height = visual.HeightUnit.ToPixels(parentHeight, _graphics.Viewport);

                visual.Position = new(x, y);
                visual.Size = new(width, height);

                visual.IsDirty = false;
            }
        }
    }
}
