using System;
using Npgsql;

namespace ClassLibrary
{
    public static partial class ManagementService
    {
        public static void CreateDatabase(string connectionString, string newDatabaseName)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                var checkDbCmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{newDatabaseName}'", con);
                var exists = checkDbCmd.ExecuteScalar();

                if (exists != null)
                {
                    Console.WriteLine($"Database '{newDatabaseName}' already exists");
                }
                else
                {
                    Console.WriteLine("Press any button to Create new Database automatically");
                    Console.ReadKey();
                    var createDbCmd = new NpgsqlCommand($"CREATE DATABASE \"{newDatabaseName}\"", con);
                    createDbCmd.ExecuteNonQuery();
                    Console.WriteLine($"Database '{newDatabaseName}' created successfully.");
                }
                con.Close();
            }
        }
        public static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }


    }
}
