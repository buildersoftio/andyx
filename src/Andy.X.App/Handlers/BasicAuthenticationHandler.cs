using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly List<CredentialsConfiguration> _credentialsConfigurations;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            List<CredentialsConfiguration> credentialsConfigurations) : base(options, logger, encoder, clock)
        {
            _credentialsConfigurations = credentialsConfigurations;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string username = null;
            string role = null;
            try
            {
                var headers = Request.Headers.Authorization;
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                username = credentials.FirstOrDefault();
                var password = credentials.LastOrDefault();

                if (!IsAuthorized(username, password, out role))
                    throw new ArgumentException("Invalid credentials");
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail($"Authentication failed: {ex.Message}"));
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public bool IsAuthorized(string username, string password, out string role)
        {
            role = "";
            var credentials = _credentialsConfigurations
                .Where(c => c.Username == username && c.Password == password)
                .FirstOrDefault();

            if (credentials is null)
                return false;

            role = credentials.Role.ToString();
            return true;
        }
    }
}