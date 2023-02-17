using AngleSharp;
using AngleSharp.Dom;
using System.Collections.Concurrent;
using VladlenKazmiruk.Parser;

using MySql = MySqlConnector;
using Csl = System.Console;


namespace VladlenKazmiruk
{
    class CLIProgram
    {
        public static Data.Car ToyotaEuCar = new Data.Car(){
            Name = "toyota",
            MarketCode = "EU",
            Market = "Европа"
        };

        public static void Main(string[] args)
        {

            using (var connection = TestSql.SqlConnectOpen())
            {
                //TestSql.InsertIntoDb(connection, getMockData());
                
                 foreach (var carModel in TestParse.ParseCar(ToyotaEuCar))
                 {
                     TestSql.InsertIntoDb(connection, carModel);
                 }
                
                TestSql.log("Closing connection...");
                connection.Close();
            }
            
        }
#region MockData
        public static Data.CarModel getMockData()
            {
                var cm = new Data.CarModel(){
                    Name = "TestCarModel",
                    Code = "11111",
                    DateRange = "01.1990 - 04-1997",
                    ComplectationCode = "2fg 4fd f# c5# gd#",
                    Car = ToyotaEuCar,
                    Url = "/testCarModelUrl",
                    Complectation = new List<Data.Complectation>(2)
                };
                cm.Car = ToyotaEuCar;

                ((List<Data.Complectation>)cm.Complectation).Add(new Data.Complectation(){
                    ModCode = "4F56-DFG1",
                    DateRange = "01.1990 - 01-1991",
                    CarModel = cm,
                    Url = "/testComplUrl1",
                    ModData = new List<KeyValuePair<string, string>>()
                });

                ((List<Data.Complectation>)cm.Complectation)[0].ModData.Add(new KeyValuePair<string, string>("ENGINE 1", "2L"));
                ((List<Data.Complectation>)cm.Complectation)[0].ModData.Add(new KeyValuePair<string, string>("GRADE", "STD"));
                ((List<Data.Complectation>)cm.Complectation)[0].ModData.Add(new KeyValuePair<string, string>("BODY", "T1"));
                ((List<Data.Complectation>)cm.Complectation)[0].ModData.Add(new KeyValuePair<string, string>("ATM,MTM", "MTM"));

                ((List<Data.Complectation>)cm.Complectation).Add(new Data.Complectation(){
                    ModCode = "3A76-G6F2",
                    DateRange = "02.1991 - 05-1992",
                    CarModel = cm,
                    Url = "/testComplUrl2",
                    ModData = new List<KeyValuePair<string, string>>()
                });
                ((List<Data.Complectation>)cm.Complectation)[1].ModData.Add(new KeyValuePair<string, string>("ENGINE 1", "2L"));
                ((List<Data.Complectation>)cm.Complectation)[1].ModData.Add(new KeyValuePair<string, string>("GRADE", "DLX"));
                ((List<Data.Complectation>)cm.Complectation)[1].ModData.Add(new KeyValuePair<string, string>("BODY", "T1"));
                ((List<Data.Complectation>)cm.Complectation)[1].ModData.Add(new KeyValuePair<string, string>("TRANSMISSION MODEL", "G"));
                ((List<Data.Complectation>)cm.Complectation)[1].ModData.Add(new KeyValuePair<string, string>("LOADING CAPACITY", "HLF"));

                ToyotaEuCar.Models = new List<Data.CarModel>();
                ((List<Data.CarModel>)ToyotaEuCar.Models).Add(cm);

                return cm;
            }
#endregion
    }
}