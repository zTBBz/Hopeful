using Friflo.Engine.ECS.Systems;
using Microsoft.Xna.Framework;

namespace Hopeful.Render.Views;

public sealed class CameraSystem : QuerySystem<Camera>
{
    protected override void OnUpdate()
    {
        foreach (var cameraEntity in Query.Entities)
        {
            var camera = cameraEntity.GetComponent<Camera>();
            if (!camera.IsActive) return;

            camera.Zoom = MathHelper.Clamp(camera.Zoom, camera.MinZoom, camera.MaxZoom);
            camera.TransformMatrix = Matrix.CreateTranslation(new Vector3(-camera.Position, 0)) *
            Matrix.CreateScale(camera.Zoom) *
            Matrix.CreateTranslation(new Vector3(camera.Viewport.Width * 0.5f, camera.Viewport.Height * 0.5f, 0));
        }
    }
}
