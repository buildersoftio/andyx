﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Buildersoft.Andy.X.Extensions
{
    public static class HttpContextExtensions
    {
        public static void LogApiCallFrom<T>(this ILogger<T> logger, HttpContext httpContext)
        {
            var isFromCli = httpContext.Request.Headers["x-called-by"].ToString();
            if (isFromCli != "")
                logger.LogInformation($"{isFromCli} {httpContext.Request.Method} '{httpContext.Request.Path}' is called");
            else
                logger.LogInformation($"{httpContext.Request.Method} '{httpContext.Request.Path}' is called");
        }
    }
}
