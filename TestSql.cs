using MySql = MySqlConnector;

namespace VladlenKazmiruk
{
    public static class TestSql
    {
        static MySql.MySqlConnection dbConnection = new MySql.MySqlConnection();

        static System.Collections.Generic.Dictionary<string, MySql.MySqlCommand> commands =
            new System.Collections.Generic.Dictionary<string, MySql.MySqlCommand>();

        public static MySql.MySqlConnection SqlConnectOpen()
        {
            string mysqlConnectionString = System.IO.File.ReadAllText("db-string.user");

            Console.WriteLine("\n\t Creating database connection...");
            TestSql.dbConnection = new MySql.MySqlConnection(mysqlConnectionString);

            Console.WriteLine("\t Opening database connection...");
            TestSql.dbConnection.Open();

            Console.Write("\n\t Ping ... ");
            if (TestSql.dbConnection.Ping())
                Console.Write("Successful");
            else
                Console.Write("Denied");

            return TestSql.dbConnection;
        }

        static MySql.MySqlCommand command(string name, MySql.MySqlTransaction transaction)
        {
            return commands.GetValueOrDefault(name, 
                new MySql.MySqlCommand(TestSql.dbConnection, transaction));
        }

        public static void InsertIntoDb(MySql.MySqlConnection connection, Data.CarModel carModel)
        {
             string? carNameOrNull = "NULL";

            if (carModel == null)
                throw new NullReferenceException("Car Model must be assigned before adding to database");

            var transaction = connection.BeginTransaction();

            try
            {
                if (carModel.Car != null && carModel.Car.Name != null)
                    carNameOrNull = carModel.Car.Name;

                command("model", transaction).CommandText =
                $"INSERT IGNORE INTO Model(code, name, date_prod_range, complectations, car_id)" +
                    $"VALUES('{carModel.Code}','{carModel.Name}', '{carModel.DateRange}', '{carModel.ComplectationCode}'," +
                    $"(SELECT id FROM Car WHERE name='{carNameOrNull}'))";
                command("model", transaction).ExecuteNonQuery();

                var modelComplsOrEmpty = Enumerable.Empty<Data.Complectation>();

                if (carModel.Complectation == null)
                    Console.WriteLine($"No Conplectation found in {carModel.Name}");
                else
                    modelComplsOrEmpty = carModel.Complectation;

                foreach (var compl in modelComplsOrEmpty)
                {
                    command("compl", transaction).CommandText =
                    "INSERT IGNORE INTO Complectation(date_prod_range, code, model_id)" +
                    $"VALUES('{compl.DateRange}', '{carModel.ComplectationCode}','{command("model", transaction).LastInsertedId}')"; 
                    //$"(SELECT id FROM Model WHERE code='{carModel.Code}')"

                    command("compl", transaction).ExecuteNonQuery();

                    foreach (var data in compl.ModData)
                    {

                        command("name", transaction).CommandText =
                        "INSERT IGNORE INTO ModDataName(name)" +
                        $"VALUES('{data.Key}')";

                        command("name", transaction).ExecuteNonQuery();
                        
                        command("value", transaction).CommandText =
                        "INSERT IGNORE INTO ModDataValue(value)" +
                        $"VALUES('{data.Value}')";

                        command("value", transaction).ExecuteNonQuery();

                        command("dataPair", transaction).CommandText =
                        "INSERT IGNORE INTO ModDataPair(name_id,value_id)" +
                        $"VALUES('{command("name", transaction).LastInsertedId}', '{command("value", transaction).LastInsertedId}')";

                        command("mod", transaction).CommandText =
                        "INSERT IGNORE INTO Modification(code, complectation_id, data_name_id, data_value_id)" +
                        $"VALUES('{compl.ModCode}', '{command("compl", transaction).LastInsertedId}'," + 
                        $"'{command("name", transaction).LastInsertedId}', '{command("value", transaction).LastInsertedId}')";

                        command("mod", transaction).ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
            }
        
        }
    }
}