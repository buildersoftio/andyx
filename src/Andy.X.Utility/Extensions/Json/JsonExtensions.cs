using Newtonsoft.Json;

namespace Buildersoft.Andy.X.Utility.Extensions.Json
{
    public static class JsonExtensions
    {


        public static TClass JsonToObject<TClass>(this string jsonMessage)
        {
            return (TClass)(JsonConvert.DeserializeObject(jsonMessage, typeof(TClass)));
        }
        public static string ToPrettyJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

}
