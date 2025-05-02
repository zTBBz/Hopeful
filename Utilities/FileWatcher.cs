using System;
using System.IO;

namespace Hopeful.Utilities;

/// <summary>
///   <para>Monitors a directory for file changes and raises events when files are modified.</para>
/// </summary>
public class FileWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly Action<string> _onFileChanged;
    private readonly object _lock = new();
    private bool _isEnabled;

    /// <summary>
    ///   <para>Gets or sets whether the file watcher is enabled.</para>
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            lock (_lock)
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                _watcher.EnableRaisingEvents = value;
            }
        }
    }

    /// <summary>
    ///   <para>Initializes a new instance of the FileWatcher class.</para>
    /// </summary>
    /// <param name="path">The directory to monitor for changes.</param>
    /// <param name="onFileChanged">The callback to invoke when a file is changed.</param>
    public FileWatcher(string path, Action<string> onFileChanged)
    {
        _onFileChanged = onFileChanged;
        _watcher = new FileSystemWatcher
        {
            Path = path,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size,
            Filter = "*.*",
            IncludeSubdirectories = true
        };

        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Renamed += OnFileRenamed;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (!IsEnabled) return;
        _onFileChanged(e.FullPath);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        if (!IsEnabled) return;
        _onFileChanged(e.FullPath);
    }

    /// <summary>
    ///   <para>Releases all resources used by the FileWatcher.</para>
    /// </summary>
    public void Dispose()
    {
        _watcher.Changed -= OnFileChanged;
        _watcher.Created -= OnFileChanged;
        _watcher.Renamed -= OnFileRenamed;
        _watcher.Dispose();
    }
}
