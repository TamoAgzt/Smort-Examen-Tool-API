using Microsoft.AspNetCore.Mvc;
using VistaExamenPlanner.Objecten;
using VistaExamenPlanner.Handler;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace VistaExamenPlanner.Controllers
{
    [ApiController]
    public class Users : ControllerBase
    {
        private readonly ILogger<Users> _logger;

        public Users(ILogger<Users> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        [Route("SelectUsers")]
        public void ListUsers([FromBody] int Id)
        {
            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand SelectAllUsers = new MySqlCommand();
                SelectAllUsers.CommandText = "SELECT Id, Naam, Email, Student_Nummer FROM Student;";
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteUser")]
        public void DeleteUser([FromBody] int Id)
        {
            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand RemoveUser = new MySqlCommand();
                RemoveUser.CommandText = "DELETE FROM Student WHERE Id=@Id;";
                RemoveUser.Parameters.AddWithValue("@Id", Id);
            }
        }

        [Route("ÇreateUser")]
        [HttpPost]
        public void ÇreateUser(CreateAccountObject account)
        {
            if (account == null)
                return;

            if(string.IsNullOrEmpty(account.Email) && string.IsNullOrEmpty(account.Wachtwoord))
                return;

            string studentenNummer = RegexAndTextHandler.GetStudentNumber(account.Email!);
            string HashedWachtwoord = SecurityHandler.BcrypyBasicEncryption(account.Wachtwoord!);

            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand InsertUser = new MySqlCommand();

                InsertUser.CommandText = "INSERT INTO Student (Klas_Id, Student_Nummer, Naam, Achternaam, Email, Wachtwoord) VALUES ((SELECT Id FROM Klas WHERE Naam=@KlassNaam) , @Student_Nummer, @Naam, @Achternaam, @Email, @Wachtwoord);";
               
                InsertUser.Parameters.AddWithValue("@KlassNaam", account.klass);
                InsertUser.Parameters.AddWithValue("@Persoon_Id", account.achternaam);
                InsertUser.Parameters.AddWithValue("@Student_Nummer", studentenNummer);
                InsertUser.Parameters.AddWithValue("@Wachtwoord", HashedWachtwoord);
                InsertUser.Parameters.AddWithValue("@Naam", account.naam);
                InsertUser.Parameters.AddWithValue("@Achternaam", account.achternaam);
                InsertUser.Parameters.AddWithValue("@Email", account.Email);

                database.Insert(InsertUser);
            }
        }

        [HttpPost]
        [Route("Login")]
        public string LoginUser(LoginObject Login)
        {
            LoginObject[] LoginDataDatabase = new LoginObject[1];
            if (Login == null)
                return "";

            if (string.IsNullOrEmpty(Login.Wachtwoord) && string.IsNullOrEmpty(Login.Email))
                return "";

            using (DatabaseHandler database = new())
            {
                MySqlCommand selectUser = new MySqlCommand();
                selectUser.CommandText = "SELECT Email, Wachtwoord FROM Student WHERE Email=@Email;";
                selectUser.Parameters.AddWithValue("@Email", Login.Email);
                database.Select(selectUser);
                string UserData = database.Select(selectUser);
                LoginDataDatabase = JsonConvert.DeserializeObject<LoginObject[]>(UserData)!;
            }
            if (SecurityHandler.VerifyPassword(Login.Wachtwoord, LoginDataDatabase[0].Wachtwoord))
            {
                return JwtTokenHandler.GenerateToken("0");
            }
            return "404" ;
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateUser")]
        public void UpdateUser(UpdateUserObject updateUser)
        {
            using (DatabaseHandler database = new())
            {
                MySqlCommand selectUser = new MySqlCommand();
                selectUser.CommandText = "UPDATE Student SET Naam=@Naam, Achternaam=@Achternaam, Email=@Email WHERE Id=@Id;";
                selectUser.Parameters.AddWithValue("@Naam", updateUser.naam);
                selectUser.Parameters.AddWithValue("@Achternaam", updateUser.achternaam);
                selectUser.Parameters.AddWithValue("@Email", updateUser.email);
                selectUser.Parameters.AddWithValue("@Id", updateUser.Id);

                database.Update(selectUser);
            }
        }
    }
}