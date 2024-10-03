using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace ClassLibrary
{
    public static partial class ManagementService
    {
        public static async Task TableMenu(string connectionString, List<Table> tables)
        {
            Console.Clear();
            Console.WriteLine("MANAGE TABLES");
            Console.WriteLine("1. Create Table");
            Console.WriteLine("2. Select Table");
            Console.WriteLine("3. Update Table");
            Console.WriteLine("4. Delete Table");
            Console.WriteLine("5. Back to Main Menu");
            Console.Write("Choose an option: ");
            var tableOption = int.Parse(Console.ReadLine());

            switch (tableOption)
            {
                case 1:
                    await CreateTableAsync(connectionString, tables);
                    break;

                case 2:
                    await SelectTableAsync(connectionString, tables);
                    break;

                case 3:
                    await UpdateTableAsync(connectionString, tables);
                    break;

                case 4:
                    await DeleteTableAsync(connectionString, tables);
                    break;

                case 5:
                    return;

                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
        }
        public static async Task CreateTableAsync(string connectionString, List<Table> tables)
        {
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
                await SelectTableAsync(connectionString, tables);
            }
        }

        public static async Task UpdateTableAsync(string connectionString, List<Table> tables)
        {
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

                foreach (var table in tables)
                {
                    if (table.Name.Equals(newTableName, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("This table already exists!");
                        return;
                    }
                }

                string updateQuery = $"ALTER TABLE {tableName} RENAME TO {newTableName};";
                await con.ExecuteAsync(updateQuery);
                tableToUpdate.Name = newTableName;
                Console.WriteLine("Table updated successfully.");
            }
        }

        public static async Task DeleteTableAsync(string connectionString, List<Table> tables)
        {
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

        public static async Task SelectTableAsync(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();
                tables.Clear();
                string selectQuery = "SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = 'public'";
                var tableNames = await con.QueryAsync<string>(selectQuery);

                int count = 0;
                foreach (var name in tableNames)
                {
                    count++;
                    tables.Add(new Table { Id = count, Name = name });
                }

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine(" List of Tables ");
                    Console.WriteLine("====================");

                    foreach (var table in tables)
                    {
                        Console.WriteLine($"{table.Id}. {table.Name}");
                    }

                    Console.WriteLine($"{tables.Count + 1}. Back to Main Menu");
                    Console.Write("Please choose a table by its number: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > tables.Count + 1)
                    {
                        Console.WriteLine("Oops! Invalid table number. Please try again.");
                        Console.ReadKey();
                        continue;
                    }

                    if (choice == tables.Count + 1)
                    {
                        return;
                    }

                    var selectedTable = tables.Find(c => c.Id == choice);
                    if (selectedTable != null)
                    {
                        await TableCrudMenuAsync(con, selectedTable.Name);
                    }
                    else
                    {
                        Console.WriteLine("Oops! Invalid table number. Please try again.");
                        Console.ReadKey();
                    }
                }
            }
        }

        public static async Task TableCrudMenuAsync(NpgsqlConnection con, string tableName)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Managing Table: {tableName}");
                Console.WriteLine("1. View Table");
                Console.WriteLine("2. Manage Columns");
                Console.WriteLine("3. Manage Rows");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int option) || option < 1 || option > 4)
                {
                    Console.WriteLine("Invalid option. Please choose again.");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1:
                        await ViewTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 2:
                        await ManageColumnsMenu(con, tableName);
                        break;
                    case 3:
                        await ManageRowsMenuAsync(con, tableName);
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public static async Task ViewTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Viewing data in {tableName}");

            string selectQuery = $"SELECT * FROM {tableName}";
            var rows = await con.QueryAsync(selectQuery);

            Console.WriteLine("Columns:");
            foreach (var row in rows)
            {
                foreach (var col in (IDictionary<string, object>)row)
                {
                    Console.Write($"{col.Key}: {col.Value}  ");
                }
                Console.WriteLine();
            }
        }
    }
}
