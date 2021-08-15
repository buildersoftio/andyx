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
        public bool DataStorageOnly { get; set; } = false;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (TenantOnly == true)
            {
                var tenantName = context.HttpContext.Request.Headers["x-andy-x-tenant"].ToString();
                if (tenantName == "" || context.HttpContext.Items["IsTenantValidated"].ToString() != true.ToString())
                    context.Result = new JsonResult(new { message = "Token is not valid for this tenant" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (DataStorageOnly == true)
            {
                var dataStorage = context.HttpContext.Request.Headers["x-andyx-datastorage"].ToString();
                if(dataStorage== ""||context.HttpContext.Items["IsDataStorageValidated"].ToString()!= true.ToString())
                    context.Result = new JsonResult(new { message = "Token is not valid for this data storage" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
