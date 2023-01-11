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
                int modelId = executeModelInsert(transaction, carModel);

                foreach (var compl in modelComplsOrEmpty)
                {
                    int complId = executeComplectationInsert(transaction, compl, modelId);
                    foreach (var data in compl.ModData)
                    {
                        int keyId = executeKeyInsert(transaction, data.Key);
                        int valueId = executeValueInsert(transaction, data.Value);

                        int modId = executeModificationInsert(transaction, compl, complId);
                        int dataPairId = executeDataPairInsert(transaction, modId, keyId, valueId);
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
            Console.WriteLine(DateTime.Now.ToLocalTime() + ": " + arg);
        }

        static void logQueryCommand(string name)
        {
            Console.WriteLine(DateTime.Now.ToLocalTime() + $": Querying {name}");
        }

        static int insert(MySql.MySqlCommand command, string tableName, string column, string value)
        {
            command.CommandText =
            $"INSERT IGNORE INTO {tableName}({column}) " +
            $"VALUES({value})";

            logQueryCommand(command.CommandText);

            command.ExecuteNonQuery();

            return ((int)command.LastInsertedId);
        }

        static int selectIdWhere(MySql.MySqlCommand command, string tableName, string whereColumn, string whereEquals)
        {
            command.CommandText= $"SELECT id FROM {tableName} WHERE ({whereColumn}) = ({whereEquals})";

            logQueryCommand(command.CommandText);

            var value = (command.ExecuteScalar());
            if (value == null)
                throw new NullReferenceException();
                
            return ((int)value);
        }

        static int insertOrSelect(MySql.MySqlCommand command, string tableName, 
            string columnsArg, string valuesArg, string where, string equals)
        {
            var lastId = insert(command, tableName, columnsArg, valuesArg);


            if (lastId == 0)
                return selectIdWhere(command, tableName, where, equals);

            return lastId;
        }

        static int executeModelInsert(MySql.MySqlTransaction transaction, Data.CarModel carModel)
        {
            if (carModel == null)
                throw new NullReferenceException("Car Model must be assigned before adding to database");

            string? carNameOrNull = "NULL";

            if (carModel.Car != null && carModel.Car.Name != null)
                    carNameOrNull = carModel.Car.Name;

            return insertOrSelect(command("model", transaction), 
                tableName:"Model", 
                columnsArg: "code, name, date_prod_range, complectations, car_id",
                valuesArg: $"'{carModel.Code}','{carModel.Name}', '{carModel.DateRange}', '{carModel.ComplectationCode}', " +
                    $"(SELECT id FROM Car WHERE name='{carNameOrNull}')",
                where:"name, code", 
                equals:$"'{carModel.Name}', '{carModel.Code}'");
        }

        static int executeComplectationInsert(MySql.MySqlTransaction transaction, Data.Complectation compl, int modelIdLink)
        {
            return insertOrSelect(command("compl", transaction), 
                tableName:"Complectation", 
                columnsArg: "date_prod_range, code, model_id",
                valuesArg: $"'{compl.DateRange}', '{compl.CarModel?.ComplectationCode}','{modelIdLink}'", 
                where:"code, model_id", 
                equals:$"'{compl.CarModel?.ComplectationCode}', '{modelIdLink}'");
        }

        static int executeModificationInsert(MySql.MySqlTransaction transaction, Data.Complectation compl, int compllIdLink)
        {
            return insertOrSelect(command("mod", transaction), 
                tableName:"Modification", 
                columnsArg:"code, complectation_id", 
                valuesArg: $"'{compl.ModCode}', '{compllIdLink}'", 
                where:"code, complectation_id", 
                equals:$"'{compl.ModCode}', '{compllIdLink}'");
        }

        static int executeKeyInsert(MySql.MySqlTransaction transaction, string dataKey)
        {
            return insertOrSelect(command("name", transaction), 
                tableName:"ModDataName", 
                columnsArg:"name", 
                valuesArg:$"'{dataKey}'", 
                where:"name", 
                equals:$"'{dataKey}'");
        }

        static int executeValueInsert(MySql.MySqlTransaction transaction, string dataValue)
        {
            return insertOrSelect(command("value", transaction), 
                tableName:"ModDataValue", 
                columnsArg:"value", 
                valuesArg: $"'{dataValue}'", 
                where:"value", 
                equals:$"'{dataValue}'");
        }

        static int executeDataPairInsert(MySql.MySqlTransaction transaction, int modId, int keyId, int valueId)
        {
            return insertOrSelect(command("dataPair", transaction), 
                tableName:"ModDataPair",
                columnsArg:"modification_id, name_id, value_id", 
                valuesArg:$"'{modId}', '{keyId}', '{valueId}'", 
                where:"modification_id, name_id, value_id", 
                equals:$"'{modId}', '{keyId}', '{valueId}'");
            
        }
    }
}