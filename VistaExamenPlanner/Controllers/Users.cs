using Microsoft.AspNetCore.Mvc;
using VistaExamenPlanner.Objecten;
using VistaExamenPlanner.Handler;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [HttpPost]
        [Route("SelectUsers")]
        public void ListUsers([FromBody] int Id)
        {
            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Information, $"SelectUsers: Someone without the rights tried to select a user.");
                return;
            }

            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand SelectAllUsers = new MySqlCommand();
                SelectAllUsers.CommandText = "SELECT Id, Naam, Email, FROM Gebruikers;";
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteUser")]
        public string DeleteUser([FromBody] int Id)
        {
            if (Id == 1)
            {
                _logger.Log(LogLevel.Information, $"DeleteUser: The admin account can not be deleted.");
                return "User Cant be deleted";
            }

            string Rol = User.FindFirstValue("Rol");
            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Warning, $"DeleteUser: Someone without the rights tried to delete a user.");
                return "No Perms";
            }

            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand RemoveUser = new MySqlCommand();
                RemoveUser.CommandText = "DELETE FROM Gebruikers WHERE Id=@Id;";
                RemoveUser.Parameters.AddWithValue("@Id", Id);
                database.Delete(RemoveUser);
            }
            _logger.Log(LogLevel.Information, $"DeleteUser: User Deleted By Admin");
            return "BYE BYE";
        }

        [Route("ÇreateUser")]
        [HttpPost]
        public void ÇreateUser(CreateAccountObject account)
        {
            if (account == null)
            {
                _logger.Log(LogLevel.Warning, $"ÇreateUser: No valid data received.");
                return;
            }

            if (string.IsNullOrEmpty(account.Email) && string.IsNullOrEmpty(account.Wachtwoord))
            {
                _logger.Log(LogLevel.Warning, $"ÇreateUser: No Email or Password received.");
                return;
            }

            if (!RegexAndTextHandler.IsVistaEmail(account.Email!))
            {
                _logger.Log(LogLevel.Warning, $"ÇreateUser: Someone tried to create an account with an none vista email.");
                return;
            }

            string HashedWachtwoord = SecurityHandler.BcrypyBasicEncryption(account.Wachtwoord!);

            Rol NewUserRol = Rol.Student;

            if (!RegexAndTextHandler.IsStudent(account.Email!))
            {
                NewUserRol = Rol.Docent;
            }

            using (DatabaseHandler database = new DatabaseHandler())
            {
                //is the email already used
                using (MySqlCommand IsTheEmailAlreadyUsed = new MySqlCommand())
                {
                    IsTheEmailAlreadyUsed.CommandText = "SELECT Email FROM Gebruikers WHERE Email=@Email;";
                    IsTheEmailAlreadyUsed.Parameters.AddWithValue("@Email", account.Email);

                    string EmailList = database.Select(IsTheEmailAlreadyUsed);

                    if (EmailList != "[]")
                    {
                        _logger.Log(LogLevel.Warning, $"ÇreateUser: Someone tried to create an account with an email that already is in use.");
                        return;
                    }
                }


                //Create User
                using (MySqlCommand InsertUser = new MySqlCommand())
                {
                    InsertUser.CommandText = "INSERT INTO Gebruikers (Rol_Id, Naam, Achternaam, Email, Wachtwoord) VALUES (@Rol, @Naam, @Achternaam, @Email, @Wachtwoord);";

                    InsertUser.Parameters.AddWithValue("@Rol", NewUserRol);
                    InsertUser.Parameters.AddWithValue("@Wachtwoord", HashedWachtwoord);
                    InsertUser.Parameters.AddWithValue("@Naam", account.naam);
                    InsertUser.Parameters.AddWithValue("@Achternaam", account.achternaam);
                    InsertUser.Parameters.AddWithValue("@Email", account.Email);

                    database.Insert(InsertUser);
                }

                _logger.Log(LogLevel.Information, "New Account Created");
            }
        }

        [HttpPost]
        [Route("Login")]
        public LoginReturnData? LoginUser(LoginObject Login)
        {
            string UserDataSelect = "";

            if (Login == null)
            {
                _logger.Log(LogLevel.Warning, $"Login: No LoginData Was received.");
                return null;
            }

            if (string.IsNullOrEmpty(Login.Wachtwoord) && string.IsNullOrEmpty(Login.Email))
            {
                _logger.Log(LogLevel.Warning, $"Login: No LoginData Was received.");
                return null;
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand selectUser = new MySqlCommand();
                selectUser.CommandText = "SELECT Id, Email, Wachtwoord, Rol_Id FROM Gebruikers WHERE Email=@Email;";
                selectUser.Parameters.AddWithValue("@Email", Login.Email);
                database.Select(selectUser);
                UserDataSelect = database.Select(selectUser);
            }

            UserData[] LoginDataDatabase = JsonConvert.DeserializeObject<UserData[]>(UserDataSelect)!;


            if (SecurityHandler.VerifyPassword(Login.Wachtwoord, LoginDataDatabase[0].Wachtwoord))
            {
                return new LoginReturnData()
                {
                    Token = JwtTokenHandler.GenerateToken(LoginDataDatabase[0].Rol_Id, LoginDataDatabase[0].Id),
                    Rol = LoginDataDatabase[0].Rol_Id,  
                };
            }

            _logger.Log(LogLevel.Information, $"Login: User with Id:{LoginDataDatabase[0].Id}  and rol:{(Rol)LoginDataDatabase[0].Rol_Id} has logged in.");

            return null;
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateUser")]
        public string UpdateUser(UpdateUserObject updateUser)
        {
            string Rol = User.FindFirstValue("Rol");
            int Id = 0;

            if (!Int32.TryParse(User.FindFirstValue("Id"), out Id)) {
                _logger.Log(LogLevel.Warning, $"UpdateUser: No Valid Id was given. Given Id {Id}.");
                return "No Valid Id";
            }

            if (Rol == "" || Rol != "3")
            {
                _logger.Log(LogLevel.Warning, $"UpdateUser: Someone without the rights Tried to use this api call.");
                return "No perms";
            }

            using (DatabaseHandler database = new())
            {
                MySqlCommand selectUser = new MySqlCommand();
                selectUser.CommandText = "UPDATE Gebruikers SET Naam=@Naam, Achternaam=@Achternaam, Email=@Email WHERE Id=@Id;";
                selectUser.Parameters.AddWithValue("@Naam", updateUser.naam);
                selectUser.Parameters.AddWithValue("@Achternaam", updateUser.achternaam);
                selectUser.Parameters.AddWithValue("@Email", updateUser.email);
                selectUser.Parameters.AddWithValue("@Id", Id);

                database.Update(selectUser);
            }

            _logger.Log(LogLevel.Information, $"User Id:{Id} Has Changed his/her account data.");

            return "Updated";
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateUserPassword")]
        public string UpdateUserPassword([FromBody] string Wachtwoord)
        {
            if (string.IsNullOrEmpty(Wachtwoord))
                return "No Valid New Password";

            int Id = 0;

            if (!Int32.TryParse(User.FindFirstValue("Id"), out Id))
            {
                _logger.Log(LogLevel.Warning, $"UpdateUserPassword: No Valid Id was given. Given Id {Id}.");
                return "No Valid Id";
            }
            string HashedWachtwoord = SecurityHandler.BcrypyBasicEncryption(Wachtwoord!);

            using (DatabaseHandler database = new())
            {
                MySqlCommand selectUser = new MySqlCommand();
                selectUser.CommandText = "UPDATE Gebruikers SET Wachtwoord=@Wachtwoord WHERE Id=@Id;";
                selectUser.Parameters.AddWithValue("@Wachtwoord", HashedWachtwoord);
                selectUser.Parameters.AddWithValue("@Id", Id);

                database.Update(selectUser);
            }
            _logger.Log(LogLevel.Information, $"UpdateUserPassword: User Id:{Id} Has Changed his/her password.");

            return "Updated";

        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteAccount")]
        public string DeleteAccount()
        {
            string Id = User.FindFirstValue("Id");

            if (Id == "1")
                return "User Cant be deleted";

            using (DatabaseHandler database = new DatabaseHandler())
            {
                MySqlCommand RemoveUser = new MySqlCommand();
                RemoveUser.CommandText = "DELETE FROM Gebruikers WHERE Id=@Id;";
                RemoveUser.Parameters.AddWithValue("@Id", Id);
                database.Delete(RemoveUser);
            }
            _logger.Log(LogLevel.Information, $"DeleteAccount: User Deleted His/her Account.");

            return "BYE BYE";
        }
    }
}