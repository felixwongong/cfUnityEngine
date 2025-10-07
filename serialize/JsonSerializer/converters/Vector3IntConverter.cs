using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace cfUnityEngine.Serialize
{
    public class Vector3IntConverter: JsonConverter<Vector3Int>
    {
        public override Vector3Int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "x")
                throw new JsonException("Expected property name 'x'");

            reader.Read();
            int x = reader.GetInt32();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "y")
                throw new JsonException("Expected property name 'y'");

            reader.Read();
            int y = reader.GetInt32();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "z")
                throw new JsonException("Expected property name 'z'");

            reader.Read();
            int z = reader.GetInt32();

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException("Expected end of object");

            return new Vector3Int(x, y, z);
        }

        public override void Write(Utf8JsonWriter writer, Vector3Int value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.x);
            writer.WriteNumber("y", value.y);
            writer.WriteNumber("z", value.z);
            writer.WriteEndObject();
        }
    }
}