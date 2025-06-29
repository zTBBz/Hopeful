using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Hopeful.Input.Events;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

public sealed class InputSystem(Vault vault) : InjectBaseSystem(vault) // TODO: Maybe use TinyMessanger instead ECS.Friflo signal, because they dependent entity to emit event, I need anonymous events
{
    [Inject]
    private readonly IInputInfo _info = null!;

    [Inject]
    private readonly KeyBindStore _keyBinds = null!;

    protected override void OnUpdateGroup()
    {
        UpdateState();

        var query = Store.Query().AnyTags(Tags.Get<InputListenerTag>());

        foreach (var entity in query.Entities)
        {
            TryEmitMouseClickEvent(entity);
            TryEmitKeyboardEvents(entity);
        }
    }

    private void TryEmitMouseClickEvent(Entity entity)
    {
        if (!(_info.IsLeftMouseButtonPressed || _info.IsRightMouseButtonPressed)) return;

        var button = _info.IsLeftMouseButtonPressed && !_info.IsRightMouseButtonPressed; // left = true, right = false
        var isJustPressed = button ? _info.IsLeftMouseButtonJustPressed : _info.IsRightMouseButtonJustPressed;
        var isJustReleased = button ? _info.IsLeftMouseButtonJustReleased : _info.IsRightMouseButtonJustReleased;

        entity.EmitSignal<MouseEvent>(new(button, isJustPressed, isJustReleased, _info.MousePosition));
    }

    private void TryEmitKeyboardEvents(Entity entity)
    {
        TryEmitKeyEvent(entity);
        TryEmitKeyBindEvent(entity);
    }

    private void TryEmitKeyEvent(Entity entity)
    {
        var modifiers = _info.GetModifiersPressed();
        var keys = _info.GetCurrentPressedKeys(); // why previous?

        foreach (var key in keys)
            entity.EmitSignal<KeyEvent>(new(key, _info.IsKeyJustPressed(key), _info.IsKeyJustReleased(key), modifiers));
    }

    private void TryEmitKeyBindEvent(Entity entity)
    {
        var keyBinds = _keyBinds.GetKeyBinds();
        foreach (var pair in keyBinds)
        {
            var pressed = _info.IsKeyBindJustPressed(pair.Key);
            var released = _info.IsKeyBindJustReleased(pair.Key);
            if (pressed || released)
                entity.EmitSignal<KeyBindEvent>(new(pair.Key, pressed, released));
        }
    }

    private void UpdateState()
        => _info.UpdateState(Mouse.GetState(), Keyboard.GetState());
}
