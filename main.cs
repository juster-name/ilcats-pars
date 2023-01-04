// See https://aka.ms/new-console-template for more information
using AngleSharp;
using MySql = MySqlConnector;
using Csl = System.Console;

namespace VladlenKazmiruk
{
    class CLIProgram
    {

        public static void Main(string[] args)
        {
            SqlConnectionCheck();

            var carNames = Test.getTestNames();
            foreach (var name in carNames){
                Csl.WriteLine(name);}
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
        public static IEnumerable<string> getTestNames()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
            var context = BrowsingContext.New(config);
            var openDocTask = context.OpenAsync(address);
            var document = 
                openDocTask.WaitAsync(CancellationToken.None).Result;
            var carCellsSelector = 
                "div[class='List Multilist'] div[class='Header'] div[class='name']";

            return document.QuerySelectorAll(carCellsSelector).Select(el => el.TextContent);
        }
    }
}