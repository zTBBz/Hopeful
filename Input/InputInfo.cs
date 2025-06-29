using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

[Service(null, typeof(IInputInfo))]
public sealed class InputInfo : IInputInfo
{
    [Inject]
    private readonly KeyBindStore _keyBinds = null!;

    private MouseState _currentMouseState;
    private MouseState _previousMouseState;
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    public int ScrollWheelValue => _currentMouseState.ScrollWheelValue;
    public int ScrollWheelDelta => _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
    public bool HasScrolled => ScrollWheelDelta != 0;

    public Vector2 PreviousMousePosition => new(_previousMouseState.X, _previousMouseState.Y);
    public Vector2 MousePosition => new(_currentMouseState.X, _currentMouseState.Y);

    public bool IsLeftMouseButtonPressed => _currentMouseState.LeftButton == ButtonState.Pressed;
    public bool IsLeftMouseButtonJustPressed => _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
    public bool IsLeftMouseButtonJustReleased => _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

    public bool IsRightMouseButtonPressed => _currentMouseState.RightButton == ButtonState.Pressed;
    public bool IsRightMouseButtonJustPressed => _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
    public bool IsRightMouseButtonJustReleased => _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed;

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
        => _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);

    public bool IsKeyJustReleased(Keys key)
        => _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);

    public bool IsKeyPressed(Keys key)
        => _currentKeyboardState.IsKeyDown(key);

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
        => _currentKeyboardState.GetPressedKeys();

    public Keys[] GetPreviousPressedKeys()
        => _previousKeyboardState.GetPressedKeys();

    public void UpdateState(MouseState currentMouseState, KeyboardState currentKeyboardState)
    {
        _previousMouseState = _currentMouseState;
        _currentMouseState = currentMouseState;

        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = currentKeyboardState;
    }
}
