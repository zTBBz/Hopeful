using Hopeful.Injection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

public interface IInputListener
{
    Vector2 PreviousMousePosition { get; }
    Vector2 MousePosition { get; }
    bool IsLeftMouseButtonPressed { get; }
    bool IsLeftMouseButtonJustPressed { get; }
    bool IsLeftMouseButtonJustReleased { get; }
    bool IsRightMouseButtonPressed { get; }
    bool IsRightMouseButtonJustPressed { get; }
    bool IsRightMouseButtonJustReleased { get; }
    int ScrollWheelValue { get; }
    int ScrollWheelDelta { get; }
    bool HasScrolled { get; }
    bool IsActionPressed(GameAction action);
    bool IsActionJustPressed(GameAction action);
    bool IsActionJustReleased(GameAction action);
    bool IsKeyPressed(Keys key);
    bool IsKeyJustPressed(Keys key);
    bool IsKeyJustReleased(Keys key);
    bool IsKeyComboPressed(params Keys[] keys);
    bool IsKeyComboLastPressed(params Keys[] keys);
    bool IsModifierComboPressed(Keys mainKey, bool ctrl = false, bool shift = false, bool alt = false);
    void Update();
}

[Service]
public class BaseInput : IInputListener
{
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

    public bool IsActionJustPressed(GameAction action)
    {
        var bind = InputBindings.Instance.GetBinding(action);

        return IsKeyJustPressed(bind.MainKey) &&
               (!bind.RequireCtrl || IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl)) &&
               (!bind.RequireShift || IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift)) &&
               (!bind.RequireAlt || IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt));
    }

    public bool IsActionJustReleased(GameAction action)
    {
        var bind = InputBindings.Instance.GetBinding(action);

        return IsKeyJustReleased(bind.MainKey) &&
               (!bind.RequireCtrl || IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl)) &&
               (!bind.RequireShift || IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift)) &&
               (!bind.RequireAlt || IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt));
    }

    public bool IsActionPressed(GameAction action)
    {
        var bind = InputBindings.Instance.GetBinding(action);

        return IsModifierComboPressed(
            bind.MainKey,
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
