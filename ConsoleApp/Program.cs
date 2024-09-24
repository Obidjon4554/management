using System;
using System.Collections.Generic;
using ClassLibrary;
using Npgsql;

namespace ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Table> tables = new List<Table>();
            Console.Write("Enter Host (e.g., localhost): ");
            string host = Console.ReadLine();
            Console.Write("Enter Username (e.g., postgres): ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            string connectionString = $"Host={host}; Username={username}; Password={password}";
            var exit = true;
            string newDatabaseName = "NewpDatabase";
            string newDbConnectionString = $"Host={host}; Database={newDatabaseName}; Username={username}; Password={password}";
            Console.WriteLine("Connection string created: " + connectionString);
            CreateDatabase(connectionString, newDatabaseName);
            Console.WriteLine("Press any button to continue...");
            Console.ReadKey();

            while (exit)
            {
                Console.Clear();
                Console.WriteLine("WELCOME");
                Console.WriteLine("1. Create Table");
                Console.WriteLine("2. Select Table");
                Console.WriteLine("3. Update Table");
                Console.WriteLine("4. Delete Table");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                var temp = int.Parse(Console.ReadLine());

                switch (temp)
                {
                    case 1:
                        CreateTable(newDbConnectionString, tables);
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
                        break;
                    case 2:
                        SelectTable(newDbConnectionString, tables);
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
                        break;
                    case 3:
                        //InsertIntoTable(newDbConnectionString, categories);
                        UpdateTable(newDbConnectionString, tables);
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
                        break;
                    case 4:
                        DeleteTable(newDbConnectionString, tables);
                        //  UpdateCategory(newDbConnectionString, categories);
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
                        break;
                    case 5:
                        Console.WriteLine("Press any button to exit...");
                        Console.ReadKey();
                        exit = false;
                        break;
                    default:
                        Console.WriteLine("Error 404");
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
                        exit = false;
                        break;
                }
            }
        }

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

        public static void CreateTable(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                Console.WriteLine("ATTENTION: column id SERIAL PRIMARY KEY will be added authomatically to the table!");
                Console.Write("Enter new table name: ");
                string newTable = Console.ReadLine();
                var createTable = new NpgsqlCommand(
                    $"CREATE TABLE IF NOT EXISTS {newTable}(id SERIAL PRIMARY KEY)", con);
                int newId = tables.Count + 1; 
                tables.Add(new Table { Id = newId, Name = newTable });
                createTable.ExecuteNonQuery();
                Console.WriteLine($"Table '{newTable}' created successfully.");
                con.Close();
                SelectTable(connectionString, tables);
            }
        }
        public static void SelectTable(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                tables.Clear();
                using (var cmd = new NpgsqlCommand("SELECT table_name\r\nFROM INFORMATION_SCHEMA.TABLES\r\nWHERE table_schema = 'public'", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            count++;
                            tables.Add(new Table { Id = count, Name = name });
                        }
                    }
                }

                Console.WriteLine("Existing tables:");
                foreach (var table in tables)
                {
                    Console.WriteLine($"Id: {table.Id}, Name {table.Name}");
                }

                con.Close();
            }
        }

        public static void UpdateTable(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                Console.WriteLine("Existing tables:");

                foreach (var table in tables)
                {
                    Console.WriteLine($"Id: {table.Id}, Name {table.Name}");
                }

                Console.Write("Choose table ID for the update:");
                int tableId = int.Parse(Console.ReadLine());
                var tableToUpdate = tables.Find(c => c.Id == tableId);

                if (tableToUpdate == null)
                {
                    Console.WriteLine("Invalid table ID. Please try again.");
                    return;
                }

                string tableName = tableToUpdate.Name;
                Console.Write("Enter the new name for the table: ");
                string newTableName = Console.ReadLine();

                foreach (var table in tables)
                {
                    if (table.Name == newTableName)
                    {
                        Console.WriteLine("This table already exists!");
                        return;
                    }
                }

                var updateCmd = new NpgsqlCommand($"ALTER TABLE {tableName} RENAME TO {newTableName};", con);
                updateCmd.ExecuteNonQuery();
                tableToUpdate.Name = newTableName;
                Console.WriteLine("Table updated successfuly");

                con.Close();
            }
        }

        public static void DeleteTable(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                Console.WriteLine("Existing tables:");

                foreach (var table in tables)
                {
                    Console.WriteLine($"Id: {table.Id}, Name {table.Name}");
                }

                Console.Write("Enter the ID of the Table to delete: ");
                int tableId = int.Parse(Console.ReadLine());
                var tableToDelete = tables.Find(c => c.Id == tableId);

                if (tableToDelete == null)
                {
                    Console.WriteLine("Invalid table ID. Please try again.");
                    return;
                }

                var cmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tableToDelete.Name}", con);
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Table '{tableToDelete.Name}' deleted successfully.");
                tables.Remove(tableToDelete);
                con.Close();
            }
        }
    }
}