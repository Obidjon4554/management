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

    }
}
