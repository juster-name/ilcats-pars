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
            var transaction = connection.BeginTransaction();

            var modelComplsOrEmpty = Enumerable.Empty<Data.Complectation>();
            if (carModel.Complectation == null)
                Console.WriteLine($"No Conplectation found in {carModel.Name}");
            else
                modelComplsOrEmpty = carModel.Complectation;

            try
            {
                executeModelInsert(carModel, transaction);

                foreach (var compl in modelComplsOrEmpty)
                {
                    executeComplectationInsert(compl, transaction);

                    foreach (var data in compl.ModData)
                    {
                        executeKeyInsert(data.Key, transaction);
                        executeValueInsert(data.Value, transaction);
                        executeDataPairInsert(transaction);
                        executeModificationInsert(compl, transaction);
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

        static int executeModelInsert(Data.CarModel carModel, MySql.MySqlTransaction transaction)
        {
            if (carModel == null)
                throw new NullReferenceException("Car Model must be assigned before adding to database");

            string? carNameOrNull = "NULL";

            if (carModel.Car != null && carModel.Car.Name != null)
                    carNameOrNull = carModel.Car.Name;

                command("model", transaction).CommandText =
                $"INSERT IGNORE INTO Model(code, name, date_prod_range, complectations, car_id)" +
                    $"VALUES('{carModel.Code}','{carModel.Name}', '{carModel.DateRange}', '{carModel.ComplectationCode}'," +
                    $"(SELECT id FROM Car WHERE name='{carNameOrNull}'))";
                return command("model", transaction).ExecuteNonQuery();
        }

        static int executeComplectationInsert(Data.Complectation compl, MySql.MySqlTransaction transaction)
        {
            command("compl", transaction).CommandText =
                    "INSERT IGNORE INTO Complectation(date_prod_range, code, model_id)" +
                    $"VALUES('{compl.DateRange}', '{compl.CarModel?.ComplectationCode}','{command("model", transaction).LastInsertedId}')"; 
                    //$"(SELECT id FROM Model WHERE code='{carModel.Code}')"

            return command("compl", transaction).ExecuteNonQuery();
        }

        static int executeKeyInsert(string dataKey, MySql.MySqlTransaction transaction)
        {
            command("name", transaction).CommandText =
            "INSERT IGNORE INTO ModDataName(name)" +
            $"VALUES('{dataKey}')";

            return command("name", transaction).ExecuteNonQuery();
        }

        static int executeValueInsert(string dataValue, MySql.MySqlTransaction transaction)
        {
            command("value", transaction).CommandText =
            "INSERT IGNORE INTO ModDataValue(value)" +
            $"VALUES('{dataValue}')";

            return command("value", transaction).ExecuteNonQuery();
        }

        static int executeDataPairInsert(MySql.MySqlTransaction transaction)
        {
            command("dataPair", transaction).CommandText =
            "INSERT IGNORE INTO ModDataPair(name_id,value_id)" +
            $"VALUES('{command("name", transaction).LastInsertedId}', '{command("value", transaction).LastInsertedId}')";

            return command("dataPair", transaction).ExecuteNonQuery();
        }

        static int executeModificationInsert(Data.Complectation compl, MySql.MySqlTransaction transaction)
        {
            command("mod", transaction).CommandText =
            "INSERT IGNORE INTO Modification(code, complectation_id, data_name_id, data_value_id)" +
            $"VALUES('{compl.ModCode}', '{command("compl", transaction).LastInsertedId}'," + 
            $"'{command("name", transaction).LastInsertedId}', '{command("value", transaction).LastInsertedId}')";

            return command("mod", transaction).ExecuteNonQuery();
        }
    }
}