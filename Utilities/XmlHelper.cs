using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Hopeful.Utilities;

public static class XmlHelper
{
    public static void SerializeToFile<T>(T obj, string filePath)
    {
        try
        {
            var path = PathHelper.GetAbsolutePath(filePath);
            PathHelper.ValidatePath(path);

            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, obj);
        }
        catch (InvalidOperationException ex)
        {
            throw new XmlException("Serialization error", ex);
        }
    }

    public static T DeserializeFromFile<T>(string filePath)
    {
        try
        {
            var path = PathHelper.GetAbsolutePath(filePath);
            PathHelper.ValidatePath(path);

            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StreamReader(path);
            return (T)serializer.Deserialize(reader)!;
        }
        catch (InvalidOperationException ex)
        {
            throw new XmlException("Serialization error", ex);
        }
    }

    public static void CustomSerializeToFile<T>(T obj, string filePath) where T : IXmlSerializable, new()
    {
        try
        {
            var path = PathHelper.GetAbsolutePath(filePath);
            PathHelper.ValidatePath(path);

            /*using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            using var writer = XmlWriter.Create(stream);
            writer.WriteStartDocument();
            obj.WriteXml(writer);
            writer.WriteEndDocument();*/

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                NewLineChars = "\n",
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Auto,
                Async = false
            };

            using var stream = new FileStream(path, FileMode.Create);
            using var writer = XmlWriter.Create(stream, settings);

            obj.WriteXml(writer); // Не вызываем WriteStartDocument/WriteEndDocument
            writer.Flush();
        }
        catch (InvalidOperationException ex)
        {
            throw new XmlException("Serialization error", ex);
        }
    }

    public static async Task CustomSerializeToFileAsync<T>(T obj, string filePath) where T : IXmlSerializable, new()
    {
        try
        {
            var path = PathHelper.GetAbsolutePath(filePath);
            PathHelper.ValidatePath(path);

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                NewLineChars = "\n",
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Auto,
                Async = true
            };

            await using var stream = new FileStream(path, FileMode.Create);
            await using var writer = XmlWriter.Create(stream, settings);

            obj.WriteXml(writer); // Не вызываем WriteStartDocument/WriteEndDocument
            await writer.FlushAsync();

            /*await writer.WriteStartDocumentAsync();
            obj.WriteXml(writer);
            await writer.WriteEndDocumentAsync();*/
        }
        catch (InvalidOperationException ex)
        {
            throw new XmlException("Serialization error", ex);
        }
    }

    public static T CustomDeserializeFromFile<T>(string filePath) where T : IXmlSerializable, new()
    {
        try
        {
            var path = PathHelper.GetAbsolutePath(filePath);
            PathHelper.ValidatePath(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = XmlReader.Create(stream);
            var obj = new T();
            obj.ReadXml(reader);
            return obj;
        }
        catch (InvalidOperationException ex)
        {
            throw new XmlException("Serialization error", ex);
        }
    }
}
