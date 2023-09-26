using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
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

        [HttpGet("GetLokaal")]
        public string GetLokaal()
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.Connection = database.Connection;
                command.CommandText = "SELECT * FROM Lokaal";
                var result = database.Select(command);
                return result;
            }
        }

        [HttpPost("AddLokaal")]
        public void AddLokaal(Lokaal lokaal)
        {
            if (lokaal is null)
            {
                throw new ArgumentNullException(nameof(lokaal));
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();
                command.Connection = database.Connection;
                command.CommandText = $"INSERT INTO Lokaal (Lokaal) VALUES (@ExamenLokaal);";
                command.Parameters.AddWithValue("@ExamenLokaal", lokaal.ExamenLokaal);
                database.Insert(command);
            }
        }

        [HttpPost("DeleteLokaal")]
        public void DeleteLokaal(int IdToDropTable)
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new MySqlCommand();

                command.Connection = database.Connection;
                command.CommandText = $"DELETE FROM Lokaal WHERE Id = @IdToDropTable;";
                command.Parameters.AddWithValue("@IdToDropTable", IdToDropTable);
                database.Update(command);
            }
        }
    }
}