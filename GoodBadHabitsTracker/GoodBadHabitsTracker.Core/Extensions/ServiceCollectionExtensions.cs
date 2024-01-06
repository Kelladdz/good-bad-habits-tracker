using GoodBadHabitsTracker.Core.Mappings;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using GoodBadHabitsTracker.Core.Services.UserAccessor;
using GoodBadHabitsTracker.Core.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;

namespace GoodBadHabitsTracker.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCore(this IServiceCollection services)
        {
            
            services.AddScoped<IHabitsService, HabitsService>();
            services.AddAutoMapper(typeof(HabitsMappingProfile));
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
