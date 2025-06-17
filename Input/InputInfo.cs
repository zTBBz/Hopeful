using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

[Service(typeof(IInputInfo))]
public sealed class InputInfo : IInputInfo
{
    [Inject]
    private readonly KeyBindStore _keyBinds = null!;

    private readonly InputCache _input;

    public InputInfo() => _input = GameCore.RootVault.InjectService<EntityStore>().GetUniqueEntity("Input").GetComponent<InputCache>();

    public int ScrollWheelValue => _input.CurrentMouseState.ScrollWheelValue;
    public int ScrollWheelDelta => _input.CurrentMouseState.ScrollWheelValue - _input.PreviousMouseState.ScrollWheelValue;
    public bool HasScrolled => ScrollWheelDelta != 0;

    public Vector2 PreviousMousePosition => new(_input.PreviousMouseState.X, _input.PreviousMouseState.Y);
    public Vector2 MousePosition => new(_input.CurrentMouseState.X, _input.CurrentMouseState.Y);

    public bool IsLeftMouseButtonPressed => _input.CurrentMouseState.LeftButton == ButtonState.Pressed;
    public bool IsLeftMouseButtonJustPressed => _input.CurrentMouseState.LeftButton == ButtonState.Pressed && _input.PreviousMouseState.LeftButton == ButtonState.Released;
    public bool IsLeftMouseButtonJustReleased => _input.CurrentMouseState.LeftButton == ButtonState.Released && _input.PreviousMouseState.LeftButton == ButtonState.Pressed;

    public bool IsRightMouseButtonPressed => _input.CurrentMouseState.RightButton == ButtonState.Pressed;
    public bool IsRightMouseButtonJustPressed => _input.CurrentMouseState.RightButton == ButtonState.Pressed && _input.PreviousMouseState.RightButton == ButtonState.Released;
    public bool IsRightMouseButtonJustReleased => _input.CurrentMouseState.RightButton == ButtonState.Released && _input.PreviousMouseState.RightButton == ButtonState.Pressed;

    public Vector2 GetCameraMousePosition(Matrix cameraMatrix)
        => Vector2.Transform(new Vector2(MousePosition.X, MousePosition.Y), Matrix.Invert(cameraMatrix));

    public Vector2 GetCameraPreviousMousePosition(Matrix cameraMatrix)
        => Vector2.Transform(new Vector2(PreviousMousePosition.X, PreviousMousePosition.Y), Matrix.Invert(cameraMatrix));

    public bool IsKeyBindJustPressed(string keyBind)
    {
        var bind = _keyBinds.TryGetKeyBind(keyBind);
        if (IsKeyJustPressed(bind.Key) && GetModifiersPressed() == bind.Modifiers) return true;
        return false;
    }

    public bool IsKeyBindJustReleased(string keyBind)
    {
        var bind = _keyBinds.TryGetKeyBind(keyBind);
        if (IsKeyJustReleased(bind.Key) && GetModifiersPressed() == bind.Modifiers) return true;
        return false;
    }

    public bool IsKeyBindPressed(string keyBind)
    {
        var bind = _keyBinds.TryGetKeyBind(keyBind);
        if (IsKeyPressed(bind.Key) && GetModifiersPressed() == bind.Modifiers) return true;
        return false;
    }

    public bool IsKeyComboLastPressed(params Keys[] keys)
    {
        if (keys == null || keys.Length == 0)
            return false;

        if (!IsKeyJustPressed(keys[^1]))
            return false;

        for (int i = 0; i < keys.Length - 1; i++)
            if (!IsKeyPressed(keys[i]))
                return false;
        return true;
    }

    public bool IsKeyComboPressed(params Keys[] keys)
    {
        if (keys == null || keys.Length == 0)
            return false;

        foreach (var key in keys)
            if (!IsKeyPressed(key))
                return false;
        return true;
    }

    public bool IsKeyJustPressed(Keys key)
        => _input.CurrentKeyboardState.IsKeyDown(key) && _input.PreviousKeyboardState.IsKeyUp(key);

    public bool IsKeyJustReleased(Keys key)
        => _input.CurrentKeyboardState.IsKeyUp(key) && _input.PreviousKeyboardState.IsKeyDown(key);

    public bool IsKeyPressed(Keys key)
        => _input.CurrentKeyboardState.IsKeyDown(key);

    public KeyModifiers GetModifiersPressed()
    {
        KeyModifiers modifiers = 0;

        if (IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl))
            modifiers |= KeyModifiers.Ctrl;

        if (IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift))
            modifiers |= KeyModifiers.Shift;

        if (IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt))
            modifiers |= KeyModifiers.Alt;

        return modifiers;
    }

    public Keys[] GetCurrentPressedKeys()
        => _input.CurrentKeyboardState.GetPressedKeys();

    public Keys[] GetPreviousPressedKeys()
        => _input.PreviousKeyboardState.GetPressedKeys();
}
