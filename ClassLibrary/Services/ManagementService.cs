using System;
using System.Threading.Tasks;
using Npgsql;

namespace ClassLibrary
{
    public static partial class ManagementService
    {
        public static async Task CreateDatabaseAsync(string connectionString, string newDatabaseName)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();
                var checkDbCmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{newDatabaseName}'", con);
                var exists = await checkDbCmd.ExecuteScalarAsync();

                if (exists != null)
                {
                    Console.WriteLine($"Database '{newDatabaseName}' already exists");
                }
                else
                {
                    Console.WriteLine("Press any button to Create new Database automatically");
                    Console.ReadKey();
                    var createDbCmd = new NpgsqlCommand($"CREATE DATABASE \"{newDatabaseName}\"", con);
                    await createDbCmd.ExecuteNonQueryAsync();
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
