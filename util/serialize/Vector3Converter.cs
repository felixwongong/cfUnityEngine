using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public class Vector3Converter: JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "x")
                throw new JsonException("Expected property name 'x'");

            reader.Read();
            float x = reader.GetSingle();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "y")
                throw new JsonException("Expected property name 'y'");

            reader.Read();
            float y = reader.GetSingle();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "z")
                throw new JsonException("Expected property name 'z'");

            reader.Read();
            float z = reader.GetSingle();

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException("Expected end of object");

            return new Vector3(x, y, z);
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.x);
            writer.WriteNumber("y", value.y);
            writer.WriteNumber("z", value.z);
            writer.WriteEndObject();
        }
    }
}