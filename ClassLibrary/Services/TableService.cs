using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace ClassLibrary
{
    public static partial class ManagementService
    {
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

                var createTable = new NpgsqlCommand($"CREATE TABLE IF NOT EXISTS {newTable}(id SERIAL PRIMARY KEY)", con);
                int newId = tables.Count + 1;
                tables.Add(new Table { Id = newId, Name = newTable });
                await createTable.ExecuteNonQueryAsync();
                Console.WriteLine($"Table '{newTable}' created successfully.");
                await con.CloseAsync();
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
                    Console.WriteLine($"Id: {table.Id}, Name {table.Name}");
                }

                Console.Write("Choose table ID for the update:");
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

                var updateCmd = new NpgsqlCommand($"ALTER TABLE {tableName} RENAME TO {newTableName};", con);
                await updateCmd.ExecuteNonQueryAsync();
                tableToUpdate.Name = newTableName;
                Console.WriteLine("Table updated successfully");

                await con.CloseAsync();
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
                    Console.WriteLine($"Id: {table.Id}, Name {table.Name}");
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

                var cmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tableToDelete.Name}", con);
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Table '{tableToDelete.Name}' deleted successfully.");
                tables.Remove(tableToDelete);
                await con.OpenAsync();
            }
        }

        public static async Task SelectTableAsync(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();
                tables.Clear();
                using (var cmd = new NpgsqlCommand("SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = 'public'", con))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        int count = 0;
                        while (await reader.ReadAsync())
                        {
                            string name = reader.GetString(0);
                            count++;
                            tables.Add(new Table { Id = count, Name = name });
                        }
                    }
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
                        ManageColumnsMenu(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 3:
                        await ManageRowsMenuAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
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

            using (var cmd = new NpgsqlCommand($"SELECT * FROM {tableName}", con))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var schemaTable = await reader.GetSchemaTableAsync();
                    if (schemaTable != null)
                    {
                        Console.WriteLine("Columns:");
                        foreach (DataRow row in schemaTable.Rows)
                        {
                            Console.Write($"{row["ColumnName"]}  ");
                        }
                        Console.WriteLine("\n-------------------------------------");

                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write($"{reader[i]}  ");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }

        }
    }
}
