using Friflo.Engine.ECS;

namespace Hopeful.Utilities;
public static class EntityStoreExtensions
{
    public static Entity CreateUniqueEntity(this EntityStore s, string name)
        => s.CreateEntity(new UniqueEntity(name));
}
