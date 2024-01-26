using Azure.Core;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Infrastructure.Configurations;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using GoodBadHabitsTracker.Infrastructure.Repositories;
using GoodBadHabitsTracker.Infrastructure.Services.IdTokenHandler;
using GoodBadHabitsTracker.Infrastructure.Services.JwtTokenGenerator;
using GoodBadHabitsTracker.Infrastructure.Services.JwtTokenRevoker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDbContext<HabitsDbContext>(options =>
                options.UseSqlServer(configuration.GetSection("ConnectionStrings:Default").Value));
            services.AddHttpContextAccessor()
                .AddSingleton<IJwtTokenRevoker, JwtTokenRevoker>();
            services.AddScoped<IHabitsRepository, HabitsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IIDTokenHandler, IdTokenHandler>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<HabitsDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, HabitsDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, HabitsDbContext, Guid>>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        AudienceValidator = (audience, securityToken, validationParameters) =>
                        {
                            var jwtToken = securityToken as JsonWebToken;
                            if (jwtToken is null) return false;

                            var userFingerprintClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userFingerprint")?.Value;
                            if (userFingerprintClaim is null) return false;

                            var isValid = audience.Any(audience => audience.Equals(validationParameters.ValidAudience, StringComparison.OrdinalIgnoreCase));
                            return isValid;
                        },
                        LifetimeValidator = new JwtTokenRevoker().ValidateTokenLifetime
                    };
                    new JwtBearerEvents().OnAuthenticationFailed = (context) =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.WriteAsync("Unauthorized");
                        return Task.CompletedTask;
                    };
                })

                /*.AddCookie("CookiesAuth", options =>
                {
                    options.Cookie.Name = "JSESSIONID";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.IsEssential = true;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = new TimeSpan(2, 0, 0);
                    options.Events.OnSignedIn = (context) =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.Cookies.Append("ONSESS", "true", new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            IsEssential = true,
                            Expires = DateTimeOffset.UtcNow.AddHours(2)
                        });
                        return Task.CompletedTask;
                    };
                    options.Events.OnSigningOut = (context) =>
                    {
                        context.Response.Cookies.Delete("JSESSIONID");
                        context.Response.Cookies.Delete("ONSESS");
                        return Task.CompletedTask;
                    };

                })*/
                /*.AddOpenIdConnect("Google", options =>
                {
                    options.ClientId = configuration.GetSection("web:client_id").Value;
                    options.ClientSecret = configuration.GetSection("web:client_secret").Value;
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.ClaimActions.MapJsonKey("image", "picture");
                });*/
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = configuration.GetSection("web:client_id").Value;
                    options.ClientSecret = configuration.GetSection("web:client_secret").Value;
                    options.AuthorizationEndpoint = "https://localhost:7154/o/oauth2/v2/auth";
                    options.AccessType = "online";
                    options.ClaimActions.MapJsonKey("image", "picture");
                    options.SaveTokens = true;
                    options.Events.OnTicketReceived = (context) =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.Cookies.Append("ONSESS", "true", new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            IsEssential = true,
                            Expires = DateTimeOffset.UtcNow.AddHours(2)
                        });
                        return Task.CompletedTask;
                    };
                    
                }).
                AddFacebook(options =>
                {
                    options.AppSecret = configuration.GetSection("web:facebook_client_secret").Value;
                    options.AppId = configuration.GetSection("web:facebook_app_id").Value;
                    options.Scope.Add("email");
                    options.Scope.Add("public_profile");
                });
            /*.AddOAuth<GoogleOptions, GoogleHandler>(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = configuration.GetSection("web:client_id").Value;
                options.ClientSecret = configuration.GetSection("web:client_secret").Value;
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.ClaimActions.MapJsonKey("image", "picture");
            });*/
            services.AddAuthorizationBuilder()
                .AddPolicy("CommonPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim("userFingerprint");
                });
            
        }
    }
}
