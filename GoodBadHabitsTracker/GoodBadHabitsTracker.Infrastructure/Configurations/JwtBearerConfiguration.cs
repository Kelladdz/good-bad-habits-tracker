using GoodBadHabitsTracker.Infrastructure.Services.JwtTokenRevoker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Configurations
{
    public static class JwtBearerConfiguration
    {
        private static IServiceProvider? services;
        private static IJwtTokenRevoker? JwtTokenRevoker => services!.GetService<IJwtTokenRevoker>();

        public static IApplicationBuilder UseJwtTokenManagement(this IApplicationBuilder builder)
        {
            services = builder.ApplicationServices;

            return builder;
        }

        public static Action<JwtBearerOptions> SetUp(WebApplicationBuilder builder) =>
           options =>
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
                   LifetimeValidator = LifetimeValidator
               };
           };

        private static bool LifetimeValidator(DateTime? notBefore,
                                               DateTime? expires,
                                               SecurityToken securityToken,
                                               TokenValidationParameters validationParameters) =>
           JwtTokenRevoker?.ValidateTokenLifetime(notBefore, expires, securityToken, validationParameters) ?? false;
    }
}
