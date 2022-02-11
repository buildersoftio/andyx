using Buildersoft.Andy.X.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Buildersoft.Andy.X.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthorizeAttribute : TypeFilterAttribute
    {
        public BasicAuthorizeAttribute() : base(typeof(BasicAuthorizationFilter))
        {

        }
    }
}
