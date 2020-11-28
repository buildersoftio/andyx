using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public bool TenantOnly { get; set; } = false;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tenantName = context.HttpContext.Request.Headers["x-andy-x-tenant"].ToString();

            if (TenantOnly == true)
                if (tenantName == "" || context.HttpContext.Items["IsTenantValidated"].ToString() != true.ToString())
                    context.Result = new JsonResult(new { message = "Token is not valid for this tenant" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
