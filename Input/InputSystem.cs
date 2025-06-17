using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Hopeful.Input.Events;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

public sealed class InputSystem(Vault vault) : InjectBaseSystem(vault)
{
    [Inject]
    private readonly InputInfo _info = null!;

    [Inject]
    private readonly KeyBindStore _keyBinds = null!;

    protected override void OnUpdateGroup()
    {
        var entity = Store.GetUniqueEntity("InputCache");
        var input = entity.GetComponent<InputCache>();

        UpdateState(ref input);

        TryEmitMouseClickEvent(entity);
        TryEmitKeyboardEvents(entity);
    }

    private void TryEmitMouseClickEvent(Entity entity)
    {
        if (_info.IsLeftMouseButtonPressed || _info.IsRightMouseButtonPressed)
        {
            bool button = _info.IsLeftMouseButtonPressed && !_info.IsRightMouseButtonPressed; // left = true, right = false
            bool isJustPressed = button ? _info.IsLeftMouseButtonJustPressed : _info.IsRightMouseButtonJustPressed;
            bool isJustReleased = button ? _info.IsLeftMouseButtonJustReleased : _info.IsRightMouseButtonJustReleased;

            entity.EmitSignal<MouseEvent>(new(button, isJustPressed, isJustReleased, _info.MousePosition));
        }
    }

    private void TryEmitKeyboardEvents(Entity entity)
    {
        TryEmitKeyEvent(entity);
        TryEmitKeyBindEvent(entity);
    }

    private void TryEmitKeyEvent(Entity entity)
    {
        var modifiers = _info.GetModifiersPressed();
        var keys = _info.GetPreviousPressedKeys();
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

    private static void UpdateState(ref InputCache input)
    {
        input.PreviousMouseState = input.CurrentMouseState;
        input.CurrentMouseState = Mouse.GetState();

        input.PreviousKeyboardState = input.CurrentKeyboardState;
        input.CurrentKeyboardState = Keyboard.GetState();
    }
}
