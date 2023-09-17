using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


namespace VistaExamenPlanner.Extensions
{
    public static class AddSerilogExtension
    {
        public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            return services;
        }
    }
}
