using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;

namespace Hopeful.Utilities;

public static class GameTimeExtensions
{
    public static UpdateTick ToUpdateTick(this ITime time)
        => new(time.DeltaTime, time.TotalTime);

    public static UpdateTick ToUpdateTick(this GameTime time)
        => new(time.ElapsedGameTime.Seconds, time.TotalGameTime.Seconds);
}