using System;

namespace Hopeful.Render.Animation;

public readonly struct AnimatedFrame(string name, TimeSpan duration)
{
    public string Name { get; } = name;
    public TimeSpan Duration { get; } = duration;
}
