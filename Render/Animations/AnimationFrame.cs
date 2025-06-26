using System;

namespace Hopeful.Render.Animations;

public readonly struct AnimationFrame(string name, TimeSpan duration)
{
    public string Name { get; } = name;
    public TimeSpan Duration { get; } = duration;
}
