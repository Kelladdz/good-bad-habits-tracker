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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCore();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<HabitsDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<ApplicationUser>();

app.UseHttpsRedirection();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
