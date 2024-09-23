using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Xml.Linq;
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
               /* Console.WriteLine("5. Update Products");
                Console.WriteLine("6. Delete Categories");
                Console.WriteLine("7. Delete Products");*/
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
                      //  UpdateProduct(newDbConnectionString, products, categories);
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
                        break;
                    case 6:
                      //  DeleteCategory(newDbConnectionString, tables);
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
                        break;
                    case 7:
                      //  DeleteProduct(newDbConnectionString, products);
                        Console.WriteLine("Press any button to continue...");
                        Console.ReadKey();
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

                createTable.ExecuteNonQuery();
                Console.WriteLine($"Table '{newTable}' created successfully.");
                con.Close();
            }
        }
        public static void SelectTable(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand("SELECT table_name\r\nFROM INFORMATION_SCHEMA.TABLES\r\nWHERE table_schema = 'public'", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            count++;
                            tables.Add(new Table {Id=count, Name = name });
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

                using (var cmd = new NpgsqlCommand("SELECT table_name\r\nFROM INFORMATION_SCHEMA.TABLES\r\nWHERE table_schema = 'public'", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                        }
                    }
                }
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

                    using (var updateCmd = new NpgsqlCommand($"ALTER TABLE {tableName} RENAME TO {newTableName};", con))
                {
                    updateCmd.Parameters.AddWithValue("@Name", newTableName);
                    updateCmd.Parameters.AddWithValue("@Id", tableId);

                    Console.WriteLine("Table updated successfuly");
                }

                tableToUpdate.Name = newTableName;

                con.Close();
            }

        }

        public static void DeleteTable(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();

                
                using (var cmd = new NpgsqlCommand("SELECT table_name\r\nFROM INFORMATION_SCHEMA.TABLES\r\nWHERE table_schema = 'public'", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                        }
                    }
                }
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

                using (var cmd = new NpgsqlCommand($"DROP TABLE {tableToDelete.Name}", con))
                {
                    cmd.Parameters.AddWithValue("@Id", tableId);
                    cmd.Parameters.AddWithValue("@Name", tableToDelete.Name);
                    

                        Console.WriteLine($"Table {tableToDelete.Name} deleted successfully.");
                }

                con.Close();
            }
        }
/*        public static void InsertColIntoTable(string connectionString, List<Table> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                Console.Write("Enter new column: ");
                var category_name = Console.ReadLine();

                using (var cm = new NpgsqlCommand("INSERT INTO categories (name) VALUES (@name) RETURNING id", con))
                {
                    cm.Parameters.AddWithValue("name", category_name);
                    var categoryId = (int)cm.ExecuteScalar();
                    categories.Add(new Category { Id = categoryId, Name = category_name });
                    Console.WriteLine("Successfully inserted into categories!");
                }

                categories.Clear();
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            categories.Add(new Category { Id = id, Name = name });
                        }
                    }
                }

                Console.WriteLine("Available categories:");
                foreach (var item in categories)
                {
                    Console.WriteLine($"ID: {item.Id}, Name: {item.Name}");
                }

                Console.Write("Enter new product: ");
                var product_name = Console.ReadLine();

                foreach (var item in categories)
                    Console.WriteLine($"ID: {item.Id}, Name: {item.Name}");

                Console.Write("Choose category ID for the product: ");
                var chosenCategoryId = int.Parse(Console.ReadLine());

                using (var cmd = new NpgsqlCommand("INSERT INTO products (name, category_id) VALUES (@name, @category_id)", con))
                {
                    cmd.Parameters.AddWithValue("name", product_name);
                    cmd.Parameters.AddWithValue("category_id", chosenCategoryId);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Successfully inserted into products!");
                }

                con.Close();
            }
        }
*/
/*
        public static void SelectColumn(string connectionString, List<Category> categories)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                categories.Clear();
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            categories.Add(new Category { Id = id, Name = name });
                        }
                    }
                }

                Console.WriteLine("Available categories:");
                foreach (var category in categories)
                {
                    Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");
                }

                con.Close();
            }
        }

        public static void SelectProducts(string connectionString, List<Product> products)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();

                products.Clear();

                using (var cmd = new NpgsqlCommand("SELECT id, name, category_id FROM products", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            int categoryId = reader.GetInt32(2);
                            products.Add(new Product { Id = id, Name = name, Category_id = categoryId });
                        }
                    }
                }

                Console.WriteLine("Available products:");
                foreach (var product in products)
                {
                    Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Category ID: {product.Category_id}");
                }

                con.Close();
            }
        }

        public static void UpdateCategory(string connectionString, List<Category> categories)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                categories.Clear();
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            categories.Add(new Category { Id = id, Name = name });
                        }
                    }
                }
                Console.WriteLine("Available categories:");
                foreach (var item in categories)
                {
                    Console.WriteLine($"ID: {item.Id}, Name: {item.Name}");
                }

                Console.Write("Choose category ID for the update:");
                int categoryId = int.Parse(Console.ReadLine());

                var categoryToUpdate = categories.Find(c => c.Id == categoryId);
                if (categoryToUpdate == null)
                {
                    Console.WriteLine("Invalid category ID. Please try again.");
                    return;
                }

                Console.Write("Enter the new name for the category: ");
                string newCategoryName = Console.ReadLine();

                using (var updateCmd = new NpgsqlCommand("UPDATE categories SET name = @name WHERE id = @id", con))
                {
                    updateCmd.Parameters.AddWithValue("name", newCategoryName);
                    updateCmd.Parameters.AddWithValue("id", categoryId);
                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Category updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No rows were updated.");
                    }
                }

                categoryToUpdate.Name = newCategoryName;
                con.Close();
            }
        }

        public static void UpdateProduct(string connectionString, List<Product> products, List<Category> categories)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();

                products.Clear();
                using (var cmd = new NpgsqlCommand("SELECT id, name, category_id FROM products", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            int categoryId = reader.GetInt32(2);
                            products.Add(new Product { Id = id, Name = name, Category_id = categoryId });
                        }
                    }
                }
                Console.WriteLine("Available products:");
                foreach (var product in products)
                {
                    Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Category ID: {product.Category_id}");
                }

                Console.Write("Enter the ID of the product to update: ");
                int productId = int.Parse(Console.ReadLine());

                var productToUpdate = products.Find(p => p.Id == productId);
                if (productToUpdate == null)
                {
                    Console.WriteLine("Invalid product ID. Please try again.");
                    return;
                }

                Console.Write("Enter the new name for the product: ");
                string newProductName = Console.ReadLine();

                categories.Clear();
                using (var catCmd = new NpgsqlCommand("SELECT id, name FROM categories", con))
                {
                    using (var reader = catCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            categories.Add(new Category { Id = id, Name = name });
                        }
                    }
                }

                Console.WriteLine("Available categories:");
                foreach (var category in categories)
                    Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");

                Console.Write("Enter the new category ID for the product: ");
                int newCategoryId = int.Parse(Console.ReadLine());

                var chosenCategory = categories.Find(c => c.Id == newCategoryId);
                if (chosenCategory == null)
                {
                    Console.WriteLine("Invalid category ID. Please try again.");
                    return;
                }

                using (var updateCmd = new NpgsqlCommand("UPDATE products SET name = @name, category_id = @category_id WHERE id = @id", con))
                {
                    updateCmd.Parameters.AddWithValue("name", newProductName);
                    updateCmd.Parameters.AddWithValue("category_id", newCategoryId);
                    updateCmd.Parameters.AddWithValue("id", productId);
                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("Product updated successfully.");
                    else
                        Console.WriteLine("No rows were updated.");
                }
                productToUpdate.Name = newProductName;
                productToUpdate.Category_id = newCategoryId;

                con.Close();
            }
        }

        public static void DeleteCategory(string connectionString, List<string> tables)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();

                SelectTable(connectionString, tables);

                Console.Write("Enter the ID of the category to delete: ");
                int categoryId = int.Parse(Console.ReadLine());

                 var categoryToDelete = tables.Find(c => c.Id == categoryId);
                if (categoryToDelete == null)
                {
                    Console.WriteLine("Invalid category ID. Please try again.");
                    return;
                }

                using (var cmd = new NpgsqlCommand("DELETE FROM categories WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("id", categoryId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine($"Category with ID {categoryId} deleted successfully.");
                    else
                        Console.WriteLine("No rows were deleted.");
                }

                con.Close();
            }
        }

        public static void DeleteProduct(string connectionString, List<Product> products)
        {
            using (var con = new NpgsqlConnection(connectionString))
            {
                con.Open();

                SelectProducts(connectionString, products);

                Console.Write("Enter the ID of the product to delete: ");
                int productId = int.Parse(Console.ReadLine());

                var productToDelete = products.Find(p => p.Id == productId);
                if (productToDelete == null)
                {
                    Console.WriteLine("Invalid product ID. Please try again.");
                    return;
                }

                using (var cmd = new NpgsqlCommand("DELETE FROM products WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("id", productId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine($"Product with ID {productId} deleted successfully.");

                    else
                        Console.WriteLine("No rows were deleted.");
                }

                con.Close();
            }
        }
*/
    }
}