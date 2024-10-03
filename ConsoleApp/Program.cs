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
            Console.Write("Enter Host (e.g., localhost): ");
            string host = Console.ReadLine();
            Console.Write("Enter Username (e.g., postgres): ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = ManagementService.ReadPassword();
            string connectionString = $"Host={host}; Username={username}; Password={password}";
            string newDatabaseName = "NewDatabase";
            string newDbConnectionString = $"Host={host}; Database={newDatabaseName}; Username={username}; Password={password}";

            Console.WriteLine("\nConnection string created: " + connectionString);
            await ManagementService.CreateDatabaseAsync(connectionString, newDatabaseName);
            Console.WriteLine("Press any button to continue...");
            Console.ReadKey();

        MainMenu:
            Console.Clear();
            Console.WriteLine("WELCOME TO YOUR DATABASE");
            Console.WriteLine("1. Manage Tables");
            Console.WriteLine("2. Manage Columns");
            Console.WriteLine("3. Manage Rows");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");
            var option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    await TableMenu(newDbConnectionString, tables);
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
            Console.ReadKey();
        }


        
    }
}
