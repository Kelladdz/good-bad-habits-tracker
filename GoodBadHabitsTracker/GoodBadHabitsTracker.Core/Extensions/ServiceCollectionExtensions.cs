using GoodBadHabitsTracker.Core.Mappings;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using GoodBadHabitsTracker.Core.Services.UserAccessor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IHabitsService, HabitsService>();
            services.AddAutoMapper(typeof(HabitsMappingProfile));
            services.AddScoped<IUserAccessor, UserAccessor>();
        }
    }
}
