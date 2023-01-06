// See https://aka.ms/new-console-template for more information
using AngleSharp;
using System.Collections.Concurrent;
using VladlenKazmiruk.Parser;

using MySql = MySqlConnector;
using Csl = System.Console;

namespace VladlenKazmiruk
{
    class CLIProgram
    {

        public static void Main(string[] args)
        {
            foreach (var car in Test.getCars())
            {
                Csl.WriteLine($"\t{car.Name}");
            }
        }
    }

    public static class Test
    {
        public static IEnumerable<Data.Car> getCars()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
            var context = BrowsingContext.New(config);
            var document = context.OpenAsync(address).WaitAsync(CancellationToken.None).Result;

            var carCatcher = new Parser.CarsCatcher(document.GetElementById("Body"));

            foreach (var car in carCatcher.Catch())
            {
                Csl.WriteLine($"Current Element = {carCatcher?.CurrentElement?.OuterHtml}");

                yield return car;
            }
        }

        static void SqlConnectionCheck()
        {
            string mysqlConnectionString = System.IO.File.ReadAllText("db-string.user");

            Csl.WriteLine("\n\t Creating database connection...");
            var dbConnection = new MySql.MySqlConnection(mysqlConnectionString);

            Csl.WriteLine("\t Opening database connection...");
            dbConnection.Open();

            Csl.Write("\n\t Ping ... ");
            if (dbConnection.Ping())
                Csl.Write("Successful");
            else
                Csl.Write("Denied");

            Csl.WriteLine("\n\t Closing connection...");
            dbConnection.Close();
        }
    }
}