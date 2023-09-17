using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VistaExamenPlanner.Handler
{
    public class DatabaseHandler : IDisposable
    {
        public MySqlConnection Connection { get; set; }

        private readonly ILogger Logger;


        public DatabaseHandler(ILogger<DatabaseHandler> logger = null)
        {
            Logger = logger;

            string DatabaseName = "SmortTestDb" ?? Environment.GetEnvironmentVariable("DatabaseDb");
            string Password = "password" ?? Environment.GetEnvironmentVariable("PasswordDb");
            string Username = "root" ?? Environment.GetEnvironmentVariable("UsernameDb");
            string Server = "localhost" ?? Environment.GetEnvironmentVariable("HostDb");

            string connectionString = $"server={Server};port=3306;uid={Username};pwd={Password};database={DatabaseName};";

            Connection = new MySqlConnection(connectionString);

            Connection.ConnectionString = connectionString;
            Connection.Open();
        }

        public void Delete(MySqlCommand sqlCommand)
        {
            ExecuteInsertDeleteUpdate(sqlCommand, "Delete");
        }

        public void Update(MySqlCommand sqlCommand)
        {
            ExecuteInsertDeleteUpdate(sqlCommand, "Update");
        }

        public void Insert(MySqlCommand sqlCommand)
        {
            ExecuteInsertDeleteUpdate(sqlCommand, "Insert");
        }

        private void ExecuteInsertDeleteUpdate(MySqlCommand sqlCommand, string functie)
        {
            sqlCommand.Connection = Connection;
            try
            {
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"ERROR: {ex.Message} : {ex.Data} : {ex} : {functie}");
            }
        }

        public string Select(MySqlCommand sqlCommand)
        {
            sqlCommand.Connection = Connection;
            sqlCommand.Prepare();
            try
            {
                using (MySqlDataReader SelectData = sqlCommand.ExecuteReader())
                {
                    return SqlReaderToJson(SelectData);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"ERROR: {ex.Message} : {ex.Data} : {ex}");
                return "";
            }
        }

        public int GetNumber(MySqlCommand sqlCommand)
        {
            sqlCommand.Connection = Connection;
            try
            {
                using (MySqlDataReader Reader = sqlCommand.ExecuteReader())
                {
                    return sqlReaderToInt(Reader);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"message: {ex.Message}, source {ex.Source}");
                return 0;
            }
        }


        public string SqlReaderToJson(MySqlDataReader reader)
        {
            List<object> JsonList = new List<object>();
            while (reader.Read())
            {
                Dictionary<string, Object> sqlDictionary = new Dictionary<string, Object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    sqlDictionary.Add(reader.GetName(i), reader[i]);
                }
                JsonList.Add(sqlDictionary);

            }
            return JsonConvert.SerializeObject(JsonList);
        }

        public int sqlReaderToInt(MySqlDataReader reader)
        {
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }
            
            return 0;
        }

        public void Migrate(string[] sqlFileContent)
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            foreach (string query in sqlFileContent)
            {
                sqlCommand.CommandText = query;
                sqlCommand.Connection = Connection;
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
