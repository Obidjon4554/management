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
        public static async Task ManageRowsMenu(string connectionString, List<Table> tables)
        {
            Console.Clear();
            Console.WriteLine("MANAGE ROWS");
            await SelectTableAsync(connectionString, tables);
        }
        public static async Task ManageRowsMenuAsync(NpgsqlConnection con, string tableName)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Managing Rows in {tableName}");
                Console.WriteLine("1. Add a Row");
                Console.WriteLine("2. View All Rows");
                Console.WriteLine("3. Update a Row");
                Console.WriteLine("4. Delete a Row");
                Console.WriteLine("5. Back to Table Management");

                Console.Write("Choose an option: ");
                if (!int.TryParse(Console.ReadLine(), out int option) || option < 1 || option > 5)
                {
                    Console.WriteLine("Invalid option. Please choose again.");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1:
                        await AddRowToTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 2:
                        await ViewRowsFromTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 3:
                        await UpdateRowInTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 4:
                        await DeleteRowFromTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 5:
                        return;
                }
            }
        }

        public static async Task AddRowToTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Adding a new row to {tableName}");

            var columns = await GetColumnNamesAsync(con, tableName);
            columns.Remove("id");

            var values = new DynamicParameters();
            foreach (var column in columns)
            {
                Console.Write($"Enter value for column '{column}': ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Value cannot be empty.");
                    return;
                }

                values.Add($"@{column}", input);
            }

            string insertCmd = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(c => $"@{c}"))})";
            await con.ExecuteAsync(insertCmd, values);

            Console.WriteLine("Row added successfully!");
        }

        public static async Task ViewRowsFromTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Viewing rows in {tableName}");
            Console.WriteLine("Rows:");
            Console.WriteLine("--------------------------------------------------");

            var rows = await con.QueryAsync($"SELECT * FROM {tableName}");
            var firstRow = rows.FirstOrDefault();
            if (firstRow != null)
            {
                foreach (var column in firstRow)
                {
                    Console.Write($"{column.Key}\t");
                }
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");

                foreach (var row in rows)
                {
                    foreach (var value in row)
                    {
                        Console.Write($"{value.Value}\t");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }

            Console.WriteLine("--------------------------------------------------");
        }

        public static async Task UpdateRowInTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            var rows = (await con.QueryAsync($"SELECT * FROM {tableName}")).ToList();

            Console.WriteLine("Rows:");
            Console.WriteLine("--------------------------------------------------");
            foreach (var row in rows)
            {
                foreach (var column in row)
                {
                    Console.Write($"{column.Key}: {column.Value}  ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------------------------");

            Console.Write("Enter the ID of the row you want to update: ");
            if (!int.TryParse(Console.ReadLine(), out int rowId))
            {
                Console.WriteLine("Invalid input. Please enter a valid row ID.");
                Console.ReadKey();
                return;
            }

            var rowToUpdate = rows.FirstOrDefault(r => r.id == rowId);
            if (rowToUpdate == null)
            {
                Console.WriteLine("Invalid row ID. Please try again.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Current values for ID {rowId}:");
            foreach (var column in rowToUpdate)
            {
                if (column.Key != "id")
                {
                    Console.WriteLine($"{column.Key}: {column.Value}");
                }
            }

            Console.Write("Enter the column name you want to update: ");
            string columnName = Console.ReadLine();

            if (!rowToUpdate.ContainsKey(columnName))
            {
                Console.WriteLine("Invalid column name. Please try again.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter the new value: ");
            string newValue = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newValue))
            {
                Console.WriteLine("New value cannot be empty.");
                return;
            }

            string updateCmd = $"UPDATE {tableName} SET {columnName} = @newValue WHERE id = @id";
            await con.ExecuteAsync(updateCmd, new { newValue, id = rowId });

            Console.WriteLine("Row updated successfully!");
        }

        public static async Task DeleteRowFromTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Deleting a row from {tableName}");

            await ViewRowsFromTableAsync(con, tableName);

            Console.Write("Enter the ID of the row you want to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int idToDelete))
            {
                Console.WriteLine("Invalid input. Please enter a valid row ID.");
                Console.ReadKey();
                return;
            }

            string deleteCmd = $"DELETE FROM {tableName} WHERE id = @id";
            await con.ExecuteAsync(deleteCmd, new { id = idToDelete });

            Console.WriteLine("Row deleted successfully!");
        }

        private static async Task<List<string>> GetColumnNamesAsync(NpgsqlConnection con, string tableName)
        {
            var columns = (await con.QueryAsync<string>($"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = @tableName", new { tableName })).ToList();
            return columns;
        }
    }
}
