using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace ClassLibrary
{
    public static partial class ManagementService
    {
        public static async Task ManageColumnsMenu(string connectionString, List<Table> tables)
        {
            Console.Clear();
            Console.WriteLine("MANAGE COLUMNS");
            await SelectTableAsync(connectionString, tables);
        }
        public static async Task ManageColumnsMenu(NpgsqlConnection con, string tableName)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Managing Columns in {tableName}");
                Console.WriteLine("1. Add a Column");
                Console.WriteLine("2. View All Columns");
                Console.WriteLine("3. Update a Column");
                Console.WriteLine("4. Delete a Column");
                Console.WriteLine("5. Back to Table Management");

                Console.Write("Choose an option: ");
                int option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        await AddColumnToTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 2:
                        await ViewColumnsFromTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 3:
                        await UpdateColumnInTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 4:
                        await DeleteColumnFromTableAsync(con, tableName);
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public static async Task AddColumnToTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Adding a new column to {tableName}");

            Console.Write("Enter the new column name: ");
            string columnName = Console.ReadLine();

            Console.Write("Enter the data type (e.g., text, integer): ");
            string dataType = Console.ReadLine();

            string addColumnCmd = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {dataType}";

            await con.ExecuteAsync(addColumnCmd);

            Console.WriteLine($"Column '{columnName}' added successfully!");
        }

        public static async Task ViewColumnsFromTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Viewing columns in {tableName}");

            string query = $"SELECT column_name, data_type FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = @TableName";
            var columns = await con.QueryAsync<dynamic>(query, new { TableName = tableName });

            Console.WriteLine("Columns:");
            Console.WriteLine("==========================");
            foreach (var column in columns)
            {
                Console.WriteLine($"{column.column_name} ({column.data_type})");
            }
        }

        public static async Task UpdateColumnInTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Updating a column in {tableName}");

            await ViewColumnsFromTableAsync(con, tableName);

            Console.Write("Enter the column name you want to update: ");
            string oldColumnName = Console.ReadLine();

            Console.Write("Enter the new column name: ");
            string newColumnName = Console.ReadLine();

            Console.Write("Enter the new data type (or press Enter to keep it the same): ");
            string newDataType = Console.ReadLine();

            string renameColumnCmd = $"ALTER TABLE {tableName} RENAME COLUMN {oldColumnName} TO {newColumnName}";
            await con.ExecuteAsync(renameColumnCmd);

            if (!string.IsNullOrWhiteSpace(newDataType))
            {
                string alterDataTypeCmd = $"ALTER TABLE {tableName} ALTER COLUMN {newColumnName} TYPE {newDataType}";
                await con.ExecuteAsync(alterDataTypeCmd);
            }

            Console.WriteLine($"Column '{oldColumnName}' updated successfully to '{newColumnName}'!");
        }

        public static async Task DeleteColumnFromTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Deleting a column from {tableName}");

            await ViewColumnsFromTableAsync(con, tableName);

            Console.Write("Enter the column name you want to delete: ");
            string columnName = Console.ReadLine();

            string deleteColumnCmd = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
            await con.ExecuteAsync(deleteColumnCmd);

            Console.WriteLine($"Column '{columnName}' deleted successfully!");
        }
    }
}
