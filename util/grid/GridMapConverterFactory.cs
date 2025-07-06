using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public class GridMapConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }
            
            if(typeToConvert.GetGenericTypeDefinition() != typeof(GridMap<>))
            {
                return false;
            }

            return true;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var type = typeToConvert.GetGenericArguments()[0];
            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(GridMapConverterInner<>).MakeGenericType(new Type[] { type }),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, null
            );
            return converter;
        }
    }

    public class GridMapConverterInner<T> : JsonConverter<GridMap<T>>
    {
        private readonly Type _type = typeof(T);

        public override GridMap<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            reader.Read();
            var dimensions = JsonSerializer.Deserialize<Vector3Int>(ref reader, options);
            reader.Read();
            reader.Read();
            GridMap<T> gridMap = new GridMap<T>(dimensions, () => default);
            var position = new Vector3Int();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if(reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Expected start of object for grid map value.");

                T value = default;
                try
                {
                    value = JsonSerializer.Deserialize<T>(ref reader, options);
                }
                catch (Exception e)
                {
                    Console.WriteLine(reader.TokenType);
                    throw;
                }

                gridMap[position] = value;
                position.x++;
                if (position.x >= gridMap.dimensions.x)
                {
                    position.x = 0;
                    position.y++;
                    if (position.y >= gridMap.dimensions.y)
                    {
                        position.y = 0;
                        position.z++;
                    }
                }
            }

            return gridMap;
        }

        public override void Write(Utf8JsonWriter writer, GridMap<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("dimensions");
            JsonSerializer.Serialize(writer, value.dimensions, options);

            writer.WritePropertyName("grids");
            writer.WriteStartArray();
            var position = new Vector3Int();
            for (int i = 0; i < value.dimensions.x * value.dimensions.y * value.dimensions.z; i++)
            {
                if (position.x >= value.dimensions.x)
                {
                    position.x = 0;
                    position.y++;
                    if (position.y >= value.dimensions.y)
                    {
                        position.y = 0;
                        position.z++;
                    }
                }

                JsonSerializer.Serialize(writer, value[position], options);
                position.x++;
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}