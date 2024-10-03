using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
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

                string checkDbQuery = "SELECT 1 FROM pg_database WHERE datname = @DatabaseName";
                var exists = await con.QuerySingleOrDefaultAsync<int?>(checkDbQuery, new { DatabaseName = newDatabaseName });

                if (exists != null)
                {
                    Console.WriteLine($"Database '{newDatabaseName}' already exists.");
                }
                else
                {
                    Console.WriteLine("Press any button to create a new database.");
                    Console.ReadKey();

                    string createDbQuery = $"CREATE DATABASE \"{newDatabaseName}\"";
                    await con.ExecuteAsync(createDbQuery);
                    Console.WriteLine($"Database '{newDatabaseName}' created successfully.");
                }
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
