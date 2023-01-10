using MySql = MySqlConnector;

namespace VladlenKazmiruk
{
    public static class TestSql
    {

        public static MySql.MySqlConnection SqlConnectOpen()
        {
            string mysqlConnectionString = System.IO.File.ReadAllText("db-string.user");

            Console.WriteLine("\n\t Creating database connection...");
            var dbConnection = new MySql.MySqlConnection(mysqlConnectionString);

            Console.WriteLine("\t Opening database connection...");
            dbConnection.Open();

            Console.Write("\n\t Ping ... ");
            if (dbConnection.Ping())
                Console.Write("Successful");
            else
                Console.Write("Denied");

            return dbConnection;
        }

        public static void InsertIntoDb(MySql.MySqlConnection connection, Data.CarModel carModel)
        {
             string? carNameOrNull = "NULL";

            if (carModel == null)
                throw new NullReferenceException("Car Model must be assigned before adding to database");

            var transaction = connection.BeginTransaction();
            var commandModel = new MySql.MySqlCommand(connection, transaction);
            var commandCompl= new MySql.MySqlCommand(connection, transaction);
            var commandName = new MySql.MySqlCommand(connection, transaction);
            var commandValue = new MySql.MySqlCommand(connection, transaction);
            var commandMod = new MySql.MySqlCommand(connection, transaction);

            try
            {
                if (carModel.Car != null && carModel.Car.Name != null)
                    carNameOrNull = carModel.Car.Name;

                commandModel.CommandText =
                $"INSERT INTO Model(code, date_prod_range, complectations, car_id)" +
                    $"VALUES('{carModel.Code}', '{carModel.DateRange}', '{carModel.ComplectationCode}'," +
                    $"(SELECT id FROM Car WHERE name='{carNameOrNull}'))";
                commandModel.ExecuteNonQuery();

                var modelComplsOrEmpty = Enumerable.Empty<Data.Complectation>();

                if (carModel.Complectation == null)
                    Console.WriteLine($"No Conplectation found in {carModel.Name}");
                else
                    modelComplsOrEmpty = carModel.Complectation;

                foreach (var compl in modelComplsOrEmpty)
                {
                    commandCompl.CommandText =
                    "INSERT INTO Complectation(date_prod_range, code, model_id)" +
                    $"VALUES('{compl.DateRange}', '{carModel.ComplectationCode}','{commandModel.LastInsertedId}')"; 
                    //$"(SELECT id FROM Model WHERE code='{carModel.Code}')"

                    commandCompl.ExecuteNonQuery();

                    foreach (var data in compl.ModData)
                    {

                        commandName.CommandText =
                        "INSERT INTO ModDataName(name)" +
                        $"VALUES('{data.Key}')";

                        commandName.ExecuteNonQuery();
                        
                        commandValue.CommandText =
                        "INSERT INTO ModDataValue(value)" +
                        $"VALUES('{data.Value}')";
                        commandValue.ExecuteNonQuery();

                        commandMod.CommandText =
                        "INSERT INTO Modification(code, complectation_id, data_name_id, data_value_id)" +
                        $"VALUES('{compl.ModCode}', '{commandCompl.LastInsertedId}'," + 
                        $"'{commandName.LastInsertedId}', '{commandValue.LastInsertedId}')";

                        commandMod.ExecuteNonQuery();
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