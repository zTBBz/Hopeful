using Chasm.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hopeful.Utilities;

/// <summary>
///   <para>Provides a set of static methods for running code on the main thread.</para>
/// </summary>
public static class MainThread
{
    private static readonly ConcurrentQueue<Action> _queue = [];

    private static readonly List<Action> _actionsList = [];
    private static readonly List<Exception> _exceptionsList = [];

    /// <summary>
    ///   <para>Enqueues the specified <paramref name="action"/> to run on the main thread.</para>
    /// </summary>
    /// <param name="action">The action to run on the main thread.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public static void Enqueue(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        // TODO: log a warning if _activeRunner is null or disabled
        _queue.Enqueue(action);
    }

    /// <summary>
    ///   <para>Enqueues the specified <paramref name="action"/> to throw on the main thread.</para>
    /// </summary>
    /// <param name="exception">The exception to throw on the main thread.</param>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/>.</exception>
    public static void EnqueueException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        _exceptionsList.Add(exception);
    }

    /// <summary>
    ///   <para>Enqueues the specified <paramref name="action"/> to run on the main thread and returns a <see cref="Task"/> that represents it.</para>
    /// </summary>
    /// <param name="action">The action to run on the main thread.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    /// <returns>A task representing the specified <paramref name="action"/> queued to execute on the main thread.</returns>
    [MustUseReturnValue]
    public static Task RunAsync(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return RunAsync<object?>(() => { action(); return null; });
    }

    /// <summary>
    ///   <para>Enqueues the specified <paramref name="function"/> to run on the main thread and returns a <see cref="Task{T}"/> that represents it.</para>
    /// </summary>
    /// <param name="function">The function to run on the main thread.</param>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is <see langword="null"/>.</exception>
    /// <returns>A task representing the specified <paramref name="function"/> queued to execute on the main thread.</returns>
    [MustUseReturnValue]
    public static Task<T> RunAsync<T>(Func<T> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        TaskCompletionSource<T> tcs = new();

        Enqueue(() =>
        {
            try { tcs.SetResult(function()); }
            catch (Exception ex) { tcs.SetException(ex); }
        });

        return tcs.Task;
    }

    internal static void Update()
    {
        while (_queue.TryDequeue(out var action))
            _actionsList.Add(action);

        for (var i = 0; i < _actionsList.Count; i++)
            if (Util.Catch(_actionsList[i]) is { } exception)
                _exceptionsList.Add(exception);

        _actionsList.Clear();

        if (_exceptionsList.Count > 0)
        {
            var exception = _exceptionsList.Count == 1 ? _exceptionsList[0] : new AggregateException(_exceptionsList);
            _exceptionsList.Clear();
            throw exception;
        }
    }
}
