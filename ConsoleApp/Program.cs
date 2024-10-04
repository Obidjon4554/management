using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary;

namespace ConsoleApp
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            List<Table> tables = new List<Table>();
            List<Column> columns = new List<Column>();
            List<Row> rows = new List<Row>();
            string connectionString = await ManagementService.GetValidConnectionStringAsync();
            string newDatabaseName = "NewDatabase";
            string newDbConnectionString = $"Host={ManagementService.GetHostFromConnectionString(connectionString)}; " +
                $"Database={newDatabaseName}; Username={ManagementService.GetUsernameFromConnectionString(connectionString)}; " +
                $"Password={ManagementService.GetPasswordFromConnectionString(connectionString)}";

            Console.WriteLine("\nConnection string created: " + connectionString);
            await ManagementService.CreateDatabaseAsync(connectionString, newDatabaseName);
            Console.WriteLine("Press any button to continue...");
            Console.ReadKey();

        MainMenu:
            Console.Clear();
            Console.WriteLine($"{newDatabaseName}"); 
            Console.WriteLine("1. Manage Tables");
            Console.WriteLine("2. Manage Columns");
            Console.WriteLine("3. Manage Rows");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");
            var option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    await ManagementService.TableMenu(newDbConnectionString, tables);
                    goto MainMenu;

                case 2:
                    await ManagementService.ManageColumnsMenu(newDbConnectionString, tables);
                    goto MainMenu;

                case 3:
                    await ManagementService.ManageRowsMenu(newDbConnectionString, tables);
                    goto MainMenu;

                case 4:
                    goto Exit;

                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    Console.ReadKey();
                    goto MainMenu;
            }

        Exit:
            Console.WriteLine("Exiting...");
        }
    }
}
