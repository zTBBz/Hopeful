using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Microsoft.Xna.Framework;

namespace Hopeful.Render.View;

public class CameraSystem : QuerySystem<Camera>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref Camera camera, Entity entity) =>
        {
            if (!camera.IsActive) return;

            camera.Zoom = MathHelper.Clamp(camera.Zoom, camera.MinZoom, camera.MaxZoom);

            camera.TransformMatrix = Matrix.CreateTranslation(new Vector3(-camera.Position, 0)) *
                             Matrix.CreateScale(camera.Zoom) *
                             Matrix.CreateTranslation(new Vector3(camera.Viewport.Width * 0.5f, camera.Viewport.Height * 0.5f, 0));
        });
    }
}
