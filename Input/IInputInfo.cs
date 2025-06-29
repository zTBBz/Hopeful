using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

public interface IInputInfo
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
    bool IsKeyBindPressed(string keyBind);
    bool IsKeyBindJustPressed(string keyBind);
    bool IsKeyBindJustReleased(string keyBind);
    bool IsKeyPressed(Keys key);
    bool IsKeyJustPressed(Keys key);
    bool IsKeyJustReleased(Keys key);
    bool IsKeyComboPressed(params Keys[] keys);
    bool IsKeyComboLastPressed(params Keys[] keys);
    KeyModifiers GetModifiersPressed();
    Keys[] GetCurrentPressedKeys();
    Keys[] GetPreviousPressedKeys();

    void UpdateState(MouseState currentMouseState, KeyboardState currentKeyboardState);
}
