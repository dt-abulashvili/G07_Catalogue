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
            //string connectionString = args.Length > 0
                //? args[0]
                //: "Server=localhost;Database=Catalogue;User Id=sa;Password=your_password;";

            Console.WriteLine("Starting Catalogue Importer...");
            Console.Write("Enter the path to the CSV file: ");
            //string filePath = Console.ReadLine()!;
            string filePath = @"C:\Users\Beqa\OneDrive\Desktop\Products.csv";
            string connectionString = "Data Source=LAPTOP-4OJRG3M6\\SQLSERVER;Initial Catalog=G07_Catalogue;Integrated Security=True;TrustServerCertificate=True";

            IDataReader<Category> reader = CsvFileReaderFactory.Create(filePath);
            IDataWriter<Category> writer = DatabaseWriterFactory.Create(connectionString);
           
            try
            {
                IEnumerable<Category> categories = reader.GetData();
                writer.WriteData(categories);
                Console.WriteLine("Data import completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the import: {ex.Message}");
            }
        }
    }
}
