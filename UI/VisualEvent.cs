using Friflo.Engine.ECS;
using Hopeful.Render;

namespace Hopeful.UI;

public struct VisualEvent(Entity publisher, Entity subscriber, Visual visual, Sprite sprite)
{
    public Entity Publisher = publisher;
    public Entity Subscriber = subscriber;
    public Visual Visual = visual;
    public Sprite Sprite = sprite;
}
