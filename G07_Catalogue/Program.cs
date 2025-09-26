using Catalogue.Data.Reader;
using Catalogue.Database.Importer;
using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace G07_Catalogue
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Beqa\OneDrive\Desktop\csv.txt";
            string connectionString = "Data Source=LAPTOP-4OJRG3M6\\SQLSERVER;Initial Catalog=Northwind;Integrated Security=True;TrustServerCertificate=True";

            //IDataReader<Category> reader = CsvFileReaderFactory.Create(filePath);
            //IDataWriter<Category> writer = DatabaseWriterFactory.Create(connectionString);

            //try
            //{
            //    IEnumerable<Category> categories = reader.GetData();
            //    writer.WriteData(categories);
            //    Console.WriteLine("Data import completed successfully.");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"An error occurred during the import: {ex.Message}");
            //}

            IDataReader<Category> reader = DatabaseReaderFactory.Create(connectionString);
            IDataWriter<Category> writer = CsvFileWriterFactory.Create(filePath);

            try
            {
                IEnumerable<Category> categories = reader.GetData();
                writer.WriteData(categories);
                Console.WriteLine("Data export completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the export: {ex.Message}");
            }
        }
    }
}
