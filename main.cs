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
            using (StreamWriter fileW = File.AppendText("carData.txt"))
            {
                foreach (var car in Test.getCars())
                {
                    fileW.WriteLine(car.ToString());
                }
            }
        }
    }

    public static class Test
    {
        public static IEnumerable<Data.Car> getCars()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var carsAddress = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
            var context = BrowsingContext.New(config);
            var document = context.OpenAsync(carsAddress).WaitAsync(CancellationToken.None).Result;


            var carCatcher = new Parser.CarsCatcher(document.GetElementById("Body"));
            var carModelCatcher = new Parser.CarModelCatcher(null);
            var complCatcher = new Parser.ComplCatcher(null);

            var buffCarModels = new HashSet<Data.CarModel>();
            var buffCompls = new HashSet<Data.Complectation>();

            foreach (var car in carCatcher.Catch())
            {
                carModelCatcher.changeContext(carCatcher?.CurrentElement);

                buffCarModels.Clear();

                foreach (Data.CarModel carModel in carModelCatcher.Catch())
                {
                    carModel.Car = car;
                    var newUrl = "https://www.ilcats.ru" + carModel.Url;

                    var docCompl = context.OpenAsync(newUrl).WaitAsync(CancellationToken.None).Result;
                    var el = docCompl.GetElementById("Body");
                    complCatcher.changeContext(el);

                    foreach(Data.Complectation compl in complCatcher.Catch())
                    {
                        compl.CarModel = carModel;
                        buffCompls.Add(compl);
                    }
                    carModel.Complectations = buffCompls;
                    buffCarModels.Add(carModel);
                    
                }
                
                car.Models = buffCarModels;
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