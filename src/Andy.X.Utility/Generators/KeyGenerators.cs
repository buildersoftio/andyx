using System;
using System.Security.Cryptography;

namespace Buildersoft.Andy.X.Utility.Generators
{
    public static class KeyGenerators
    {
        public static string GenerateApiKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);

            return Convert.ToBase64String(key);
        }
    }
}
