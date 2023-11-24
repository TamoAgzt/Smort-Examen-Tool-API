using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Security.Claims;
using VistaExamenPlanner.Extensions;
using VistaExamenPlanner.Handler;
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Controllers;

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
            command.Parameters.AddWithValue("@Toezichthouder_Id", examen.Toezichthouders_Id);
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
            command.Parameters.AddWithValue("@Toezichthouder_Id", examen.Toezichthouders_Id);
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

    [Authorize]
    [HttpPost("ExamenInPlannen")]
    public void ExamenInPlannen(ExamenAgendaItem examenAgendaItem)
    {
        if (examenAgendaItem is null)
        {
            throw new ArgumentNullException(nameof(examenAgendaItem));
        }

        using (DatabaseHandler database = new())
        {
            MySqlCommand command = new();

            command.CommandText = "INSERT INTO Examen ( Naam_Examen, Vak_Examen, Toezichthouders_Id) VALUES (@Naam_Examen,@Vak_Examen,@Toezichthouder_Id);" +
                "INSERT INTO AgendaItem ( Klas_Id, Examen_Id, Lokaal_Id,Tijd_Begin,Tijd_Einden) VALUES (@Klas_Id, LAST_INSERT_ID(),@Lokaal_Id,@BeginTijd,@EindTijd);";

            command.Parameters.AddWithValue("@Naam_Examen", examenAgendaItem.Naam_Examen);
            command.Parameters.AddWithValue("@Vak_Examen", examenAgendaItem.Vak_Examen);
            command.Parameters.AddWithValue("@Toezichthouder_Id", examenAgendaItem.Toezichthouder_Id);
            command.Parameters.AddWithValue("@Klas_Id", examenAgendaItem.Klas_Id);
            command.Parameters.AddWithValue("@Examen_Id", examenAgendaItem.Examen_Id);
            command.Parameters.AddWithValue("@Lokaal_Id", examenAgendaItem.Lokaal_Id);
            command.Parameters.AddWithValue("@BeginTijd", examenAgendaItem.BeginTijd);
            command.Parameters.AddWithValue("@EindTijd", examenAgendaItem.EindTijd);

            database.Insert(command);
        }
    }

    [Authorize]
    [HttpGet("GetExamesForAweek")]
    public List<ExamenWeekItem>? GetExamesForAweek()
    {
        string rolUser = User.FindFirstValue("Rol");
        string UserId = User.FindFirstValue("Id");
        int KlasId = 0;

        List<ExamenWeekItem> WeekExamen = new List<ExamenWeekItem>();

        using (DatabaseHandler database = new())
        {

            _logger.Log(LogLevel.Debug, rolUser);

            if (rolUser == "1")
            {
                MySqlCommand GetUserKlas = new();
                GetUserKlas.CommandText = "SELECT klas_Id From Student WHERE Gebruikers_Id=@IdGebruikers;";
                GetUserKlas.Parameters.AddWithValue("@IdGebruikers", UserId);

                KlasId = database.GetNumber(GetUserKlas);
            }

            MySqlCommand SelectAgendaItem = new();

            if (KlasId != 0)
            {
                SelectAgendaItem.CommandText = "SELECT Klas_Id, Examen_Id, Lokaal_Id, Tijd_Begin, Tijd_Einden FROM AgendaItem WHERE Tijd_Begin >= @TimeStartWeek AND Tijd_Einden <= @TimeEndWeek AND Klas_Id=@KlasId;";
                SelectAgendaItem.Parameters.AddWithValue("@klasId", KlasId);
            }
            else
                SelectAgendaItem.CommandText = "SELECT Klas_Id, Examen_Id, Lokaal_Id, Tijd_Begin, Tijd_Einden FROM AgendaItem WHERE Tijd_Begin >= @TimeStartWeek AND Tijd_Einden <= @TimeEndWeek;";


            SelectAgendaItem.Parameters.AddWithValue("@TimeStartWeek", DateTime.Now);
            SelectAgendaItem.Parameters.AddWithValue("TimeEndWeek", DateTime.Now.AddDays(7));


            string DataAgendaItem = database.Select(SelectAgendaItem);

            AgendaItem[] agendaItemExamen = JsonConvert.DeserializeObject<AgendaItem[]>(DataAgendaItem);

            if (agendaItemExamen.Length == 0)
                return null;

            foreach (AgendaItem Item in agendaItemExamen)
            {

                MySqlCommand SelectExamen = new();

                SelectExamen.CommandText = "SELECT Naam_Examen, Vak_Examen, Toezichthouders_Id FROM Examen WHERE Id=@Id;";
                SelectExamen.Parameters.AddWithValue("@Id", Item.Examen_Id);

                Examen[] examenData = JsonConvert.DeserializeObject<Examen[]>(database.Select(SelectExamen))!;

                if (rolUser == "2")
                {
                    if (examenData[0].Toezichthouders_Id.ToString() == UserId)
                    {

                        MySqlCommand SelectToezichthouder = new();
                        SelectToezichthouder.CommandText = "SELECT Naam, Achternaam FROM Gebruikers WHERE Id=@Id;";
                        SelectToezichthouder.Parameters.AddWithValue("@Id", examenData[0].Toezichthouders_Id);

                        Gebruikers[] Toezichthouder = JsonConvert.DeserializeObject<Gebruikers[]>(database.Select(SelectToezichthouder))!;

                        WeekExamen.Add(new ExamenWeekItem()
                        {
                            agendaItem = Item,
                            examenItem = examenData[0],
                            Toezichterhouders = Toezichthouder
                        });
                    }
                }
                else
                {
                    MySqlCommand SelectToezichthouder = new();
                    SelectToezichthouder.CommandText = "SELECT Naam, Achternaam FROM Gebruikers WHERE Id=@Id;";
                    SelectToezichthouder.Parameters.AddWithValue("@Id", examenData[0].Toezichthouders_Id);

                    Gebruikers[] Toezichthouder = JsonConvert.DeserializeObject<Gebruikers[]>(database.Select(SelectToezichthouder))!;

                    WeekExamen.Add(new ExamenWeekItem()
                    {
                        agendaItem = Item,
                        examenItem = examenData[0],
                        Toezichterhouders = Toezichthouder
                    });
                }
            }
            return WeekExamen;
        }
    }

    //TODO Maak toegankelijke voor de toezichthouders
    [Authorize]
    [HttpGet("GetExamesForAMonth")]
    public List<ExamenWeekItem>? GetExamesForAMonth(int month = 0, int year = 0)
    {

        if (month == 0)
            month = DateTime.Now.Month;

        if (year == 0)
            year = DateTime.Now.Year;


        string rolUser = User.FindFirstValue("Rol");
        string UserId = User.FindFirstValue("Id");
        int KlasId = 0;

        List<ExamenWeekItem> WeekExamen = new List<ExamenWeekItem>();

        using (DatabaseHandler database = new())
        {

            _logger.Log(LogLevel.Debug, rolUser);

            if (rolUser == "1")
            {
                MySqlCommand GetUserKlas = new();
                GetUserKlas.CommandText = "SELECT klas_Id From Student WHERE Gebruikers_Id=@IdGebruikers;";
                GetUserKlas.Parameters.AddWithValue("@IdGebruikers", UserId);

                KlasId = database.GetNumber(GetUserKlas);
            }

            MySqlCommand SelectAgendaItem = new();

            if (KlasId != 0)
            {
                SelectAgendaItem.CommandText = "SELECT Klas_Id, Examen_Id, Lokaal_Id, Tijd_Begin, Tijd_Einden FROM AgendaItem WHERE Tijd_Begin >= @TimeStartWeek AND Tijd_Einden <= @TimeEndWeek AND Klas_Id=@KlasId;";
                SelectAgendaItem.Parameters.AddWithValue("@klasId", KlasId);
            }
            else
                SelectAgendaItem.CommandText = "SELECT Klas_Id, Examen_Id, Lokaal_Id, Tijd_Begin, Tijd_Einden FROM AgendaItem WHERE Tijd_Begin >= @TimeStartWeek AND Tijd_Einden <= @TimeEndWeek;";

            SelectAgendaItem.Parameters.AddWithValue("@TimeStartWeek", new DateTime(year, month, 1)) ;

            SelectAgendaItem.Parameters.AddWithValue("@TimeEndWeek", new DateTime(year, month, DateTime.DaysInMonth(year, month))) ;


            string DataAgendaItem = database.Select(SelectAgendaItem);

            AgendaItem[] agendaItemExamen = JsonConvert.DeserializeObject<AgendaItem[]>(DataAgendaItem);

            if (agendaItemExamen.Length == 0)
                return null;

            foreach (AgendaItem Item in agendaItemExamen)
            {

                MySqlCommand SelectExamen = new();

                SelectExamen.CommandText = "SELECT Naam_Examen, Vak_Examen, Toezichthouders_Id FROM Examen WHERE Id=@Id;";
                SelectExamen.Parameters.AddWithValue("@Id", Item.Examen_Id);

                Examen[] examenData = JsonConvert.DeserializeObject<Examen[]>(database.Select(SelectExamen))!;

                if (rolUser == "2")
                {
                    if (examenData[0].Toezichthouders_Id.ToString() == UserId)
                    {

                        MySqlCommand SelectToezichthouder = new();
                        SelectToezichthouder.CommandText = "SELECT Naam, Achternaam FROM Gebruikers WHERE Id=@Id;";
                        SelectToezichthouder.Parameters.AddWithValue("@Id", examenData[0].Toezichthouders_Id);

                        Gebruikers[] Toezichthouder = JsonConvert.DeserializeObject<Gebruikers[]>(database.Select(SelectToezichthouder))!;

                        WeekExamen.Add(new ExamenWeekItem()
                        {
                            agendaItem = Item,
                            examenItem = examenData[0],
                            Toezichterhouders = Toezichthouder
                        });
                    }
                }
                else
                {
                    MySqlCommand SelectToezichthouder = new();
                    SelectToezichthouder.CommandText = "SELECT Naam, Achternaam FROM Gebruikers WHERE Id=@Id;";
                    SelectToezichthouder.Parameters.AddWithValue("@Id", examenData[0].Toezichthouders_Id);

                    Gebruikers[] Toezichthouder = JsonConvert.DeserializeObject<Gebruikers[]>(database.Select(SelectToezichthouder))!;

                    WeekExamen.Add(new ExamenWeekItem()
                    {
                        agendaItem = Item,
                        examenItem = examenData[0],
                        Toezichterhouders = Toezichthouder
                    });
                }
            }
            return WeekExamen;
        }
    }
}