using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using OpenCart.Infrastructure.Context;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using OpenCart.Operations.Mapping;
using OpenCart.Repositories.Repositories.CartItemImageRepository;
using OpenCart.Repositories.Repositories.CartItemRepository;
using OpenCart.Repositories.Repositories.UserRepository;
using OpenCart.Services.Services.CartService;
using OpenCart.Services.Services.SettingsService;
using OpenCart.Services.Services.UserService;
using OpenCart.Services.Services.Validators;
using Serilog;
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
        public static void ConfigureServices(this IServiceCollection services, IOpenCartServiceSettings openCartServiceSettings)
        {
            services.AddAutoMapper(typeof(CartItemMapping));
            services.AddScoped<IValidator<CartItemImageDto>, CartImageValidator>();
            services.AddScoped<IValidator<CartItemDto>, CartValidator>();
            services.AddScoped<IValidator<ApplicationUser>, UserValidator>();
            services.AddScoped<ICartItemImageRepository, CartItemImageRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICartService, CartService>();
        }
        public static void ConfigureDatabase(this IServiceCollection services, IOpenCartServiceSettings openCartServiceSettings)
        {
            services.AddDbContext<OpenCartDbContext>(options => options.UseSqlServer(openCartServiceSettings.ConnectionString));
        }

        public static void ConfigureLogging(this WebApplicationBuilder builder, IOpenCartServiceSettings openCartServiceSettings)
        {
            builder.Host.UseSerilog((ctx, lc) =>
            {
                var seqUrl = openCartServiceSettings.SeqUrl;

                lc.MinimumLevel.Warning();
                lc.WriteTo.Console();
                if (!string.IsNullOrEmpty(seqUrl) && seqUrl.StartsWith("http"))
                {
                    lc.WriteTo.Seq(seqUrl);
                }
            });

            builder.Services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);
        }
    }
}
