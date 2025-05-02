using System;
using System.IO;
using System.Security;

namespace Hopeful.Utilities;

public static class PathHelper
{
    private static string? _gameDirectory;

    internal static string GameDirectory
    {
        get
        {
            if (_gameDirectory == null)
            {
                _gameDirectory = AppContext.BaseDirectory;
            }
            return _gameDirectory;
        }
    }

    /// <summary>
    ///   <para>Converts a relative path to an absolute path relative to the game root.</para>
    /// </summary>
    /// <param name="path">The relative path to convert.</param>
    /// <returns>The absolute path relative to the game root.</returns>
    /// <exception cref="ArgumentException">If the path is null or empty.</exception>
    public static string GetAbsolutePath(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));
        if (!Path.IsPathRooted(path)) path = Path.Combine(GameDirectory, path);
        return path;
    }

    public static string GetAssetAbsolutePath(string relativeAssetPath, string? modId = null)
    {
        string fullPath = Path.GetFullPath(Path.Combine("Assets", relativeAssetPath));
        return GetAbsolutePath(fullPath);
    }

    /// <summary>
    ///   <para>Converts an absolute path to a relative path from the game root.</para>
    /// </summary>
    /// <param name="absolutePath">The absolute path to convert.</param>
    /// <returns>The relative path from the game root.</returns>
    /// <exception cref="ArgumentException">If the path is null or empty.</exception>
    public static string GetRelativePath(string absolutePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(absolutePath, nameof(absolutePath));
        return Path.GetRelativePath(GameDirectory, absolutePath);
    }

    /// <summary>
    /// Validates if the specified path is within the game directory bounds
    /// and is safe to access.
    /// </summary>
    /// <param name="path">The path to validate (absolute or relative)</param>
    /// <returns>true if the path is safe, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when path is null or empty</exception>
    /// <remarks>
    /// This method performs several security checks:
    /// - Ensures the path is within game directory
    /// - Prevents access to hidden files
    /// - Protects against path traversal attacks
    /// </remarks>
    public static bool IsPathSafe(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

        try
        {
            string absolutePath = Path.GetFullPath(
                path.StartsWith(GameDirectory)
                    ? path
                    : GetAbsolutePath(path)
            );

            return absolutePath.StartsWith(GameDirectory, StringComparison.OrdinalIgnoreCase)
                   && !Path.GetFileName(absolutePath).StartsWith(".")
                   && !absolutePath.Contains("..");
        }
        catch (Exception ex) when (
            ex is SecurityException
            || ex is PathTooLongException
            || ex is NotSupportedException)
        {
            return false;
        }
    }

    public static string NormalizePath(string path)
        => path.Replace('\\', '/');

    /// <summary>
    /// Validates the path and throws an exception if it's not safe
    /// </summary>
    /// <param name="path">The path to validate</param>
    /// <exception cref="SecurityException">Thrown when the path is outside the project directory or otherwise unsafe</exception>
    /// <exception cref="ArgumentException">Thrown when path is null or empty</exception>
    /// <remarks>
    /// Use this method when you want to enforce path safety through exceptions
    /// rather than boolean checks
    /// </remarks>
    public static void ValidatePath(string path)
    {
        if (!IsPathSafe(path))
        {
            throw new SecurityException(
                $"Attempted to access file outside game directory: {path}");
        }
    }
}
