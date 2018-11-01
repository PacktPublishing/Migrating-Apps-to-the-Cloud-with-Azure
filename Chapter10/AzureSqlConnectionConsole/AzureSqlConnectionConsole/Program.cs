using System;
using System.Data;
using System.Data.SqlClient;

namespace AzureSqlConnectionConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection("Server=tcp:[your server].database.windows.net,1433;" +
                "Initial Catalog=migrationdb;Persist Security Info=False;" +
                "User ID=[your username];Password=[your password];" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            using (SqlCommand command = new SqlCommand("SELECT Id, FirstName, LastName FROM Person", connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]} - {reader["FirstName"]} {reader["LastName"]}");
                    }
                }
            }
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
