using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Core;
using System.ComponentModel.DataAnnotations.Schema;
using VistaExamenPlanner.Extensions;
using VistaExamenPlanner.Handler;

namespace VistaExamenPlanner.Extensions
{
    public static class MigrateDatabaseExtension
    {
        public static void UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (DatabaseHandler database = new DatabaseHandler())
            {
                Log.Information("Database Migrated");
                string[] qeuries = File.ReadAllLines("./Database.sql");
                database.Migrate(qeuries);
            }
          
        }
    }
}
