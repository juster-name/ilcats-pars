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

        public static async Task Main(string[] args)
        {
            await foreach (var car in Test.getCars())
            {
                Csl.WriteLine(car.Name);
                foreach (CarModel carModel in car.Models)
                {
                    Csl.WriteLine($"\t{carModel.Url}");
                    Csl.WriteLine($"\t{carModel.Code} | {carModel.DateRange} | {carModel.ComplectationCode}");
                }
                
            }
            //SqlConnectionCheck();
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

    public static class Test
    {
        static string topLevelSelector = ".Multilist";
        static string carCellSelector = ":scope > div[class='List']"; // :scope для topLevelSelector
        static string carNameSelector = "div[class='name']";
        static string carInfoSelector = "div[class='List'] div[class='List']";
        static string carDatesSelector = "div[class='dateRange']";
        static string carComplCodeSelector = "div[class='modelCode']";

        public static async IAsyncEnumerable<Car> getCars()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);

            // :scope для поиска только по верхнему уровню в .Multilist
            // [0] для обхода Possible NullReference от QuerySelector
            var carCells = document.QuerySelectorAll(topLevelSelector)[0].QuerySelectorAll(carCellSelector);

            foreach (var cellEl in carCells )
            {
                var car = new Car(cellEl);
                var carNameEl = cellEl.QuerySelector(carNameSelector);
                car.Name = carNameEl?.TextContent; 
                car.Models = getCarModels(cellEl);

                yield return car;
            }
        }

        public static IEnumerable<CarModel> getCarModels(AngleSharp.Dom.IElement topElement)
        {
                var carInfos = topElement.QuerySelectorAll(carInfoSelector);
                var carModels = new BlockingCollection<CarModel>();

                foreach (var carInfoEl in carInfos)
                {
                    var carModel = new CarModel(carInfoEl);

                    var idEl = carInfoEl.QuerySelector("a");
                    carModel.Url = idEl?.GetAttribute("href");
                    carModel.Code = idEl?.TextContent;

                    carModel.DateRange = carInfoEl.QuerySelector(carDatesSelector)?.TextContent;
                    carModel.ComplectationCode = carInfoEl.QuerySelector(carComplCodeSelector)?.TextContent;

                    carModels.Add(carModel);
                }

                return carModels;
        }
    }
}