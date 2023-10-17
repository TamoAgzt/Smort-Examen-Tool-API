using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace VistaExamenPlanner.Extensions
{
    public static class CorsExtension
    {
        public static IServiceCollection SetupCorseAny(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Cors", Policy =>
                    Policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder EnableCorse(this IApplicationBuilder app)
        {

            app.UseCors("Cors");
            return app;
        }
    }
}
