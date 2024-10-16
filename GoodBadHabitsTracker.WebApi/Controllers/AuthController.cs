﻿using Microsoft.AspNetCore.Mvc;
using MediatR;
using GoodBadHabitsTracker.Application.Commands.Auth.Register;
using GoodBadHabitsTracker.Application.Commands.Auth.Login;
using GoodBadHabitsTracker.Application.DTOs.Auth.Request;


namespace GoodBadHabitsTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register
            ([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new RegisterCommand(request, cancellationToken));
            return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login
            ([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new LoginCommand(request, cancellationToken));

            Response.Cookies.Append("__Secure-Fgp", response.UserFingerprint, new CookieOptions
            {
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Secure = true,
                MaxAge = TimeSpan.FromMinutes(15),
            });
            return Ok(new { accessToken = response.AccessToken.ToString(), refreshToken = response.RefreshToken });
        }
    }
}