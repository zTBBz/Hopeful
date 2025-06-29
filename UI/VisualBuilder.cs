using Friflo.Engine.ECS;
using Hopeful.Input;
using Hopeful.Input.Events;
using Hopeful.Render;
using Hopeful.Render.Texts;
using Hopeful.Render.Views;
using Hopeful.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Hopeful.UI;

public sealed class VisualBuilder(in Entity entity)
{
    private readonly Entity _entity = entity;
    private Visual _visual = new();
    private Sprite _sprite = new();

    public static VisualBuilder Setup(in Entity entity)
    {
        entity.Enabled = false;
        entity.AddTag<VisualDirtyTag>();
        return new(entity);
    }

    public void Save()
    {
        _entity.AddComponent(_visual);
        _entity.AddComponent(_sprite);
        _entity.Enabled = true;
    }

    public VisualBuilder WithSprite(string spriteName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(spriteName);
        _sprite.SpriteName = spriteName;
        return this;
    }

    public VisualBuilder WithSize(Vector2 size)
    {
        _visual.Size = size;
        return this;
    }

    public VisualBuilder AsViewed(CameraMask mask)
    {
        _entity.AddComponent<View>(new() { CameraMask = (int)mask });
        return this;
    }

    public VisualBuilder WithText(string text, string? fontName = null, int fontSize = 14, Vector2? position = null, Color? color = null, float rotation = 0f)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(text);
        
        var textComponent = new Text(text);
        if (fontName != null) textComponent.FontName = fontName;
        textComponent.FontSize = fontSize;
        if (position != null) textComponent.Position = (Vector2)position;
        textComponent.Rotation = rotation;
        if (color != null) textComponent.Color = (Color)color;
        _entity.AddComponent(textComponent);
        return this;
    }

    public VisualBuilder AsInputListener()
    {
        _entity.AddTag<InputListenerTag>();
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

    public VisualBuilder OnMouseClick(Action<MouseEvent, VisualEvent> action)
    {
        AsInputListener();
        return OnEvent(action);
    }

    public VisualBuilder OnKey(Action<KeyEvent, VisualEvent> action)
    {
        AsInputListener();
        return OnEvent(action);
    }

    public VisualBuilder OnKeyBind(Action<KeyBindEvent, VisualEvent> action)
    {
        AsInputListener();
        return OnEvent(action);
    }

    public VisualBuilder OnEvent<TEvent>(Action<TEvent, VisualEvent> action) where TEvent : struct
    {
        _entity.AddSignalHandler<TEvent>(s => action.Invoke(s.Event, new(s.Entity, _entity, _visual, _sprite)));
        return this;
    }
}
