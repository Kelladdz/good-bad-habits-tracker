using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using GoodBadHabitsTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {       
            services.AddDbContext<HabitsDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));
            services.AddScoped<IHabitsRepository, HabitsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<HabitsDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider)
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, HabitsDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, HabitsDbContext, Guid>>();
            services.AddAuthentication()
                .AddCookie("CookiesAuth", options =>
                {
                    options.Cookie.Name = "JSESSIONID";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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

                })
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
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.ClaimActions.MapJsonKey("image", "picture");
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
        }
    }
}
