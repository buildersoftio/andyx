using System;
using System.Security.Cryptography;
using System.Text;

namespace Buildersoft.Andy.X.Utility.Extensions
{
    public static class CryptographyExtensions
    {
        public static string ToHashString(this string text, string stalt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA512 to create the hash
            using (HashAlgorithm algorithm = SHA512.Create())
            {
                // Convert the string to a byte array first, to be processed
                byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text + stalt));

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }
    }
}
