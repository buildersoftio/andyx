using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Buildersoft.Andy.X.Utilities.Extensions
{
    public static class JsonSerialization
    {
        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(object), new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static TClass JsonToObject<TClass>(this string jsonMessage) where TClass : new()
        {
            return (TClass)JsonSerializer.Deserialize(jsonMessage, typeof(TClass), new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static object JsonToObject(this string jsonMessage)
        {
            return (object)JsonSerializer.Deserialize(jsonMessage, typeof(object), new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static string ToPrettyJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(object), new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            });
        }
    }
}
