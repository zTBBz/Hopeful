using System;
using Friflo.Engine.ECS.Systems;
using Friflo.Engine.ECS;
using Hopeful.Asset;
using Hopeful.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hopeful.Render.Animations.Obsolete;

/*
public interface IAnimator : IDisposable
{
    bool IsPaused { get; }
    bool IsPlaying { get; }
    bool IsLooping { get; set; }
    bool IsReversed { get; set; }
    double Speed { get; set; }
    event Action<IAnimator, AnimationEventTrigger> OnAnimationEvent;
    TimeSpan CurrentFrameTimeRemaining { get; }
    int CurrentFrame { get; }
    int FrameCount { get; }
    void SetFrame(int index);
    bool Play();
    bool Play(int startingFrame);
    bool Pause();
    bool Pause(bool resetFrameDuration);
    bool Unpause();
    bool Unpause(bool advanceToNextFrame);
    void Update(GameTime gameTime);
    bool Stop();
    void Reset();
}

public class BaseAnimator : IAnimator
{
    [Import]
    private AssetManager _assets = null!;

    private readonly IAnimation _definition;
    private int _direction;
    private int _internalFrame;

    public bool IsPaused { get; private set; }
    public bool IsPlaying { get; private set; }
    public bool IsLooping { get; set; }
    public bool IsReversed
    {
        get => _direction == -1;
        set => _direction = value ? -1 : 1;
    }
    public double Speed { get; set; }

    public event Action<IAnimator, AnimationEventTrigger> OnAnimationEvent;

    public TimeSpan CurrentFrameTimeRemaining { get; private set; }
    public int CurrentFrame => _definition.Frames[_internalFrame].FrameIndex;
    public int FrameCount => _definition.FrameCount;

    public BaseAnimator(string animationId)
    {
        IsLooping = definition.IsLooping;
        IsReversed = definition.IsReversed;
        Speed = 1.0f;

        Play();
    }

    public bool Pause() => Pause(false);

    public bool Pause(bool resetFrameDuration)
    {
        //  We can only pause something that is animating and not already paused.  This is to prevent situations
        //  that could accidentally reset frame duration if it was set to true.
        if (!IsPlaying || IsPaused) return false;

        IsPaused = true;

        if (resetFrameDuration)
            CurrentFrameTimeRemaining = _definition.Frames[_internalFrame].Duration;

        return true;
    }

    public bool Play() => Play(0);

    public bool Play(int startingFrame)
    {
        if (startingFrame < 0 || startingFrame >= _definition.FrameCount)
            throw new ArgumentOutOfRangeException(nameof(startingFrame), $"{nameof(startingFrame)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(BaseAnimator)}");

        if (IsPlaying)
            return false;

        IsPlaying = true;
        _internalFrame = startingFrame;
        CurrentFrameTimeRemaining = _definition.Frames[_internalFrame].Duration;
        return true;
    }

    public void Reset()
    {
        IsReversed = _definition.IsReversed;
        IsLooping = _definition.IsLooping;
        IsPlaying = false;
        IsPaused = true;
        Speed = 1.0d;
        _internalFrame = IsReversed ? _definition.FrameCount - 1 : 0;
        CurrentFrameTimeRemaining = _definition.Frames[_internalFrame].Duration;
    }

    public void SetFrame(int index)
    {
        if (index < 0 || index >= _definition.FrameCount)
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be less than zero or greater than or equal to the total number of frames in this {nameof(BaseAnimator)}");

        _internalFrame = index;
        CurrentFrameTimeRemaining = _definition.Frames[_internalFrame].Duration;
        OnAnimationEvent?.Invoke(this, AnimationEventTrigger.FrameBegin);
    }

    public bool Stop() => Stop(AnimationEventTrigger.AnimationStopped);

    private bool Stop(AnimationEventTrigger trigger)
    {
        //  We can't stop something that's not animating.  This is to prevent accidentally invoking OnAnimationEnd
        if (!IsPlaying) return false;

        IsPlaying = false;
        IsPaused = true;
        OnAnimationEvent?.Invoke(this, trigger);
        return true;
    }

    public bool Unpause() => Unpause(false);

    public bool Unpause(bool advanceToNextFrame)
    {
        //  We can't unpause something that's not animating and also isn't paused. This is to prevent improper usage
        //  that could accidentally advance to the next frame if it was set to true.
        if (!IsPlaying || !IsPaused) return false;

        IsPaused = false;

        if (advanceToNextFrame) AdvanceFrame();

        return true;
    }

    public void Update(GameTime gameTime)
    {
        TimeSpan elapsedTime = gameTime.ElapsedGameTime;
        TimeSpan remainingTime = TimeSpan.Zero;

        if (!IsPlaying || IsPaused) return;

        CurrentFrameTimeRemaining -= elapsedTime * Speed;

        while (CurrentFrameTimeRemaining <= TimeSpan.Zero)
        {
            remainingTime += -CurrentFrameTimeRemaining;

            //  End the current frame
            OnAnimationEvent?.Invoke(this, AnimationEventTrigger.FrameEnd);

            if (!AdvanceFrame()) break;

            CurrentFrameTimeRemaining -= remainingTime;
            remainingTime = TimeSpan.Zero;
        }
    }

    private bool AdvanceFrame()
    {
        //  Increment the current frame
        _internalFrame += _direction;

        //  Ensure frame is in bounds
        if (_internalFrame < 0 || _internalFrame >= _definition.FrameCount)
        {
            if (IsLooping)
            {
                _internalFrame = IsReversed ? _definition.FrameCount - 1 : 0;
                OnAnimationEvent?.Invoke(this, AnimationEventTrigger.AnimationLoop);
            }
            else
            {
                _internalFrame -= _direction;
                Stop(AnimationEventTrigger.AnimationCompleted);
                return false;
            }
        }

        CurrentFrameTimeRemaining = _definition.Frames[_internalFrame].Duration;
        OnAnimationEvent?.Invoke(this, AnimationEventTrigger.FrameBegin);
        return true;
    }

    public void Dispose()
        => GC.SuppressFinalize(this);
}
*/