using AspNet.Security.OAuth.Validation;
using Buildersoft.Andy.X.Data.Model.Authentication;
using Buildersoft.Andy.X.Logic.Authorization;
using Buildersoft.Andy.X.Utilities.Authentication.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Extensions.DI
{
    public static class JwtToken
    {
        /// <summary>
        /// This extension with generate the authentication options for Buildersoft Systems.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Part of appSettings has to be an tag Jwt with other options</param>
        /// <returns></returns>
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection("Jwt"));

            services.AddSingleton<IAuthorizationLogic, AuthorizationLogic>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidAudience = configuration["Jwt:Audience"],
                            IssuerSigningKey = JwtSecurityKey.Create(configuration["Jwt:Key"])
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                return Task.CompletedTask;
                            },
                            OnMessageReceived = context =>
                            {
                                return Task.FromResult(0);
                            }
                        };
                    });


            return services;
        }

        /// <summary>
        /// This extension with generate the authorization options for Buildersoft Systems.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Member",
                    policy => policy.RequireClaim("MembershipId"));
                options.AddPolicy("Internal",
                    policy => policy.RequireClaim("InternalId"));
            });

            return services;
        }
    }

}
