using Friflo.Engine.ECS;

namespace Hopeful.Render.Views;

public struct View() : IComponent
{
    public int CameraMask = 0b_0001;
}