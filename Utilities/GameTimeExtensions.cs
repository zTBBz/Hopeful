using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;

namespace Hopeful.Utilities;

public static class GameTimeExtensions
{
    public static UpdateTick ToUpdateTick(this GameTime time)
        => new(time.ElapsedGameTime.Seconds, time.TotalGameTime.Seconds);

    public static float DeltaTime(this GameTime time)
        => (float)time.ElapsedGameTime.TotalSeconds;

    public static float TotalTime(this GameTime time)
        => (float)time.TotalGameTime.TotalSeconds;
}