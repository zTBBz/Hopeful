using Friflo.Engine.ECS;

namespace Hopeful.Render.View;

public struct Viewed() : IComponent
{
    public int CameraMask = 0b_0001;
}