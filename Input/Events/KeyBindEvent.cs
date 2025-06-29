namespace Hopeful.Input.Events;

public readonly struct KeyBindEvent(string keyBind, bool isJustPressed, bool isJustReleased)
{
    public readonly string KeyBind = keyBind;

    public bool IsPressed => true;
    public bool IsJustPressed => isJustPressed;
    public bool IsJustReleased => isJustReleased;
}
