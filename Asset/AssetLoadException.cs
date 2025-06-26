using System;

namespace Hopeful.Asset;

/// <summary>
///   <para>Exception thrown when an asset fails to load.</para>
/// </summary>
public sealed class AssetLoadException : Exception
{
    /// <summary>
    ///   <para>Gets the path to the asset that failed to load.</para>
    /// </summary>
    public string AssetPath { get; }

    /// <summary>
    ///   <para>Initializes a new instance of the AssetLoadException class.</para>
    /// </summary>
    /// <param name="assetPath">The path to the asset that failed to load.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public AssetLoadException(string assetPath, string message) : base($"Failed to load asset '{assetPath}': {message}")
        => AssetPath = assetPath;

    /// <summary>
    ///   <para>Initializes a new instance of the AssetLoadException class.</para>
    /// </summary>
    /// <param name="assetPath">The path to the asset that failed to load.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AssetLoadException(string assetPath, string message, Exception innerException) : base($"Failed to load asset '{assetPath}': {message}", innerException)
        => AssetPath = assetPath;
}
