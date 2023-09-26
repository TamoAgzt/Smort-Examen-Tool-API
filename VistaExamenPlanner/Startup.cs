using VistaExamenPlanner.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace VistaExamenPlanner
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddAuthorization();

            services.AddMvc();

            services.AddSwaggerGen();

            services.AddEndpointsApiExplorer();

            services.AddSerilog(Configuration);

        }
        public void Configure(IApplicationBuilder app)
        {

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseDatabaseMigration();

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
