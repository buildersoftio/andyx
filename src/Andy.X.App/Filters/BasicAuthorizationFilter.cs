using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace Buildersoft.Andy.X.Filters
{
    public class BasicAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (context.HttpContext.Request.Headers["Authorization"].ToString() != null)
                {
                    var authToken = context.HttpContext.Request.Headers["Authorization"];

                    // decoding authToken we get decode value in 'Username:Password' format
                    var decodeauthToken = Encoding.UTF8.GetString(
                        Convert.FromBase64String(authToken.ToString().Split(" ")[1]));

                    // spliting decodeauthToken using ':' 
                    var credentials = decodeauthToken.Split(':');

                    if (IsAuthorized(context, credentials[0], credentials[1]))
                        return;
                    else
                        ReturnUnauthorizedResult(context);
                }
                else
                    ReturnUnauthorizedResult(context);
            }
            catch (Exception)
            {
                ReturnUnauthorizedResult(context);
            }
        }


        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"NO_ACCESS You have to be authorized to use this endpoint. Please contact administrators for more info!";
            context.Result = new UnauthorizedResult();
        }

        public bool IsAuthorized(AuthorizationFilterContext context, string username, string password)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            return username == configuration.GetSection("Credentials:Username").Value
                && password == configuration.GetSection("Credentials:Password").Value;
        }
    }
}
