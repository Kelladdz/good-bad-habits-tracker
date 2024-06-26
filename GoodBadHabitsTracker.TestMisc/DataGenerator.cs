﻿using Bogus;
using GoodBadHabitsTracker.Application.DTOs.Auth.Request;
using GoodBadHabitsTracker.Application.Exceptions;
using GoodBadHabitsTracker.Core.Models;
using GoodBadHabitsTracker.Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.TestMisc
{
    public class DataGenerator()
    {
        public RegisterRequest SeedValidRegisterRequest()
        {
            var registerRequestGenerator = new Faker<RegisterRequest>()
                .RuleFor(rr => rr.Email, f => f.Internet.Email())
                .RuleFor(rr => rr.UserName, f => f.Internet.UserName())
                .RuleFor(rr => rr.Password, f => f.Internet.Password())
                .RuleFor(rr => rr.ConfirmPassword, (f, rr) => rr.Password);

            return registerRequestGenerator.Generate();
        }

        public LoginRequest SeedLoginRequest()
        {
            var loginRequestGenerator = new Faker<LoginRequest>()
                .RuleFor(rr => rr.Email, f => f.Internet.Email())
                .RuleFor(rr => rr.Password, f => f.Internet.Password());

            return loginRequestGenerator.Generate();
        }

        public string SeedAccessToken(string email)
        {
            var headerGenerator = new Faker<JwtHeader>()
                .CustomInstantiator(f => new JwtHeader()
                {
                    { "alg", "RS256" },
                    { "typ", "JWT" }
                });
            var payloadGenerator = new Faker<JwtPayload>()
                .CustomInstantiator(f => new JwtPayload()
                {
                    { JwtRegisteredClaimNames.Sub, f.Name.FirstName() },
                    { JwtRegisteredClaimNames.Name, f.Name.LastName() },
                    { JwtRegisteredClaimNames.Email, email },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
                    { "roles", "User" },
                    { "userFingerprint", f.Random.String2(32) },
                    { JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() },
                    { "authenticationMethod", "EmailPassword" },
                    { "email_verified", f.Internet.Email() },
                    { "iss", $"{f.Internet.Url}:{f.Internet.Port}" },
                    { "aud", $"{f.Internet.Url}:{f.Internet.Port}" },
                    { "exp", DateTime.UtcNow },
                });

            var signatureGenerator = new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32));



            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32))));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var header = (JwtHeader)headerGenerator;
            var payload = (JwtPayload)payloadGenerator;


            var token = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken.ToString();
        }

        public string SeedRandomString()
        {
            var tokenGenerator = new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32));

            var token = (string)tokenGenerator;
            return token;
        }

        public UserSession SeedUserSession()
        {
            var userName = new Faker<string>().CustomInstantiator(f => f.Internet.UserName()).Generate();
            var email = new Faker<string>().CustomInstantiator(f => f.Internet.Email()).Generate();

            return new UserSession(Guid.NewGuid(), userName, email, ["User"]);
        }

        public Dictionary<string, string> SeedConfiguration()
        {
            var configurationGenerator = new Faker<Dictionary<string, string>>()
                .CustomInstantiator(f => new Dictionary<string, string>
                {
                    { "JwtSettings:Issuer", $"{f.Internet.Url}:{f.Internet.Port}" },
                    { "JwtSettings:Audience", $"{f.Internet.Url}:{f.Internet.Port}" },
                    { "JwtSettings:Key", f.Random.String2(32) },
                });

            return configurationGenerator.Generate();
        }
    }
}
