using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Buildersoft.Andy.X.Utilities.Extensions
{
    public static class StreamSerialization
    {
        public static String ToEncodedString(this Stream stream, Encoding enc = null)
        {
            enc = enc ?? Encoding.UTF8;

            byte[] bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);
            string data = enc.GetString(bytes);

            return enc.GetString(bytes);
        }
    }
}
