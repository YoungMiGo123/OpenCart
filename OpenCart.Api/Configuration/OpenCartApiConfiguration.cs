using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using OpenCart.Services.Services.SettingsService;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace OpenCart.Api.Configuration
{
    public static class OpenCartApiConfiguration
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IOpenCartServiceSettings openCartServiceSettings)
        {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "cookie";
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = "cookie";
                    options.DefaultChallengeScheme = "GitHub";
                })
                .AddCookie("cookie")
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = openCartServiceSettings?.SecureOAuthSettings?.ClientId ?? throw new ArgumentNullException("ClientId");
                    options.ClientSecret = openCartServiceSettings?.SecureOAuthSettings?.ClientSecret ?? throw new ArgumentNullException("ClientSecret");

                    options.CallbackPath = new PathString("/SecureGithubCallbackResponse");

                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";
                    options.SaveTokens = true;

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Locality, "location");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(user);
                        }
                    };
                });
        }

    }
}
