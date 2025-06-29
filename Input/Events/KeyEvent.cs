using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input.Events;

public readonly struct KeyEvent(Keys key, bool isJustPressed, bool isJustReleased, KeyModifiers modifiers)
{
    public readonly Keys Key = key;

    public bool IsPressed => true;
    public bool IsJustPressed => isJustPressed;
    public bool IsJustReleased => isJustReleased;

    public readonly KeyModifiers KeyModifiers = modifiers;
}
