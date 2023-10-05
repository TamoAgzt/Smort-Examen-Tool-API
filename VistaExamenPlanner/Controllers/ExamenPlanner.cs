using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using VistaExamenPlanner.Handler;
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Controllers
{
    [ApiController]
    public class ExamenPlanner : ControllerBase
    {
        private readonly ILogger<ExamenPlanner> _logger;

        public ExamenPlanner(ILogger<ExamenPlanner> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("GetExamens")]
        public string GetExamensList()
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = "SELECT * FROM Examen";
                var result = database.Select(command);
                return result;
            }
        }

        [HttpGet("GetExamensByClassId")]
        public string GetExamensByClassId(string ClassId)
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = "SELECT Examen.Id, Examen.Naam_Examen, Examen.Vak_Examen , Examen.Toezichthouders_Id  FROM Examen , Klas  WHERE Klas.Id = @ClassId ; ";
                command.Parameters.AddWithValue("@ClassId", ClassId);
                var result = database.Select(command);
                return result;
            }
        }

        [Authorize]
        [HttpPost("AddExamen")]
        public void AddExamen(Examen examen)
        {
            if (examen is null)
            {
                throw new ArgumentNullException(nameof(examen));
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = $"INSERT INTO Examen ( Naam_Examen, Vak_Examen, Toezichthouders_Id) VALUES (@Naam_Examen,@Vak_Examen,@Toezichthouder_Id);";
                command.Parameters.AddWithValue("@Naam_Examen", examen.Naam_Examen);
                command.Parameters.AddWithValue("@Vak_Examen", examen.Vak_Examen);
                command.Parameters.AddWithValue("@Toezichthouder_Id", examen.Toezichthouder_Id);
                database.Insert(command);
            }
        }

        [Authorize]
        [HttpPut("UpdateExamen")]
        public void UpdateExamen(int IdToUpdate, Examen examen)
        {
            if (examen is null)
            {
                throw new ArgumentNullException(nameof(examen));
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new MySqlCommand();

                command.CommandText = "UPDATE Examen SET Naam_Examen = @Naam_Examen, Vak_Examen = @Vak_Examen, Toezichthouders_Id = @Toezichthouder_Id WHERE Id = @IdToUpdate; ";
                command.Parameters.AddWithValue("@Naam_Examen", examen.Naam_Examen);
                command.Parameters.AddWithValue("@Vak_Examen", examen.Vak_Examen);
                command.Parameters.AddWithValue("@Toezichthouder_Id", examen.Toezichthouder_Id);
                command.Parameters.AddWithValue("@IdToUpdate", IdToUpdate);
                database.Update(command);
            }
        }

        [Authorize]
        [HttpPost("DeleteExamen")]
        public void DeleteExamen(int IdToDropTable)
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new MySqlCommand();

                command.CommandText = $"DELETE FROM Examen WHERE Id = @IdToDropTable;";
                command.Parameters.AddWithValue("@IdToDropTable", IdToDropTable);
                database.Delete(command);
            }
        }
    }
}