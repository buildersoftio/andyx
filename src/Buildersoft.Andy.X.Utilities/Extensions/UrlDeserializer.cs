using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Data.Model.Url;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Utilities.Extensions
{
    public static class UrlDeserializer
    {
        public static XUrl ToXUrl(this string url)
        {
            //Example:
            // we have the function id first connect_, or keepAlive_
            // andyx://{tenant}/{product}/{component}/{book}/{reader}/{reader_type}
            // 0     1     2        3         4          5      6        7
            int indexOfFunctionDelimiter = url.IndexOf('_');
            if (indexOfFunctionDelimiter != -1)
                url = url.Remove(0, indexOfFunctionDelimiter + 1);

            string[] urlSplited = url.Split(@"/");
            if (urlSplited[0] == "andyx:")
            {
                ReaderTypes readerType = (ReaderTypes)Enum.Parse(typeof(ReaderTypes), urlSplited[7]);
                return new XUrl()
                {
                    Protocol = "andyx",
                    Tenant = urlSplited[2],
                    Product = urlSplited[3],
                    Component = urlSplited[4],
                    Book = urlSplited[5],
                    Reader = urlSplited[6],

                    ReaderType = readerType
                };
            }
            return null;
        }
    }
}
