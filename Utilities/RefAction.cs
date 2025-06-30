namespace Hopeful.Utilities;

/// <summary>
/// A delegate that defines a method signature for actions that accept a parameter by reference.
/// </summary>
/// <typeparam name="T">The type of the parameter passed by reference.</typeparam>
/// <param name="item">The item to be passed by reference, allowing the method to modify it directly.</param>
public delegate void RefAction<T>(ref T item);
