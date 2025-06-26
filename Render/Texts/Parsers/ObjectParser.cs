using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hopeful.Render.Texts;

public static class ObjectParser // TODO: Move to Utilities
{
    public static float ParseFloat(string param)
    {
        ArgumentException.ThrowIfNullOrEmpty(param);

        return float.TryParse(param, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result) ? result : float.NaN;
    }

    public static bool ParseIntBool(string param)
    {
        ArgumentException.ThrowIfNullOrEmpty(param);

        if (!int.TryParse(param, out var result)) throw new KeyNotFoundException($"Param {param} is not int (0 or 1)!");

        return result == 1;
    }

    public static bool ParseBool(string param)
    {
        ArgumentException.ThrowIfNullOrEmpty(param);

        if (!bool.TryParse(param, out bool result)) throw new KeyNotFoundException($"Param {param} is not bool (true or false)!");

        return result;
    }

    public static float ParseAngle(string param) => Math.Clamp(ParseFloat(param), -360, 360);

    public static float ParseChance(string param) => Math.Clamp(ParseFloat(param), 0f, 1f);
}
