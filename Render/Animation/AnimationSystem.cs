using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Microsoft.Xna.Framework;
using System;

namespace Hopeful.Render.Animation;

public sealed class AnimationSystem(Vault vault) : InjectQuerySystem<Animator, Sprite>(vault)
{
    [Inject]
    private readonly GameCore _game = null!;

    protected override void OnUpdate()
        => Query.Each(new AnimateEach(_game.CurrentGameTime));
}

public readonly struct AnimateEach(GameTime gameTime) : IEach<Animator, Sprite>
{
    public void Execute(ref Animator animator, ref Sprite sprite)
    {
        if (!animator.IsPlaying || animator.IsPaused || !sprite.IsVisible) return;

        TimeSpan elapsedTime = gameTime.ElapsedGameTime;
        TimeSpan remainingTime = TimeSpan.Zero;

        if (!animator.Animations.TryGetValue(animator.CurrentAnimation, out var animation)) return;

        animation.CurrentFrameTime -= elapsedTime * animation.Speed;

        if (animation.CurrentFrameTime <= TimeSpan.Zero)
        {
            //  End the current frame
            remainingTime += -animation.CurrentFrameTime;

            AdvanceFrame(ref animator, ref animation);

            if (!animator.IsPlaying) return;

            sprite.SpriteName = animation.Frames[animation.CurrentFrame].Name;

            animation.CurrentFrameTime -= remainingTime;
            remainingTime = TimeSpan.Zero;
        }
    }

    private static void AdvanceFrame(ref Animator animator, ref Animated animation)
    {
        //  Increment the current frame
        animation.CurrentFrame += GetFrameDirection(animation.IsReversed);

        //  Ensure frame is in bounds
        if (animation.CurrentFrame < 0 || animation.CurrentFrame >= animation.FrameCount)
        {
            if (animation.IsLooping)
                animation.CurrentFrame = animation.IsReversed ? animation.FrameCount - 1 : 0;
            else
            {
                animation.CurrentFrame -= GetFrameDirection(animation.IsReversed);
                animator.IsPlaying = false;
                return;
            }
        }

        animation.CurrentFrameTime = animation.Frames[animation.CurrentFrame].Duration;
    }

    private static int GetFrameDirection(bool revers)
    => revers ? -1 : 1;
}
