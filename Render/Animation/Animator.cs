using Friflo.Engine.ECS;
using System.Collections.Generic;

namespace Hopeful.Render.Animation;

public struct Animator() : IComponent
{
    public readonly Dictionary<string, Animated> Animations = [];
    public string CurrentAnimation = string.Empty;
    public bool IsPaused = false;
    public bool IsPlaying = false;
}
