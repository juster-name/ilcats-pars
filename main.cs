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
            var dbConnection = Test.SqlConnectOpen();
            var transaction = dbConnection.BeginTransaction();
            var command = new MySql.MySqlCommand(dbConnection, transaction);


            dbConnection.Close();
            using (StreamWriter fileW = File.AppendText("carData.txt"))
            {
                foreach (var car in Test.getCars())
                {
                    try
                    {
                        command.CommandText = $"INSERT INTO Car(name) VALUES('{car.Name}')";
                        command.ExecuteNonQuery();
                        
                        foreach (var carModel in car.Models)
                        {
                        command.CommandText = 
                        "INSERT INTO Model(code, date_prod_range, complectations, car_id)" +
                         $"VALUES('{carModel.Code}', '{carModel.DateRange}', '{carModel.ComplectationCode}'," +
                         $"(SELECT id FROM Car WHERE name='{car.Name}'))";
                        command.ExecuteNonQuery();

                            foreach (var compl in carModel.Complectations)
                            {
                                command.CommandText = 
                                "INSERT INTO Complectations(date_prod_range, code, model_id" +
                                $"VALUES('{compl.DateRange}', '{compl.Code}'," +
                                $"(SELECT id FROM Model WHERE code='{carModel.Code}'))";
                                command.ExecuteNonQuery();

                                foreach (var data in compl.Data)
                                {
                                    command.CommandText =  
                                    "INSERT INTO ComplectationData(data_name, data_value, complectation_id)" +
                                    $"VALUES('{data.Key}', '{data.Value}'," +
                                    $"(SELECT id FROM Complectations WHERE code='{compl.Code}'))";
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                    transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Csl.WriteLine(e.Message);
                        transaction.Rollback();
                    }
                    //fileW.WriteLine(car.ToString());
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

        public static MySql.MySqlConnection SqlConnectOpen()
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

            return dbConnection;
        }
    }
}