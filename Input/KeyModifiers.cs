using System;

namespace Hopeful.Input;

[Flags]
public enum KeyModifiers
{
    Ctrl = 1 << 0,
    Shift = 1 << 1,
    Alt = 1 << 2,
}
