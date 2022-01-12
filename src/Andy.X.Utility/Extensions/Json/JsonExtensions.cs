using System;
using System.Text.Json;

namespace Buildersoft.Andy.X.Utility.Extensions.Json
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(object), new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static string ToJsonAndEncrypt(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(object), new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static TClass JsonToObject<TClass>(this string jsonMessage)
        {
            return (TClass)(JsonSerializer.Deserialize(jsonMessage, typeof(TClass), new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
                MaxDepth = 64
            }));
        }

        public static TClass JsonToObjectAndDecrypt<TClass>(this string jsonMessage)
        {
            return (TClass)(JsonSerializer.Deserialize(jsonMessage, typeof(TClass), new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
                MaxDepth = 64
            }));
        }

        public static dynamic JsonToDynamic(this string jsonMessage, Type type)
        {
            return (dynamic)(JsonSerializer.Deserialize(jsonMessage, type, new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
            }));
        }

        public static string ToPrettyJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(object), new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            });
        }
    }

}
