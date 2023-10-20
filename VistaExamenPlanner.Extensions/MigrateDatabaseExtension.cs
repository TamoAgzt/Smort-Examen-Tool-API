using Microsoft.AspNetCore.Builder;
using MySql.Data.MySqlClient;
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

                MySqlCommand AddRoot = new MySqlCommand();
                AddRoot.CommandText = "INSERT IGNORE INTO `Gebruikers` (Id, Rol_Id,Email, Wachtwoord, Naam, Achternaam) VALUES (1, 3, \"Root@vistacollege.nl\", @password, \"Root\", \"Root\");";
                AddRoot.Parameters.AddWithValue("@password", SecurityHandler.BcrypyBasicEncryption("root"));
                database.Insert(AddRoot);
            }
        }
    }
}
