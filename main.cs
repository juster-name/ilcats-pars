// See https://aka.ms/new-console-template for more information
using AngleSharp;
using AngleSharp.Dom;
using System.Collections.Concurrent;
using VladlenKazmiruk.Parser;

using MySql = MySqlConnector;
using Csl = System.Console;

// TODO: Несколько значений из Complectation subquery в insert, откуда-то дупликаты.
// Загрузить документы для парсинга в файл, а не ддосить их сайт каждый раз.

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
                foreach (var carModel in TestParse.ParseCar(ToyotaEuCar))
                {
                    TestSql.InsertIntoDb(connection, carModel);
                }
                connection.Close();
            }
        }
    }
}