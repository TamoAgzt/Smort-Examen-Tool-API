using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using VistaExamenPlanner.Handler;
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Controllers
{
    [ApiController]
    public class ExamenLokaal : ControllerBase
    {
        private readonly ILogger<ExamenLokaal> _logger;

        public ExamenLokaal(ILogger<ExamenLokaal> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("GetLokaal")]
        public string GetLokaal()
        {
            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Warning, $"GetLokaal: Someone without the rights tried to get a list of all the classrooms.");
                return "NoPerm";
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = "SELECT * FROM Lokaal";
                var result = database.Select(command);
                return result;
            }
        }

        [Authorize]
        [HttpPost("AddLokaal")]
        public void AddLokaal(Lokaal lokaal)
        {
            if (lokaal is null)
            {
                throw new ArgumentNullException(nameof(lokaal));
            }

            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Warning, $"AddLokaal: Someone without the rights tried to create a ClassRoom.");
                return;
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = $"INSERT INTO Lokaal (Lokaal) VALUES (@ExamenLokaal);";
                command.Parameters.AddWithValue("@ExamenLokaal", lokaal.ExamenLokaal);
                database.Insert(command);
            }
        }

        [Authorize]
        [HttpDelete("DeleteLokaal")]
        public void DeleteLokaal(int IdToDropTable)
        {
            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Warning, $"DeleteLokaal: Someone without the rights tried to Delete a ClassRoom.");
                return;
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand DropFromAgenda = new MySqlCommand();

                DropFromAgenda.CommandText = $"DELETE FROM AgendaItem WHERE Lokaal_Id = @IdToDropTable;";
                DropFromAgenda.Parameters.AddWithValue("@IdToDropTable", IdToDropTable);
                database.Delete(DropFromAgenda);

                MySqlCommand command = new MySqlCommand();

                command.CommandText = $"DELETE FROM Lokaal WHERE Id = @IdToDropTable;";
                command.Parameters.AddWithValue("@IdToDropTable", IdToDropTable);
                database.Delete(command);
            }
        }
    }
}