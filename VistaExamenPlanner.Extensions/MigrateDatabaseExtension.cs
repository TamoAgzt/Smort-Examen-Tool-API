using Microsoft.AspNetCore.Builder;
using MySql.Data.MySqlClient;
using Serilog;
using VistaExamenPlanner.Handler;

namespace VistaExamenPlanner.Extensions
{
    public static class MigrateDatabaseExtension
    {
        private static string MigratePassword = Environment.GetEnvironmentVariable("PasswordRootAccount") ?? "root";
        private static string MigrateEmail = Environment.GetEnvironmentVariable("EmailRootAccount") ?? "Root@vistacollege.nl";
        private static string MigrateNaam = Environment.GetEnvironmentVariable("NameRootAccount") ?? "Root";
        private static string MigrateLastName = Environment.GetEnvironmentVariable("LastNameRootAccount") ?? "Root";
        public static void UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (DatabaseHandler database = new DatabaseHandler())
            {
                Log.Information("Database Migrated");
                string[] qeuries = File.ReadAllLines("./Database.sql");
                database.Migrate(qeuries);

                MySqlCommand AddRoot = new MySqlCommand();
                AddRoot.CommandText = $"INSERT IGNORE INTO `Gebruikers` (Id, Rol_Id,Email, Wachtwoord, Naam, Achternaam) VALUES (1, 3, @Email, @password, @Name, @LastName);";
                AddRoot.Parameters.AddWithValue("@password", SecurityHandler.BcrypyBasicEncryption(MigratePassword));
                AddRoot.Parameters.AddWithValue("@Email", MigrateEmail);
                AddRoot.Parameters.AddWithValue("@Name", MigrateNaam);
                AddRoot.Parameters.AddWithValue("@LastName", MigrateLastName);
                database.Insert(AddRoot);

            }
        }
    }
}
