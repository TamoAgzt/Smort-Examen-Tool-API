using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using VistaExamenPlanner.Handler;
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Controllers
{
    public class Klas : Controller
    {
        private readonly ILogger<Klas> _logger;

        public Klas(ILogger<Klas> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("SelectAllKlasses")]
        public string SelectAllKlasses()
        {
            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand SelectAllKlasses = new MySqlCommand();

                SelectAllKlasses.CommandText = "SELECT Id, Naam FROM Klas;";

                return database.Select(SelectAllKlasses);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("SelectOneKlasFromName")]
        public string SelectOneKlas([FromBody] string naam)
        {
            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Information, $"SelectOneKlasFromName: Someone without the rights tried to select a class from the name.");
                return "403";
            }

            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand SelectOneKlas = new MySqlCommand();

                SelectOneKlas.CommandText = "SELECT * FROM Klas WHERE Naam=@Naam;";
                SelectOneKlas.Parameters.AddWithValue("@Naam", naam);

                return database.Select(SelectOneKlas);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateKlas")]
        public void CreateKlas([FromBody] CreateKlass klas)
        {
            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Information, $"CreateKlas: Someone without the rights tried to create a class.");
                return;
            }

            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand AddKlas = new MySqlCommand();

                AddKlas.CommandText = "INSERT INTO Klas (Naam, Mentor) VALUES (@naam, @mentor);";
                AddKlas.Parameters.AddWithValue("@naam", klas.naam);
                AddKlas.Parameters.AddWithValue("@mentor", klas.Mentor);

                database.Insert(AddKlas);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteKlas")]
        public void DeleteKlas([FromBody] int id)
        {
            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Information, $"DeleteKlas: Someone without the rights tried to create a class.");
                return;
            }

            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand DeleteKlas = new MySqlCommand();

                DeleteKlas.CommandText = "DELETE FROM User WHERE Klas_Id=@Id; DELETE FROM AgendaItem WHERE Klas_Id=@Id; DELETE FROM Klas WHERE Id=@Id;";
                DeleteKlas.Parameters.AddWithValue("@Id", id);

                database.Delete(DeleteKlas);
            }
        }
    }
}
