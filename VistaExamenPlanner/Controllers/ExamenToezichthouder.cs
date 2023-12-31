using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using VistaExamenPlanner.Handler;
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Controllers
{
    [ApiController]
    public class ExamenToZichthouder : ControllerBase
    {
        private readonly ILogger<ExamenToZichthouder> _logger;

        public ExamenToZichthouder(ILogger<ExamenToZichthouder> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("GetToeZichtHouder")]
        public string GetToezichtHouders()
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = "SELECT Id, Naam, Achternaam FROM Gebruikers WHERE Rol_Id=2 ";
                var result = database.Select(command);
                return result;
            }
        }

        //[Authorize]
        //[HttpPost("AddToeZichtHouder")]
        //public void AddToeZichtHouder(Toezichthouders toezichthouders)
        //{
        //    if (toezichthouders is null)
        //    {
        //        throw new ArgumentNullException(nameof(toezichthouders));
        //    }

        //    using (DatabaseHandler database = new())
        //    {
        //        MySqlCommand command = new();

        //        command.CommandText = $"INSERT INTO Toezichthouders ( Naam,Tussenvoegsel,Achternaam) VALUES (@Name,@MiddleName,@LastName);";
        //        command.Parameters.AddWithValue("@Name", toezichthouders.Name);
        //        command.Parameters.AddWithValue("@MiddleName", toezichthouders.MiddleName);
        //        command.Parameters.AddWithValue("@LastName", toezichthouders.LastName);

        //        database.Insert(command);
        //    }
        //}

        //[Authorize]
        //[HttpPost("DeleteToezichthouder")]
        //public void DeleteToezichthouder(int IdToDropTable)
        //{
        //    using (DatabaseHandler database = new())
        //    {
        //        MySqlCommand command = new MySqlCommand();

        //        command.CommandText = $"DELETE FROM Toezichthouders WHERE Id = @IdToDropTable;";
        //        command.Parameters.AddWithValue("@IdToDropTable", IdToDropTable);
        //        database.Delete(command);
        //    }
        //}
    }
}