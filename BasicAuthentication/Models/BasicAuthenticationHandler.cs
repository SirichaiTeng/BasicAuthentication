using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;

namespace BasicAuthentication.Models
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        IConfiguration _config;

        public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration config
        ) : base(options, logger, encoder, clock)
        {
            _config = config;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("No header found"));

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

            if (authHeader.Parameter != null)
            {
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes);

                if (!string.IsNullOrEmpty(credentials))
                {
                    string[] array = credentials.Split(':', 2);
                    string username = array[0];
                    string password = array[1];

                    if (username.Equals(_config["BasicAuth:Username"]) && password.Equals(_config["BasicAuth:Password"]))
                    {
                        var claims = new[] { new Claim("name", username) };
                        var identity = new ClaimsIdentity(claims, "Basic");
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                    else
                    {
                        return Task.FromResult(AuthenticateResult.Fail("Username or password is incorrect"));
                    }

                }
                else
                {
                    return Task.FromResult(AuthenticateResult.Fail("UnAuthorized"));
                }
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }

        }

    }
}
