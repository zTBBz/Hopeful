using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Hopeful.Input;

public readonly struct KeyBind(Keys key, KeyModifiers modifiers = 0)
{
    public readonly Keys Key = key;
    public readonly KeyModifiers Modifiers = modifiers;

    public override string ToString()
    {
        var parts = new List<string>();

        if (Modifiers.HasFlag(KeyModifiers.Ctrl)) parts.Add("Ctrl");
        if (Modifiers.HasFlag(KeyModifiers.Shift)) parts.Add("Shift");
        if (Modifiers.HasFlag(KeyModifiers.Alt)) parts.Add("Alt");

        parts.Add(Key.ToString());

        return string.Join("+", parts);
    }
}
