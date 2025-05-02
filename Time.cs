using Microsoft.Xna.Framework;

namespace Hopeful;

public interface ITime
{
    float DeltaTime { get; }
    float TotalTime { get; }
}

public readonly struct Time(GameTime gameTime) : ITime
{
    public float DeltaTime => (float)gameTime.ElapsedGameTime.TotalSeconds;
    public float TotalTime => (float)gameTime.TotalGameTime.TotalSeconds;
}
