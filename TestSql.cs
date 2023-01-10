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

            log("\nCreating database connection...");
            TestSql.dbConnection = new MySql.MySqlConnection(mysqlConnectionString);

            log("Opening database connection...");
            TestSql.dbConnection.Open();

            log("\nPing ... ");
            if (TestSql.dbConnection.Ping())
                Console.Write("Successful.\n");
            else
                log("Denied.\n");

            return TestSql.dbConnection;
        }

        static MySql.MySqlCommand command(string name, MySql.MySqlTransaction transaction)
        {
            if (commands.ContainsKey(name) == false)
            {
                commands[name] = new MySql.MySqlCommand(TestSql.dbConnection, transaction);
            }
            return commands[name];
        }

        public static void InsertIntoDb(MySql.MySqlConnection connection, Data.CarModel carModel)
        {
            log("Beginning database transaction.");
            var transaction = connection.BeginTransaction();

            var modelComplsOrEmpty = Enumerable.Empty<Data.Complectation>();
            if (carModel.Complectation == null)
                log($"No Conplectation found in {carModel.Name}");
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
                        logInsert (executeKeyInsert(data.Key, transaction), "KeyInsert");
                        logInsert (executeValueInsert(data.Value, transaction), "ValueInsert");
                        logInsert (executeDataPairInsert(transaction), "DataPairInsert");
                        logInsert (executeModificationInsert(compl, transaction), "ModificationInsert");
                        logInsert (executeDataPairModIdInsert(transaction), "DataPairModIdInsert");
                    }
                }
                log("Commiting transaction.");
                transaction.Commit();
            }
            catch (Exception e)
            {
                log(e.Message);
                 log("Rolling back");
                transaction.Rollback();
            }
            log("Transaction commited successfully.");
        }

        static public void log(string arg)
        {
            log(DateTime.Now.ToLocalTime() + arg);
        }

        static void logInsert(int rows, string name)
        {
            Console.WriteLine(DateTime.Now.ToLocalTime() + $": '{name}' executed. Rows affected: {rows}");
        }

        static void logQueryCommand(string name)
        {
            Console.WriteLine(DateTime.Now.ToLocalTime() + $": Querying {name} command");
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

                logQueryCommand("ModelInsert");
                return command("model", transaction).ExecuteNonQuery();
        }

        static int executeComplectationInsert(Data.Complectation compl, MySql.MySqlTransaction transaction)
        {
            command("compl", transaction).CommandText =
                    "INSERT IGNORE INTO Complectation(date_prod_range, code, model_id)" +
                    $"VALUES('{compl.DateRange}', '{compl.CarModel?.ComplectationCode}','{command("model", transaction).LastInsertedId}')"; 

            logQueryCommand("ComplectationInsert");
            return command("compl", transaction).ExecuteNonQuery();
        }

        static int executeKeyInsert(string dataKey, MySql.MySqlTransaction transaction)
        {
            command("name", transaction).CommandText =
            "INSERT IGNORE INTO ModDataName(name)" +
            $"VALUES('{dataKey}')";

            logQueryCommand("KeyInsert");
            return command("name", transaction).ExecuteNonQuery();
        }

        static int executeValueInsert(string dataValue, MySql.MySqlTransaction transaction)
        {
            command("value", transaction).CommandText =
            "INSERT IGNORE INTO ModDataValue(value)" +
            $"VALUES('{dataValue}')";

            logQueryCommand("ValueInsert");
            return command("value", transaction).ExecuteNonQuery();
        }

        static int executeDataPairInsert(MySql.MySqlTransaction transaction)
        {
            command("dataPair", transaction).CommandText =
            "INSERT IGNORE INTO ModDataPair(name_id,value_id)" +
            $"VALUES('{command("name", transaction).LastInsertedId}', '{command("value", transaction).LastInsertedId}')";

            logQueryCommand("DataPairInsert");
            return command("dataPair", transaction).ExecuteNonQuery();
        }

        static int executeModificationInsert(Data.Complectation compl, MySql.MySqlTransaction transaction)
        {
            command("mod", transaction).CommandText =
            "INSERT IGNORE INTO Modification(code, complectation_id)" +
            $"VALUES('{compl.ModCode}', '{command("compl", transaction).LastInsertedId}')";

            logQueryCommand("ModificationInsert");
            return command("mod", transaction).ExecuteNonQuery();
        }

        static int executeDataPairModIdInsert(MySql.MySqlTransaction transaction)
        {
            command("dataPair", transaction).CommandText =
            "INSERT IGNORE INTO ModDataPair(modification_id)" +
            $"VALUES('{command("mod", transaction).LastInsertedId}')";

            logQueryCommand("DataPairModIdInsert");
            return command("dataPair", transaction).ExecuteNonQuery();
        }
    }
}