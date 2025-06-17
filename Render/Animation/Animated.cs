using Hopeful.Utilities;
using System;

namespace Hopeful.Render.Animation;

public struct Animated(string name, AnimatedFrame[] frames, bool isLooping, bool isReversed)
{
    public readonly string Name = name;
    public int CurrentFrame;
    public readonly AnimatedFrame[] Frames => frames;
    public TimeSpan CurrentFrameTime = frames.IsNotEmpty() ? frames[0].Duration : TimeSpan.Zero;
    public readonly int FrameCount => Frames.Length;
    public bool IsLooping = isLooping;
    public bool IsReversed = isReversed;
    public double Speed = 1.0f;
}
