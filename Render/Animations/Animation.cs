using Friflo.Engine.ECS;
using System;

namespace Hopeful.Render.Animations;

public struct Animation(string name, AnimationFrame[] frames, bool isLooping, bool isReversed) : IComponent
{
    public string Name { get; } = name;
    public int CurrentFrame;
    public readonly AnimationFrame[] Frames => frames;
    public TimeSpan CurrentFrameTime;
    public readonly int FrameCount => Frames.Length;
    public bool IsPaused = false;
    public bool IsLooping = isLooping;
    public bool IsReversed = isReversed;
    public bool IsPlaying = false;
    public double Speed = 1.0f;
}
