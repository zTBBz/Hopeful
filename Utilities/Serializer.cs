using Newtonsoft.Json;
using System;
using System.IO;

namespace Hopeful.Utilities;

public static class Serializer
{
    private static JsonSerializerSettings _settings;

    static Serializer()
    {
        _settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
    }

    /// <summary>
    /// The settings to use during <see cref="Save{T}(T, string, JsonSerializerSettings)"/> and <see cref="Load{T}(string, JsonSerializerSettings)"/>.
    /// </summary>
    public static JsonSerializerSettings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
        }
    }

    /// <summary>
    /// A shortcut for serialization that uses <see cref="JsonConvert.SerializeObject(object?, Type?, JsonSerializerSettings?)"/> with the <see cref="Settings"/> property defined by this class.
    /// </summary>
    /// <typeparam name="T">The object type to serialize.</typeparam>
    /// <param name="instance">The object to serialize.</param>
    /// <returns>A json encoded string.</returns>
    public static string Serialize<T>(T instance) =>
        JsonConvert.SerializeObject(instance, Formatting.Indented, _settings);

    /// <summary>
    /// A shortcut for serialization that uses <see cref="JsonConvert.DeserializeObject(string, Type?, JsonSerializerSettings?)"/> with the <see cref="Settings"/> property defined by this class.
    /// </summary>
    /// <param name="json">The json string to create an object from.</param>
    /// <returns>An object created from the <paramref name="json"/> parameter.</returns>
    public static T Deserialize<T>(string json) =>
        (T)JsonConvert.DeserializeObject(json, typeof(T), _settings)!;


    /// <summary>
    /// Serializes the <paramref name="instance"/> to the specified file.
    /// </summary>
    /// <typeparam name="T">Type of object to serialize</typeparam>
    /// <param name="instance">The object to serialize.</param>
    /// <param name="absolutePath">The relative path to save the object to Assets.</param>
    /// <param name="settings">Optional settings to use during serialization. If <see langword="null"/>, uses the <see cref="Settings"/> property.</param>
    public static void Save<T>(T instance, string relativePath, JsonSerializerSettings? settings = null)
    {
        var path = PathHelper.GetAssetAbsolutePath(relativePath);
        PathHelper.ValidatePath(path);

        using (Stream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (var sw = new StreamWriter(stream))
                sw.Write(JsonConvert.SerializeObject(instance, Formatting.Indented, settings ?? _settings));
        }
    }

    /// <summary>
    /// Deserializes a new instance of <typeparamref name="T"/> from the specified file.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <param name="absolutePath">The relative path to load from Assets.</param>
    /// <param name="settings">Optional settings to use during deserialization. If <see langword="null"/>, uses the <see cref="Settings"/> property.</param>
    /// <returns>A new object instance.</returns>
    public static T Load<T>(string relativePath, JsonSerializerSettings? settings = null)
    {
        var path = PathHelper.GetAssetAbsolutePath(relativePath);
        PathHelper.ValidatePath(path);

        using (Stream fileObject = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            using (var sr = new StreamReader(fileObject))
            {
                return (T)JsonConvert.DeserializeObject(sr.ReadToEnd(), typeof(T), settings ?? _settings)!;
            }
        }
    }
}
