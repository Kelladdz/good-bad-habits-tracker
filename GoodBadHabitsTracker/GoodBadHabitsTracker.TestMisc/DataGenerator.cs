using Bogus;
using FluentAssertions.Common;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.TestMisc
{
    public class DataGenerator
    {
        Random random = new Random();
        string[] allowedFreqValues = ["daily", "weekly", "monthly"];
        string[] allowedDaysOfWeek = ["monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday",
            "sunday"];
        string[] possibleProviders = ["Google", "Facebook"];

        
        public IEnumerable<Habit> SeedHabitsCollection(int number)
        {
            var habitsGenerator = new Faker<Habit>()
                .RuleFor(h => h.HabitId, f => Guid.NewGuid())
                .RuleFor(h => h.Name, f => f.Name.JobTitle())
                .RuleFor(h => h.UserId, f => Guid.NewGuid())
                .RuleFor(h => h.IsGood, f => f.Random.Bool())
                .RuleFor(h => h.IsGoalInTime, f => f.Random.Bool())
                .RuleFor(h => h.Quantity, f => f.Random.Byte())
                .RuleFor(h => h.Frequency, f => f.PickRandom(allowedFreqValues))
                .RuleFor(h => h.IsRepeatDaily, f => true)
                .RuleFor(h => h.RepeatDaysOfWeek, f => Enumerable.Range(1, random.Next(2,7)).Select(x => f.PickRandom(allowedDaysOfWeek)).ToArray())
                .RuleFor(h => h.RepeatDaysOfMonth, f => [])
                .RuleFor(h => h.StartDate, f => f.Date.FutureDateOnly())
                .RuleFor(h => h.ReminderTime, f => f.Date.SoonTimeOnly());

            IEnumerable<Habit> habits = habitsGenerator.Generate(number);
            return habits;
        }
        public Habit SeedHabit()
        {
            var habitsGenerator = new Faker<Habit>()
                .RuleFor(h => h.HabitId, f => Guid.NewGuid())
                .RuleFor(h => h.Name, f => f.Name.JobTitle())
                .RuleFor(h => h.UserId, f => Guid.NewGuid())
                .RuleFor(h => h.IsGood, f => f.Random.Bool())
                .RuleFor(h => h.IsGoalInTime, f => f.Random.Bool())
                .RuleFor(h => h.Quantity, f => f.Random.Byte())
                .RuleFor(h => h.Frequency, f => f.PickRandom(allowedFreqValues))
                .RuleFor(h => h.IsRepeatDaily, f => true)
                .RuleFor(h => h.RepeatDaysOfWeek, f => Enumerable.Range(1, random.Next(2, 7)).Select(x => f.PickRandom(allowedDaysOfWeek)).ToArray())
                .RuleFor(h => h.RepeatDaysOfMonth, f => [])
                .RuleFor(h => h.StartDate, f => f.Date.FutureDateOnly())
                .RuleFor(h => h.ReminderTime, f => f.Date.SoonTimeOnly());

            Habit habit = habitsGenerator.Generate();
            return habit;
        }
        public HabitDto SeedHabitDto()
        {
            var habitsGenerator = new Faker<HabitDto>()
                .RuleFor(h => h.Name, f => f.Name.JobTitle())
                .RuleFor(h => h.IsGood, f => f.Random.Bool())
                .RuleFor(h => h.IsGoalInTime, f => f.Random.Bool())
                .RuleFor(h => h.Quantity, f => f.Random.Byte())
                .RuleFor(h => h.Frequency, f => f.PickRandom(allowedFreqValues))
                .RuleFor(h => h.IsRepeatDaily, f => true)
                .RuleFor(h => h.RepeatDaysOfWeek, f => Enumerable.Range(1, random.Next(2, 7)).Select(x => f.PickRandom(allowedDaysOfWeek)).ToArray())
                .RuleFor(h => h.RepeatDaysOfMonth, f => [])
                .RuleFor(h => h.StartDate, f => f.Date.FutureDateOnly())
                .RuleFor(h => h.ReminderTime, f => f.Date.SoonTimeOnly());

            HabitDto habitDto = habitsGenerator.Generate();
            return habitDto;
        }
        public RegisterDto SeedRegisterDto()
        {
            var registerDtoGenerator = new Faker<RegisterDto>()
                .RuleFor(r => r.Name, f => f.Internet.UserName())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Password, f => f.Internet.Password())
                .RuleFor(r => r.ConfirmPassword, (f, r) => r.Password);

            RegisterDto registerDto = registerDtoGenerator.Generate();
            return registerDto;
        }

        public LoginDto SeedLoginDto()
        {
            var loginDtoGenerator = new Faker<LoginDto>()
                .RuleFor(l => l.Email, f => f.Internet.Email())
                .RuleFor(l => l.Password, f => f.Internet.Password());

            LoginDto loginDto = loginDtoGenerator.Generate();
            return loginDto;
        }

        public ApplicationUser SeedUser()
        {
            var userGenerator = new Faker<ApplicationUser>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
                .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool())
                .RuleFor(u => u.PasswordHash, f => f.Random.String())
                .RuleFor(u => u.SecurityStamp, f => f.Random.String())
                .RuleFor(u => u.ConcurrencyStamp, f => f.Random.String())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.PhoneNumberConfirmed, f => f.Random.Bool())
                .RuleFor(u => u.TwoFactorEnabled, f => f.Random.Bool())
                .RuleFor(u => u.LockoutEnd, f => f.Date.FutureOffset())
                .RuleFor(u => u.LockoutEnabled, f => f.Random.Bool())
                .RuleFor(u => u.AccessFailedCount, f => f.Random.Int())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName());

            ApplicationUser user = userGenerator.Generate();
            return user;
        }

        public IEnumerable<ApplicationUser> SeedUsersCollection(int number)
        {
            var userGenerator = new Faker<ApplicationUser>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
                .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool())
                .RuleFor(u => u.PasswordHash, f => f.Random.String())
                .RuleFor(u => u.SecurityStamp, f => f.Random.String())
                .RuleFor(u => u.ConcurrencyStamp, f => f.Random.String())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.PhoneNumberConfirmed, f => f.Random.Bool())
                .RuleFor(u => u.TwoFactorEnabled, f => f.Random.Bool())
                .RuleFor(u => u.LockoutEnd, f => f.Date.FutureOffset())
                .RuleFor(u => u.LockoutEnabled, f => f.Random.Bool())
                .RuleFor(u => u.AccessFailedCount, f => f.Random.Int())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName());

            IEnumerable<ApplicationUser> users = userGenerator.Generate(number);
            return users;
        }
        public string SeedGoogleIdToken()
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
                    { "given_name", f.Name.FirstName() },
                    { "family_name", f.Name.LastName() },
                    { "nickname", f.Internet.UserName() },
                    { "name", f.Internet.UserName() },
                    { "picture", f.Internet.Avatar() },
                    { "locale", f.Random.RandomLocale() },
                    { "updated_at", DateTime.Now.ToString() },
                    { "email", f.Internet.Email() },
                    { "email_verified", f.Internet.Email() },
                    { "iss", f.Internet.Url() },
                    { "aud", f.Random.String2(24) },
                    { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                    { "exp", DateTimeOffset.UtcNow.AddHours(10).ToUnixTimeSeconds().ToString() },
                    { "sub", f.Random.String(20) },
                    { "sid", f.Random.String(30) }
                });

            var signatureGenerator = new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32));



            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456789012"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var header = (JwtHeader)headerGenerator;
            var payload = (JwtPayload)payloadGenerator;


            var token = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken.ToString();
        }

        public string SeedGoogleIdTokenWithoutProviderKey()
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
                    { "given_name", f.Name.FirstName() },
                    { "family_name", f.Name.LastName() },
                    { "nickname", f.Internet.UserName() },
                    { "name", f.Internet.UserName() },
                    { "picture", f.Internet.Avatar() },
                    { "locale", f.Random.RandomLocale() },
                    { "updated_at", DateTime.Now.ToString() },
                    { "email", f.Internet.Email() },
                    { "email_verified", f.Internet.Email() },
                    { "iss", f.Internet.Url() },
                    { "aud", f.Random.String2(24) },
                    { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                    { "exp", DateTimeOffset.UtcNow.AddHours(10).ToUnixTimeSeconds().ToString() },
                    { "sid", f.Random.String(30) }
                });

            var signatureGenerator = new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32));


            var header = (JwtHeader)headerGenerator;
            var payload = (JwtPayload)payloadGenerator;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456789012"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken.ToString();
        }

        public string SeedGoogleIdTokenWithoutEmail()
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
                    { "given_name", f.Name.FirstName() },
                    { "family_name", f.Name.LastName() },
                    { "nickname", f.Internet.UserName() },
                    { "name", f.Internet.UserName() },
                    { "picture", f.Internet.Avatar() },
                    { "locale", f.Random.RandomLocale() },
                    { "updated_at", DateTime.Now.ToString() },
                    { "iss", f.Internet.Url() },
                    { "aud", f.Random.String2(24) },
                    { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                    { "exp", DateTimeOffset.UtcNow.AddHours(10).ToUnixTimeSeconds().ToString() },
                    { "sub", f.Random.String(20) },
                    { "sid", f.Random.String(30) }
                });

            var signatureGenerator = new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32));



            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456789012"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var header = (JwtHeader)headerGenerator;
            var payload = (JwtPayload)payloadGenerator;


            var token = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken.ToString();
        }

        public string SeedToken()
        {
            var tokenGenerator = new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32));

            var token = (string)tokenGenerator;
            return token;
        }
        public string SeedInvalidProvider()
        {
            var invalidProviderGenerator = new Faker();
            var invalidProvider = invalidProviderGenerator.Lorem.Word();
            return invalidProvider;
        }

        public string SeedFingerprint()
        {
            var fingerprintGenerator = new Faker<string>()
                .CustomInstantiator(f => f.Random.String2(32));

            var fingerprint = (string)fingerprintGenerator;
            return fingerprint;
        }
    }
}
