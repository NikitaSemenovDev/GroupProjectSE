using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupProject.ExternalServices;
using GroupProject.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace GroupProject.Services
{
    public static class DependencyRegistration
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddTransient<ILogger, Log4NetLogger>();
        }
    }
}
