using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

[Service(typeof(IInputListener))]
public class BaseInput : IInputListener
{
    [Inject]
    private readonly KeyBindStore _store = null!;

    private MouseState _currentMouseState;
    private MouseState _previousMouseState;
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    public Vector2 PreviousMousePosition => new(_previousMouseState.X, _previousMouseState.Y);
    public Vector2 MousePosition => new(_currentMouseState.X, _currentMouseState.Y);

    public bool IsLeftMouseButtonPressed => _currentMouseState.LeftButton == ButtonState.Pressed;
    public bool IsLeftMouseButtonJustPressed => _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
    public bool IsLeftMouseButtonJustReleased => _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

    public bool IsRightMouseButtonPressed => _currentMouseState.RightButton == ButtonState.Pressed;
    public bool IsRightMouseButtonJustPressed => _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
    public bool IsRightMouseButtonJustReleased => _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed;

    public int ScrollWheelValue => _currentMouseState.ScrollWheelValue;
    public int ScrollWheelDelta => _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
    public bool HasScrolled => ScrollWheelDelta != 0;

    public Vector2 GetCameraMousePosition(Matrix cameraMatrix)
        => Vector2.Transform(new Vector2(MousePosition.X, MousePosition.Y), Matrix.Invert(cameraMatrix));

    public Vector2 GetCameraPreviousMousePosition(Matrix cameraMatrix)
        => Vector2.Transform(new Vector2(PreviousMousePosition.X, PreviousMousePosition.Y), Matrix.Invert(cameraMatrix));

    public bool IsKeyBindJustPressed(string keyBind)
    {
        var bind = _store.TryGetKeyBind(keyBind);

        return IsKeyJustPressed(bind.Key) &&
               (!bind.RequireCtrl || IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl)) &&
               (!bind.RequireShift || IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift)) &&
               (!bind.RequireAlt || IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt));
    }

    public bool IsKeyBindJustReleased(string keyBind)
    {
        var bind = _store.TryGetKeyBind(keyBind);

        return IsKeyJustReleased(bind.Key) &&
               (!bind.RequireCtrl || IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl)) &&
               (!bind.RequireShift || IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift)) &&
               (!bind.RequireAlt || IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt));
    }

    public bool IsKeyBindPressed(string keyBind)
    {
        var bind = _store.TryGetKeyBind(keyBind);

        return IsModifierComboPressed(
            bind.Key,
            bind.RequireCtrl,
            bind.RequireShift,
            bind.RequireAlt
        );
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

    public bool IsModifierComboPressed(Keys mainKey, bool ctrl = false, bool shift = false, bool alt = false)
    {
        if (!IsKeyPressed(mainKey))
            return false;

        if (ctrl && !IsKeyPressed(Keys.LeftControl) && !IsKeyPressed(Keys.RightControl))
            return false;

        if (shift && !IsKeyPressed(Keys.LeftShift) && !IsKeyPressed(Keys.RightShift))
            return false;

        if (alt && !IsKeyPressed(Keys.LeftAlt) && !IsKeyPressed(Keys.RightAlt))
            return false;

        return true;
    }

    public void Update()
    {
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();

        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }
}
