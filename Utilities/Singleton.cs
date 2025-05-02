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

/// <summary>
/// Class for implementing the singleton pattern with parameters.
/// </summary>
/// <typeparam name="T">Type of the singleton class.</typeparam>
public abstract class InitSingleton<T> where T : class
{
    private static T? _instance;
    private static readonly object _lock = new();

    protected InitSingleton() { }

    protected static void Initialize(T instance)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                _instance ??= instance;
            }
        }
        else
            throw new InvalidOperationException($"Singleton {typeof(T)} already initialized.");
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
                throw new InvalidOperationException($"Singleton {typeof(T)} not initialized. Call Initialize().");
            return _instance;
        }
    }
}