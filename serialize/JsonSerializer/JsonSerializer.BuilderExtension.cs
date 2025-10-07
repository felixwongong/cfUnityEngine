using cfEngine.Serialize;
using cfUnityEngine.Util;

namespace cfUnityEngine.Serialize
{
    public static class JsonSerializerBuilderExtension
    {
        public static JsonSerializer.Builder SetDefault(this JsonSerializer.Builder builder)
        {
            return builder
                .WithOptions(new JsonSerializer.SerializerOptions()
                {
                    IncludeFields = true,
                    IncludeReadOnlyProperties = true
                })
                .AddConverter(new Vector3Converter())
                .AddConverter(new Vector3IntConverter())
                .AddConverter(new GridMapConverterFactory());
        }
    }
}