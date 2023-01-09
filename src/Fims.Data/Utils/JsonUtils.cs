using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Nodes;

namespace Fims.Data.Utils
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerOptions _options =
            new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public static string SimpleSerialize(object obj)
        {
            var jsonString = JsonSerializer.Serialize(obj, _options);
            return jsonString;
        }

        public static string PrettySerialize(object obj)
        {
            var options = new JsonSerializerOptions(_options)
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(obj, options);
            return jsonString;
        }

        public static void SimpleWrite(object obj, string fileName)
        {
            var jsonString = JsonSerializer.Serialize(obj, _options);
            File.WriteAllText(fileName, jsonString);
        }

        public static void PrettyWrite(object obj, string fileName)
        {
            var options = new JsonSerializerOptions(_options)
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(obj, options);
            File.WriteAllText(fileName, jsonString);
        }
        public static void Utf8BytesWrite(object obj, string fileName)
        {
            var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(obj, _options);
            File.WriteAllBytes(fileName, utf8Bytes);
        }

        public static void StreamWrite(object obj, string fileName)
        {
            using var fileStream = File.Create(fileName);
            using var utf8JsonWriter = new Utf8JsonWriter(fileStream);
            JsonSerializer.Serialize(utf8JsonWriter, obj, _options);
        }
        public static async Task StreamWriteAsync(object obj, string fileName)
        {
            await using var fileStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(fileStream, obj, _options);
        }

        public static void WriteDynamicJsonObject(JsonObject jsonObj, string fileName)
        {
            using var fileStream = File.Create(fileName);
            using var utf8JsonWriter = new Utf8JsonWriter(fileStream);
            jsonObj.WriteTo(utf8JsonWriter);
        }

    }
}
