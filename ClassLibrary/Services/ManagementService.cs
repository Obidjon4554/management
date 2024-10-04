using System;
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

        public static async Task<string> GetValidConnectionStringAsync()
        {
            while (true)
            {
                Console.Write("Enter Host (e.g., localhost): ");
                string host = Console.ReadLine();
                Console.Write("Enter Username (e.g., postgres): ");
                string username = Console.ReadLine();
                Console.Write("Enter Password: ");
                string password = ReadPassword();

                string connectionString = $"Host={host}; Username={username}; Password={password}";

                if (await IsConnectionValidAsync(connectionString))
                {
                    return connectionString;
                }
                else
                {
                    Console.WriteLine("\nInvalid connection string or unable to connect to the database. Please try again.\n");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        public static async Task<bool> IsConnectionValidAsync(string connectionString)
        {
            try
            {
                using (var con = new NpgsqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    await con.CloseAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                return false;
            }
        }

        public static string GetHostFromConnectionString(string connectionString)
        {
            var hostPart = connectionString.Split(';')[0];
            return hostPart.Split('=')[1];
        }

        public static string GetUsernameFromConnectionString(string connectionString)
        {
            var usernamePart = connectionString.Split(';')[1];
            return usernamePart.Split('=')[1];
        }

        public static string GetPasswordFromConnectionString(string connectionString)
        {
            var passwordPart = connectionString.Split(';')[2];
            return passwordPart.Split('=')[1];
        }
    }
}
