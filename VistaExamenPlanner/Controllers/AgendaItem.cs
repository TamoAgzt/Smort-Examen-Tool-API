using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using VistaExamenPlanner.Handler;
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Controllers
{
    [ApiController]
    public class AgendaItemPlanner : ControllerBase
    {
        private readonly ILogger<AgendaItemPlanner> _logger;

        public AgendaItemPlanner(ILogger<AgendaItemPlanner> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetAgendaItem")]
        public string GetAgendaItem()
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = "SELECT * FROM AgendaItem";
                var result = database.Select(command);
                return result;
            }
        }

        [Authorize]
        [HttpPost("AddAgendaItem")]
        public void AddAgendaItem(AgendaItem agendaItem)
        {
            if (agendaItem is null)
            {
                throw new ArgumentNullException(nameof(agendaItem));
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new();

                command.CommandText = $"INSERT INTO AgendaItem ( Klas_Id, Examen_Id, Lokaal_Id,Tijd_Begin,Tijd_Einden) VALUES (@Klas_Id,@Examen_Id,@Lokaal_Id,@BeginTijd,@EindTijd);";
                command.Parameters.AddWithValue("@Klas_Id", agendaItem.Klas_Id);
                command.Parameters.AddWithValue("@Examen_Id", agendaItem.Examen_Id);
                command.Parameters.AddWithValue("@Lokaal_Id", agendaItem.Lokaal_Id);
                command.Parameters.AddWithValue("@BeginTijd", agendaItem.Tijd_Begin);
                command.Parameters.AddWithValue("@EindTijd", agendaItem.Tijd_Einden);
                database.Insert(command);
            }
        }

        [Authorize]
        [HttpPut("UpdateAgendaItem")]
        public void UpdateAgendaItem(int IdToUpdate, AgendaItem agendaItem)
        {
            if (agendaItem is null)
            {
                throw new ArgumentNullException(nameof(agendaItem));
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new MySqlCommand();

                command.CommandText = $"UPDATE AgendaItem SET Klas_Id = @Klas_Id, Examen_Id = @Examen_Id, Lokaal_Id = @Lokaal_Id,Tijd_Begin = @BeginTijd, Tijd_Einden = @EindTijd WHERE Id = @IdToUpdate; ";
                command.Parameters.AddWithValue("@Klas_Id", agendaItem.Klas_Id);
                command.Parameters.AddWithValue("@Examen_Id", agendaItem.Examen_Id);
                command.Parameters.AddWithValue("@Lokaal_Id", agendaItem.Lokaal_Id);
                command.Parameters.AddWithValue("@BeginTijd", agendaItem.Tijd_Begin);
                command.Parameters.AddWithValue("@EindTijd", agendaItem.Tijd_Einden);
                command.Parameters.AddWithValue("@IdToUpdate", IdToUpdate);
                database.Update(command);
            }
        }

        [Authorize]
        [HttpPost("DeleteAgendaItem")]
        public void DeleteAgendaItem(int IdToDropTable)
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand command = new MySqlCommand();

                command.CommandText = $"DELETE FROM AgendaItem WHERE Id = @IdToDropTable;";
                command.Parameters.AddWithValue("@IdToDropTable", IdToDropTable);
                database.Delete(command);
            }
        }

    }
}