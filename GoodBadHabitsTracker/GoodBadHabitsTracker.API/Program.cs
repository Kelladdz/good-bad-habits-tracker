using GoodBadHabitsTracker.Infrastructure.Extensions;
using GoodBadHabitsTracker.Core.Extensions;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Newtonsoft.Json;
using Asp.Versioning;
using GoodBadHabitsTracker.API.Extensions;
using Org.BouncyCastle.Crypto.Signers;
using Serilog;
using System.Web.WebPages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCore();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:8080").AllowCredentials().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Set-Cookie");
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/*app.UseCookiePolicy(
    new CookiePolicyOptions
    {
        Secure = CookieSecurePolicy.Always
    });*/

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseHsts();
app.UseHttpsRedirection();

app.MapControllers();
app.UseRouting();
app.UseCors();

app.UseAuthentication();

app.UseAuthorization();


app.Run();
