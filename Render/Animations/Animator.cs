using Friflo.Engine.ECS;
using System.Collections.Generic;

namespace Hopeful.Render.Animations;

public struct Animator() : IComponent
{
    public readonly Dictionary<string, Animation> Animations = [];
    public string CurrentAnimation = string.Empty;
    public bool IsPaused = false;
    public bool IsPlaying = false;
}
