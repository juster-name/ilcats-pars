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
        static void endTransaction()
        {
            commands.Clear();
        }

        public static void InsertIntoDb(MySql.MySqlConnection connection, Data.CarModel carModel)
        {
            if (carModel == null)
                throw new NullReferenceException("Car Model must not be null when inserting into database");
             if (carModel.Car == null)
                throw new NullReferenceException("Car must not be null when inserting into database");

            log("Beginning database transaction.");
            var transaction = connection.BeginTransaction();

            int carId = executeCarInsert(command("car", transaction), carModel.Car);

            try
            {
                int modelNameId = executeModelNameInsert(command("modelName", transaction), carModel);
                int modelId = executeModelInsert(command("model", transaction), carModel, carId, modelNameId);

                foreach (var compl in carModel.Complectation)
                {
                    int complId = executeComplectationInsert(command("compl", transaction), compl, modelId);
                    foreach (var data in compl.ModData)
                    {
                        int keyId = executeKeyInsert(command("name", transaction), data.Key);
                        int valueId = executeValueInsert(command("value", transaction), data.Value);

                        int modId = executeModificationInsert(command("mod", transaction), compl, complId);
                        int dataPairId = executeDataPairInsert(command("dataPair", transaction), modId, keyId, valueId);
                    }
                }
            }
            catch (Exception e)
            {
                log(e.Message);
                log("Rolling back");
                transaction.Rollback();
            }

            try
            {
                log("Commiting transaction.");
                transaction.Commit();
                log("Transaction commited successfully.");
            }
            catch(Exception e)
            {
                log("Transaction FAILED.");
                log(e.Message);
            }
            finally
            {
                endTransaction();
            }
        }

        static public void log(string arg)
        {
            Console.WriteLine(DateTime.Now.ToLocalTime() + ": " + arg);
        }

        static void logQueryCommand(string name)
        {
            Console.WriteLine(DateTime.Now.ToLocalTime() + $": Querying {name}");
        }

        static Dictionary<string,string> addParams(MySql.MySqlCommand command, int len, List<string> values)
        {
            command.Parameters.Clear();
            var paramsDict = formParamsDict(len, values);

            foreach (var item in paramsDict)
                command.Parameters.AddWithValue(item.Key, item.Value);

            return paramsDict;
        }

        static Dictionary<string, string> formParamsDict(int len, List<string> values)
        {
            var pKeys = parseParams(len);
            var pValues = formValues(values);

            return Enumerable.Range(0, len).ToDictionary(i => pKeys[i], i => pValues[i]);
        }

        static List<string> parseParams(int paramsN)
        {
            var retList = new List<string>();

            for (int i = 0; i < paramsN; i++)
            {
                retList.Add($"@param{i}");
            }

            return retList;
        }

        static List<string>  formValues(List<string> paramValues)
        {
            paramValues.ForEach(x => x.Trim());
            return paramValues;
        }

        static int insertOrSelect(MySql.MySqlCommand command, string tableName, 
            string columnsArg, List<string> values, string where, List<string> equals)
        {
            var lastId = insert(command, tableName, columnsArg, values);

            if (lastId == 0)
                return selectIdWhere(command, tableName, where, equals);

            return lastId;
        }

        static int insert(MySql.MySqlCommand command, string tableName, string columnArg, List<string> values)
        {
            var paramsDict = addParams(command, values.Count, values);

            command.CommandText =
            $"INSERT IGNORE INTO {tableName}({columnArg}) " +
            $"VALUES({String.Join(',', paramsDict.Keys)})";

            logQueryCommand(command.CommandText);
            command.ExecuteNonQuery();

            return ((int)command.LastInsertedId);
        }

        static int selectIdWhere(MySql.MySqlCommand command, string tableName, string where, List<string> equals)
        {
            var paramsDict = addParams(command, equals.Count, equals);

            command.CommandText= $"SELECT id FROM {tableName} " + 
            $"WHERE ({where}) = ({String.Join(',', paramsDict.Keys)})";

            logQueryCommand(command.CommandText);

            var value = (command.ExecuteScalar());
            if (value == null)
                throw new NullReferenceException();
                
            return ((int)value);
        }

        static List<string> valueArray(params string?[] args)
        {
            return new List<string>(args.Select(val => val = (val == null ? "NULL" : val)));
        }

        static int executeCarInsert(MySql.MySqlCommand command, Data.Car car)
        {
            if (car.Name == null)
                throw new NullReferenceException();

            return insertOrSelect(command, 
                tableName:"Car", 
                columnsArg: "name, market_code, market_name",
                values: valueArray(car.Name, $"{car.MarketCode}", car.Market), 
                where: "name", 
                equals: valueArray(car.Name));
        }

        static int executeModelNameInsert(MySql.MySqlCommand command, Data.CarModel carModel)
        {
            return insertOrSelect(command,
                tableName:"ModelName", 
                columnsArg: "name",
                values: valueArray(carModel.Name),
                where:"name", 
                equals: valueArray(carModel.Name));
        }

        static int executeModelInsert(MySql.MySqlCommand command, Data.CarModel carModel, int carId, int nameId)
        {
            string? carNameOrNull = "NULL";

            if (carModel.Car != null && carModel.Car.Name != null)
                    carNameOrNull = carModel.Car.Name;

            return insertOrSelect(command, 
                tableName:"Model", 
                columnsArg: "code, name, car_id, model_name_id",
                values: valueArray($"{carModel.Code}", carModel.Name, $"{carId}", $"{nameId}"),
                where:"name, code", 
                equals: valueArray(carModel.Name, $"{carModel.Code}"));
        }

        static int executeComplectationInsert(MySql.MySqlCommand command, Data.Complectation compl, int modelIdLink)
        {
            return insertOrSelect(command, 
                tableName:"Complectation", 
                columnsArg: "date_prod_range, code, model_id",
                values: valueArray($"{compl.CarModel?.DateRange}", $"{compl.CarModel?.ComplectationCode}", $"{modelIdLink}"), 
                where:"code, model_id", 
                equals:valueArray($"{compl.CarModel?.ComplectationCode}", $"{modelIdLink}"));
        }

        static int executeModificationInsert(MySql.MySqlCommand command, Data.Complectation compl, int compllIdLink)
        {
            return insertOrSelect(command, 
                tableName:"Modification", 
                columnsArg:"code, complectation_id, date_prod_range", 
                values: valueArray($"{compl.ModCode}", $"{compllIdLink}", $"{compl.DateRange}"), 
                where:"code, complectation_id", 
                equals:valueArray($"{compl.ModCode}", $"{compllIdLink}"));
        }

        static int executeKeyInsert(MySql.MySqlCommand command, string dataKey)
        {
            return insertOrSelect(command, 
                tableName:"ModDataName", 
                columnsArg:"name", 
                values:valueArray($"{dataKey}"), 
                where:"name", 
                equals:valueArray($"{dataKey}"));
        }

        static int executeValueInsert(MySql.MySqlCommand command, string dataValue)
        {
            return insertOrSelect(command, 
                tableName:"ModDataValue", 
                columnsArg:"value", 
                values: valueArray($"{dataValue}"), 
                where:"value", 
                equals:valueArray($"{dataValue}"));
        }

        static int executeDataPairInsert(MySql.MySqlCommand command, int modId, int keyId, int valueId)
        {
            return insertOrSelect(command, 
                tableName:"ModDataPair",
                columnsArg:"modification_id, name_id, value_id", 
                values:valueArray($"{modId}", $"{keyId}", $"{valueId}"), 
                where:"modification_id, name_id, value_id", 
                equals:valueArray($"{modId}", $"{keyId}", $"{valueId}"));
            
        }
    }
}