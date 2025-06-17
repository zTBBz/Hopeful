using Microsoft.Xna.Framework.Graphics;

namespace Hopeful.UI;

public readonly struct Unit(float value, UnitType unitType)
{
    public float Value { get; } = value;
    public UnitType UnitType { get; } = unitType;

    /// <summary>
    /// Represents an absolute value measured in pixels.
    /// </summary>
    public static Unit Pixels(float value) => new(value, UnitType.Pixels);

    /// <summary>
    /// Represents a percentage value relative to the parent component's dimension.
    /// </summary>
    /// <remarks>
    /// If no parent component, the percentage is calculated relative to the window size.
    /// </remarks>
    public static Unit Percentage(float value) => new(value, UnitType.Percentage);

    /// <summary>
    /// Represents a value relative to the viewport width.
    /// </summary>
    /// <remarks>
    /// The value is calculated as a percentage of the window's width, regardless of
    /// the parent component's dimensions. This is similar to the CSS vw unit.
    /// </remarks>
    public static Unit ViewWidth(float value) => new(value, UnitType.ViewportWidth);

    /// <summary>
    /// Represents a value relative to the viewport height.
    /// </summary>
    /// <remarks>
    /// The value is calculated as a percentage of the window's height, regardless of
    /// the parent component's dimensions. This is similar to the CSS vh unit.
    /// </remarks>
    public static Unit ViewHeight(float value) => new(value, UnitType.ViewportHeight);

    public float ToPixels(float parentSize, Viewport viewport)
    {
        return UnitType switch
        {
            UnitType.Pixels => Value,
            UnitType.Percentage => parentSize * (Value / 100f),
            UnitType.ViewportWidth => viewport.Width * (Value / 100f),
            UnitType.ViewportHeight => viewport.Height * (Value / 100f),
            _ => Value
        };
    }
}
