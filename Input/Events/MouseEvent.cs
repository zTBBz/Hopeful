using Microsoft.Xna.Framework;

namespace Hopeful.Input.Events;

public readonly struct MouseEvent(bool mouseButton, bool isJustPressed, bool isJustReleased, Vector2 mousePosition)
{
    public readonly bool IsRightClick => !mouseButton;
    public readonly bool IsLeftClick => mouseButton;

    public static bool IsMouseButtonPressed => true;
    public readonly bool IsMouseButtonJustPressed => isJustPressed;
    public readonly bool IsMouseButtonJustReleased => isJustReleased;

    public readonly Vector2 MousePosition => mousePosition;
}
