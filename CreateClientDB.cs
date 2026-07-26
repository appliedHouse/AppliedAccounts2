using System;
using System.Data.SqlClient;

namespace AppliedDB.CreateDB
{
	public class CreateClientDB
	{
		
        public Sqlite GetSqlConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }


        public CreateClientDB()
		{
		}

        public void CreateClientDatabase(string connectionString, string clientName)
        {
            string databaseName = $"ClientDB_{clientName}";
            CreateDatabase(connectionString, databaseName);
        }


        public void CreateDatabase(string connectionString, string databaseName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string createDbQuery = $"CREATE DATABASE [{databaseName}]";
                using (SqlCommand command = new SqlCommand(createDbQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}

