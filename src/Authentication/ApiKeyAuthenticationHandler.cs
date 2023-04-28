using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ApiControllerWithFastendpoints.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ApiKeyHeaderName = "x-api-key";
        private readonly ApiKeys _keys;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<ApiKeys> keys
            ) : base(options, logger, encoder, clock)
        {
            _keys = keys.Value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.Fail("Invalid API Key provided.");
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.Fail("Invalid API Key provided.");
            }

            var existingApiKey = _keys.FirstOrDefault(k => k.Key == providedApiKey);

            if (existingApiKey == null)
                return AuthenticateResult.Fail("Invalid API Key provided.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingApiKey.Owner)
            };

            claims.AddRange(existingApiKey
                .Roles
                .Split(',')
                .Select(role => new Claim(ClaimTypes.Role, role.Trim())));

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}