using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Hopeful.Input;

public readonly struct KeyBind(Keys key, bool ctrl = false, bool shift = false, bool alt = false)
{
    public readonly Keys Key = key;
    public readonly bool RequireCtrl = ctrl;
    public readonly bool RequireShift = shift;
    public readonly bool RequireAlt = alt;

    public override string ToString()
    {
        var parts = new List<string>();

        if (RequireCtrl) parts.Add("Ctrl");
        if (RequireShift) parts.Add("Shift");
        if (RequireAlt) parts.Add("Alt");

        parts.Add(Key.ToString());

        return string.Join("+", parts);
    }
}
