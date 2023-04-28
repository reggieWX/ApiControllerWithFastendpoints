using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace ApiControllerWithFastendpoints.Authentication
{
    [ExcludeFromCodeCoverage]
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "API Key";
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }
}
