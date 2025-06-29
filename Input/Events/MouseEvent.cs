using Microsoft.Xna.Framework;

namespace Hopeful.Input.Events;

public readonly struct MouseEvent(bool mouseButton, bool isJustPressed, bool isJustReleased, Vector2 mousePosition)
{
    public bool IsRightClick => !mouseButton;
    public bool IsLeftClick => mouseButton;

    public bool IsMouseButtonPressed => true;
    public bool IsMouseButtonJustPressed => isJustPressed;
    public bool IsMouseButtonJustReleased => isJustReleased;

    public Vector2 MousePosition => mousePosition;
}
