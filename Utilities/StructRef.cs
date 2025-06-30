namespace Hopeful.Utilities;

/// <summary>
/// A generic wrapper for struct that allows indirect modification and reference semantics.
/// </summary>
/// <typeparam name="T">The value type to wrap. Must be a struct.</typeparam>
/// <param name="value">The initial value of the wrapped struct.</param>
public class StructRef<T>(T value) where T : struct
{
    /// <summary>
    /// The wrapped value of the struct.
    /// </summary>
    /// <remarks>
    /// This field provides direct access to the underlying value. 
    /// Modifications to this field will affect the wrapped struct.
    /// </remarks>
    public T Value = value;
}