using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace ClassLibrary
{
    public static partial class ManagementService
    {
        public static async Task TableMenu(string connectionString, List<Table> tables)
        {
            TableMenu:
            Console.Clear();
            Console.WriteLine("MANAGE TABLES");
            Console.WriteLine("1. Create Table");
            Console.WriteLine("2. View Tables");
            Console.WriteLine("3. Update Table");
            Console.WriteLine("4. Delete Table");
            Console.WriteLine("5. Back to Main Menu");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out var tableOption))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                return;
            }
            switch (tableOption)
            {
                case 1:
                    await CreateTableAsync(connectionString, tables);
                    goto TableMenu;
                case 2:
                    await ViewAllTablesAsync(connectionString);
                    goto TableMenu;

                case 3:
                    await UpdateTableAsync(connectionString, tables);
                    goto TableMenu;
                case 4:
                    await DeleteTableAsync(connectionString, tables);
                    goto TableMenu;
                case 5:
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    goto TableMenu;
            }
        }

        public static async Task CreateTableAsync(string connectionString, List<Table> tables)
        {
            Console.Clear();

            using (var con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();
                Console.WriteLine("ATTENTION: column id SERIAL PRIMARY KEY will be added automatically to the table!");
                Console.Write("Enter new table name: ");
                string newTable = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newTable))
                {
                    Console.WriteLine("Table name cannot be empty.");
                    return;
                }

                string createTableQuery = $"CREATE TABLE IF NOT EXISTS {newTable}(id SERIAL PRIMARY KEY)";
                int newId = tables.Count + 1;
                tables.Add(new Table { Id = newId, Name = newTable });

                await con.ExecuteAsync(createTableQuery);
                Console.WriteLine($"Table '{newTable}' created successfully.");
            }
        }

        public static async Task UpdateTableAsync(string connectionString, List<Table> tables)
        {
            Console.Clear();

            using (var con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();
                Console.WriteLine("Existing tables:");

                foreach (var table in tables)
                {
                    Console.WriteLine($"Id: {table.Id}, Name: {table.Name}");
                }

                Console.Write("Choose table ID for the update: ");
                if (!int.TryParse(Console.ReadLine(), out int tableId))
                {
                    Console.WriteLine("Invalid input. Please enter a valid table ID.");
                    return;
                }

                var tableToUpdate = tables.Find(c => c.Id == tableId);

                if (tableToUpdate == null)
                {
                    Console.WriteLine("Invalid table ID. Please try again.");
                    return;
                }

                string tableName = tableToUpdate.Name;
                Console.Write("Enter the new name for the table: ");
                string newTableName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newTableName))
                {
                    Console.WriteLine("New table name cannot be empty.");
                    return;
                }

                if (tables.Exists(t => t.Name.Equals(newTableName, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("This table already exists!");
                    return;
                }

                string updateQuery = $"ALTER TABLE {tableName} RENAME TO {newTableName};";
                await con.ExecuteAsync(updateQuery);
                tableToUpdate.Name = newTableName;
                Console.WriteLine("Table updated successfully.");
            }
        }

        public static async Task DeleteTableAsync(string connectionString, List<Table> tables)
        {
            Console.Clear();
            using (var con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();
                Console.WriteLine("Existing tables:");

                foreach (var table in tables)
                {
                    Console.WriteLine($"Id: {table.Id}, Name: {table.Name}");
                }

                Console.Write("Enter the ID of the Table to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int tableId))
                {
                    Console.WriteLine("Invalid input. Please enter a valid table ID.");
                    return;
                }

                var tableToDelete = tables.Find(c => c.Id == tableId);

                if (tableToDelete == null)
                {
                    Console.WriteLine("Invalid table ID. Please try again.");
                    return;
                }

                string deleteQuery = $"DROP TABLE IF EXISTS {tableToDelete.Name}";
                await con.ExecuteAsync(deleteQuery);
                tables.Remove(tableToDelete);
                Console.WriteLine($"Table '{tableToDelete.Name}' deleted successfully.");
            }
        }

        public static async Task ViewAllTablesAsync(string connectionString)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();

                string tablesQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";
                var tables = await con.QueryAsync<string>(tablesQuery);

                if (!tables.Any())
                {
                    Console.WriteLine("No tables found.");
                    return;
                }

                Console.Clear();
                Console.WriteLine("Tables:");
                Console.WriteLine("==========================");
                for (int i = 0; i < tables.Count(); i++)
                {
                    Console.WriteLine($"ID: {i + 1}, Name: {tables.ElementAt(i)}");
                }

                int tableId;
                do
                {
                    Console.Write("Select a table ID to view data: ");
                } while (!int.TryParse(Console.ReadLine(), out tableId) || tableId < 1 || tableId > tables.Count());

                string selectedTable = tables.ElementAt(tableId - 1);
                await ViewTableDataAsync(con, selectedTable);
            }
        }

        private static async Task ViewTableDataAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Viewing data in {tableName}");

            string selectQuery = $"SELECT * FROM {tableName}";
            var rows = await con.QueryAsync(selectQuery);

            if (rows == null || !rows.AsList().Any())
            {
                Console.WriteLine("No data found in this table.");
            }
            else
            {
                Console.WriteLine("Data:");
                foreach (var row in rows)
                {
                    foreach (var col in (IDictionary<string, object>)row)
                    {
                        Console.Write($"{col.Key}: {col.Value}  ");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }

    }
}
