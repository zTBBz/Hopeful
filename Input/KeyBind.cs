using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Hopeful.Input;

public class KeyBind
{
    public Keys MainKey { get; set; }
    public bool RequireCtrl { get; set; }
    public bool RequireShift { get; set; }
    public bool RequireAlt { get; set; }

    public KeyBind(Keys key, bool ctrl = false, bool shift = false, bool alt = false)
    {
        MainKey = key;
        RequireCtrl = ctrl;
        RequireShift = shift;
        RequireAlt = alt;
    }

    public override string ToString()
    {
        var parts = new List<string>();

        if (RequireCtrl) parts.Add("Ctrl");
        if (RequireShift) parts.Add("Shift");
        if (RequireAlt) parts.Add("Alt");

        parts.Add(MainKey.ToString());

        return string.Join("+", parts);
    }
}