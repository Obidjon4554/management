﻿using System;
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
             await  ManagementService.CreateDatabaseAsync(connectionString, newDatabaseName);
            Console.WriteLine("Press any button to continue...");
            Console.ReadKey();

        Menu:
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
                    await ManagementService.CreateTableAsync(newDbConnectionString, tables);
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                    goto Menu;

                case 2:
                    await ManagementService.SelectTableAsync(newDbConnectionString, tables);
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                    goto Menu;

                case 3:
                    await ManagementService.UpdateTableAsync(newDbConnectionString, tables);
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                    goto Menu;

                case 4:
                    await ManagementService.DeleteTableAsync(newDbConnectionString, tables);
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                    goto Menu;

                case 5:
                    goto Exit;

                default:
                    Console.WriteLine("Error 404");
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                    goto Menu;
            }

        Exit:
            Console.WriteLine("Press any button to exit...");
            Console.ReadKey();
        }
    }

}
