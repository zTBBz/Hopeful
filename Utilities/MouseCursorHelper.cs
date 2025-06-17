using Microsoft.Xna.Framework.Input;
using System;

namespace Hopeful.Utilities;

public static class MouseCursorHelper
{
    public static MouseCursorType DefaultMouseCursor { get; set; } = MouseCursorType.Arrow;
    public static MouseCursorType CurrentMouseCursor
    {
        get => field;
        set
        {
            if (field == value) return;

            MouseCursor mouseCursor = value switch
            {
                MouseCursorType.Arrow => MouseCursor.Arrow,
                MouseCursorType.IBeam => MouseCursor.IBeam,
                MouseCursorType.Wait => MouseCursor.Wait,
                MouseCursorType.Crosshair => MouseCursor.Crosshair,
                MouseCursorType.WaitArrow => MouseCursor.WaitArrow,
                MouseCursorType.SizeNWSE => MouseCursor.SizeNWSE,
                MouseCursorType.SizeNESW => MouseCursor.SizeNESW,
                MouseCursorType.SizeWE => MouseCursor.SizeWE,
                MouseCursorType.SizeNS => MouseCursor.SizeNS,
                MouseCursorType.SizeAll => MouseCursor.SizeAll,
                MouseCursorType.No => MouseCursor.No,
                MouseCursorType.Hand => MouseCursor.Hand,
                _ => throw new Exception($"Could not find mouse cursor {value}")
            };

            field = value;
            Mouse.SetCursor(mouseCursor);
        }
    }
}

public enum MouseCursorType
{
    Arrow,
    IBeam,
    Wait,
    Crosshair,
    WaitArrow,
    SizeNWSE,
    SizeNESW,
    SizeWE,
    SizeNS,
    SizeAll,
    No,
    Hand,
}
