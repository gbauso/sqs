using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Some.Lambda
{
    public static class Bootstrapper
    {
        public static ServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddSingleton(logger);

            services.AddSingleton(configuration);

            return services.BuildServiceProvider();
        }
    }
}
