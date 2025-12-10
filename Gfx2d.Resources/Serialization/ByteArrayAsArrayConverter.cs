using Newtonsoft.Json;

namespace Gfx2d.Resources.Serialization
{
    /// <summary>
    /// Converts a byte array to a JSON array.
    /// By default, Newtonsoft.Json converts a byte array to a base-64 encoded string. This custom converter overrides that default functionality.    /// 
    /// </summary>
    /// <seealso cref="https://www.newtonsoft.com/json/help/html/CustomJsonConverterGeneric.htm"/>
    public class ByteArrayAsArrayConverter : JsonConverter<byte[]>
    {
        public override void WriteJson(JsonWriter writer, byte[]? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // 1. Start writing a JSON array
            writer.WriteStartArray();

            // 2. Write each byte as a number
            foreach (byte b in value)
            {
                writer.WriteValue(b);
            }

            // 3. End the JSON array
            writer.WriteEndArray();
        }

        public override byte[]? ReadJson(JsonReader reader, Type objectType, byte[]? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonSerializationException($"Unexpected token type {reader.TokenType} when reading byte array.");
            }

            List<byte> byteList = new();

            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                if (reader.TokenType == JsonToken.Integer)
                {
                    byteList.Add(Convert.ToByte(reader.Value));
                }
                else
                {
                    throw new JsonSerializationException($"Unexpected token type {reader.TokenType} inside byte array.");
                }
            }

            return byteList.ToArray();
        }
    }
}