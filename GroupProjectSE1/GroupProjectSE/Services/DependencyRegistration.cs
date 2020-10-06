using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupProjectSE.ExternalServices;
using GroupProjectSE.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace GroupProjectSE.Services
{
    public static class DependencyRegistration
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddTransient<ILogger, Log4NetLogger>();
        }
    }
}
