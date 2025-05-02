using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System;
using System.ComponentModel.Composition;

namespace Hopeful.Render.Animations;

public class AnimationSystem : QuerySystem<Animation>
{
    [Import]
    private readonly GameCore _game = null!;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref Animation animation, Entity entity) =>
        {
            if (!animation.IsPlaying || animation.IsPaused) return;
            if (!entity.TryGetComponent<Sprite>(out var sprite) || !sprite.IsVisible || !entity.Enabled) return;

            TimeSpan elapsedTime = _game.CurrentGameTime.ElapsedGameTime;
            TimeSpan remainingTime = TimeSpan.Zero;

            animation.CurrentFrameTime -= elapsedTime * animation.Speed;

            if (animation.CurrentFrameTime <= TimeSpan.Zero)
            {
                //  End the current frame
                remainingTime += -animation.CurrentFrameTime;

                AdvanceFrame(animation);

                if (!animation.IsPlaying) return;

                animation.CurrentFrameTime -= remainingTime;
                remainingTime = TimeSpan.Zero;
            }

        });
    }

    private void AdvanceFrame(Animation animation)
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
                animation.IsPlaying = false;
                return;
            }
        }

        animation.CurrentFrameTime = animation.Frames[animation.CurrentFrame].Duration;
    }

    private int GetFrameDirection(bool revers)
        => revers ? -1 : 1;
}
