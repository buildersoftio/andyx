using NJsonSchema;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Utilities.Extensions
{
    public static class JsonSchemaValidation
    {
        public static string ToJsonSchema<T>(this T t) where T : class
        {
            return JsonSchema.FromType<T>().ToJson();
        }

        public static bool IsSchemaValid<T>(this string jsonRawData) where T : class
        {
            var jsonSchema = JsonSchema.FromType<T>();
            var errors = jsonSchema.Validate(jsonRawData);
            if (errors.Count > 0)
                return false;

            return true;
        }

        public async static Task<bool> IsSchemaValidAsync(this string jsonRawData, string jsonSchemaData)
        {
            var jsonSchema = await JsonSchema.FromJsonAsync(jsonSchemaData);
            var errors = jsonSchema.Validate(jsonRawData);
            if (errors.Count > 0)
                return false;

            return true;
        }
    }
}
