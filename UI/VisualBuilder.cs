using Friflo.Engine.ECS;
using Hopeful.Input;
using Hopeful.Input.Events;
using Hopeful.Render;
using Hopeful.Render.Texts;
using Hopeful.Render.View;
using Hopeful.Utilities;
using Microsoft.Xna.Framework.Input;
using System;

namespace Hopeful.UI;

public sealed class VisualBuilder(Entity entity, Visual visual, Sprite sprite)
{
    private readonly Entity _entity = entity;
    private readonly Visual _visual = visual;
    private readonly Sprite _sprite = sprite;

    public static VisualBuilder Setup(Entity entity)
    {
        entity.Enabled = false;
        return new(entity, entity.GetOrAddComponent<Visual>(), entity.GetOrAddComponent<Sprite>());
    }
        
    public void Save() => entity.Enabled = true;

    // Make more user-friendly mask settings
    public VisualBuilder AsViewed(int cameraMask = 0b_0001)
    {
        _entity.AddComponent<Viewed>(new() { CameraMask = cameraMask });
        return this;
    }

    public VisualBuilder WithText(string text, string? fontName = null)
    {
        _entity.AddComponent<Text>(new(text, fontName ?? "Default"));
        return this;
    }

    public VisualBuilder OnRightClick(Action<VisualEvent> action)
        => OnMouseClick((mouse, visual) =>
        {
            if (mouse.IsRightClick && mouse.MousePosition.IsVectorOverRectangle(visual.Visual.Bounds))
                action.Invoke(visual);
        });

    public VisualBuilder OnLeftClick(Action<VisualEvent> action)
        => OnMouseClick((mouse, visual) =>
        {
            if (mouse.IsLeftClick && mouse.MousePosition.IsVectorOverRectangle(visual.Visual.Bounds))
                action.Invoke(visual);
        });

    public VisualBuilder OnKeyPressed(Keys key, Action<VisualEvent> action, KeyModifiers modifiers = 0)
        => OnKey((keyEvent, visual) =>
        {
            if (keyEvent.Key == key && keyEvent.IsPressed && keyEvent.KeyModifiers == modifiers)
                action.Invoke(visual);
        });

    public VisualBuilder OnKeyJustPressed(Keys key, Action<VisualEvent> action, KeyModifiers modifiers = 0)
        => OnKey((keyEvent, visual) =>
        {
            if (keyEvent.Key == key && keyEvent.IsJustPressed && keyEvent.KeyModifiers == modifiers)
                action.Invoke(visual);
        });

    public VisualBuilder OnKeyJustReleased(Keys key, Action<VisualEvent> action, KeyModifiers modifiers = 0)
        => OnKey((keyEvent, visual) =>
        {
            if (keyEvent.Key == key && keyEvent.IsJustReleased && keyEvent.KeyModifiers == modifiers)
                action.Invoke(visual);
        });

    public VisualBuilder OnKeyBindPressed(string keyBind, Action<VisualEvent> action)
        => OnKeyBind((bind, visual) =>
        {
            if (bind.KeyBind == keyBind && bind.IsPressed)
                action.Invoke(visual);
        });

    public VisualBuilder OnKeyBindJustPressed(string keyBind, Action<VisualEvent> action)
        => OnKeyBind((bind, visual) =>
        {
            if (bind.KeyBind == keyBind && bind.IsJustPressed)
                action.Invoke(visual);
        });

    public VisualBuilder OnKeyBindJustReleased(string keyBind, Action<VisualEvent> action)
        => OnKeyBind((bind, visual) =>
        {
            if (bind.KeyBind == keyBind && bind.IsJustReleased)
                action.Invoke(visual);
        });

    public VisualBuilder OnMouseClick(Action<MouseEvent, VisualEvent> action) => OnEvent(action);
    public VisualBuilder OnKey(Action<KeyEvent, VisualEvent> action) => OnEvent(action);
    public VisualBuilder OnKeyBind(Action<KeyBindEvent, VisualEvent> action) => OnEvent(action);

    public VisualBuilder OnEvent<TEvent>(Action<TEvent, VisualEvent> action) where TEvent : struct
    {
        _entity.AddSignalHandler<TEvent>((s) => action.Invoke(s.Event, new(s.Entity, _entity, _visual, _sprite)));
        return this;
    }
}
