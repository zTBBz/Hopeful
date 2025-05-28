using System;

namespace Hopeful.Utilities;

/// <summary>
/// Base class for implementing the singleton pattern.
/// </summary>
/// <typeparam name="T">Type of the singleton class.</typeparam>
public abstract class Singleton<T> where T : class, new()
{
    private static readonly Lazy<T> _instance = new(() => new());
    public static T Instance { get => _instance.Value; }
}
