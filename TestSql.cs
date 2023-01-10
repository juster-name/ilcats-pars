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

            Console.WriteLine("\n\t Closing connection...");

            return dbConnection;
        }

        public static void InsertIntoDb(MySql.MySqlConnection connection, Data.CarModel carModel)
        {
            if (carModel == null)
                throw new NullReferenceException("Car Model must be assigned before adding to database");

            var transaction = connection.BeginTransaction();
            var command = new MySql.MySqlCommand(connection, transaction);

            bool isCarNull = carModel.Car == null;

            try
            {
                command.CommandText =
                $"INSERT INTO Model(code, date_prod_range, complectations, car_id)" +
                    $"VALUES('{carModel.Code}', '{carModel.DateRange}', '{carModel.ComplectationCode}'," +
                    $"(SELECT id FROM Car WHERE name='{carModel.Car?.Name}'))";
                command.ExecuteNonQuery();

                if (carModel.Complectations == null)
                    throw new NullReferenceException("Complectations must be assigned before adding to database");

                foreach (var compl in carModel.Complectations)
                {
                    command.CommandText =
                    "INSERT INTO Complectation(date_prod_range, code, model_id)" +
                    $"VALUES('{compl.DateRange}', '{compl.Code}'," +
                    $"(SELECT id FROM Model WHERE code='{carModel.Code}'))";
                    command.ExecuteNonQuery();

                    foreach (var data in compl.Data)
                    {
                        
                        command.CommandText =
                        "INSERT INTO ComplectationData(data_name, data_value, complectation_id)" +
                        $"VALUES('{data.Key}', '{data.Value}'," +
                        $"(SELECT id FROM Complectation "+
                        $"WHERE code='{compl.Code}' AND date_prod_range='{compl.DateRange}'))";
                        command.ExecuteNonQuery();
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