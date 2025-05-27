using Hopeful.Utilities;
using System;

namespace Hopeful.Render.Animations;

public struct Animation(string name, AnimationFrame[] frames, bool isLooping, bool isReversed)
{
    public readonly string Name = name;
    public int CurrentFrame;
    public readonly AnimationFrame[] Frames => frames;
    public TimeSpan CurrentFrameTime = frames.IsNotEmpty() ? frames[0].Duration : TimeSpan.Zero;
    public readonly int FrameCount => Frames.Length;
    public bool IsLooping = isLooping;
    public bool IsReversed = isReversed;
    public double Speed = 1.0f;
}
