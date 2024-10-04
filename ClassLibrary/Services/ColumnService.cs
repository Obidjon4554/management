using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace ClassLibrary
{
    public static partial class ManagementService
    {
        public static async Task ManageColumnsMenuAsync(string connectionString, List<Table> tables)
        {
            Console.Clear();
            Console.WriteLine("MANAGE COLUMNS");
            await SelectTableForColumnAsync(connectionString,tables);
        }

        public static async Task ManageColumnsMenuAsync(NpgsqlConnection con, string tableName)
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

            var columnList = new List<Column>();
            int id = 1;

            foreach (var column in columns)
            {
                columnList.Add(new Column { Id = id++, Name = $"{column.column_name} ({column.data_type})" });
            }

            Console.WriteLine("Columns:");
            Console.WriteLine("==========================");
            foreach (var col in columnList)
            {
                Console.WriteLine($"ID: {col.Id}, Name: {col.Name}");
            }
        }


        public static async Task UpdateColumnInTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Updating a column in {tableName}");

            await ViewColumnsFromTableAsync(con, tableName);

         
            string checkQuery = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = @TableName";
            var columns = await con.QueryAsync<dynamic>(checkQuery, new { TableName = tableName });

            
            var columnDict = new Dictionary<int, string>();         
            int id = 1;
            foreach (var column in columns)
            {
                columnDict.Add(id++, column.column_name);
            }

            int columnId;
            do
            {
                Console.Write("Enter the column ID you want to update: ");
                if (!int.TryParse(Console.ReadLine(), out columnId) || !columnDict.ContainsKey(columnId))
                {
                    Console.WriteLine("Invalid column ID. Please enter a valid column ID.");
                }
            } while (!columnDict.ContainsKey(columnId));

            string oldColumnName = columnDict[columnId];

            string newColumnName;
            do
            {
                Console.Write("Enter the new column name: ");
                newColumnName = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(newColumnName))
                {
                    Console.WriteLine("Column name cannot be empty.");
                }
            } while (string.IsNullOrEmpty(newColumnName));

            Console.Write("Enter the new data type (or press Enter to keep it the same): ");
            string newDataType = Console.ReadLine()?.Trim();

            try
            {
                string renameColumnCmd = $"ALTER TABLE {tableName} RENAME COLUMN {oldColumnName} TO {newColumnName}";
                await con.ExecuteAsync(renameColumnCmd);

                if (!string.IsNullOrWhiteSpace(newDataType))
                {
                    string alterDataTypeCmd = $"ALTER TABLE {tableName} ALTER COLUMN {newColumnName} TYPE {newDataType}";
                    await con.ExecuteAsync(alterDataTypeCmd);
                }

                Console.WriteLine($"Column '{oldColumnName}' updated successfully to '{newColumnName}'!");
            }
            catch (Npgsql.PostgresException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static async Task DeleteColumnFromTableAsync(NpgsqlConnection con, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Deleting a column from {tableName}");

            await ViewColumnsFromTableAsync(con, tableName);

            string checkQuery = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = @TableName";
            var columns = await con.QueryAsync<dynamic>(checkQuery, new { TableName = tableName });

            var columnDict = new Dictionary<int, string>();
            int id = 1;
            foreach (var column in columns)
            {
                columnDict.Add(id++, column.column_name);
            }

            int columnId;
            do
            {
                Console.Write("Enter the column ID you want to delete: ");
                if (!int.TryParse(Console.ReadLine(), out columnId) || !columnDict.ContainsKey(columnId))
                {
                    Console.WriteLine("Invalid column ID. Please enter a valid column ID.");
                }
            } while (!columnDict.ContainsKey(columnId));

            string columnName = columnDict[columnId];

            try
            {
                string deleteColumnCmd = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
                await con.ExecuteAsync(deleteColumnCmd);

                Console.WriteLine($"Column '{columnName}' deleted successfully!");
            }
            catch (Npgsql.PostgresException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static async Task SelectTableForColumnAsync(string connectionString, List<Table> tables)
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
                        await ManageColumnsMenuAsync(con,selectedTable.Name);
                    }
                    else
                    {
                        Console.WriteLine("Oops! Invalid  number. Please try again.");
                        Console.ReadKey();
                    }
                }
            }
        }


    }
}
