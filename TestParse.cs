using AngleSharp;
using AngleSharp.Dom;
using VladlenKazmiruk.Parser;


namespace VladlenKazmiruk
{
    public static class TestParse
    {
        public static IConfiguration config = Configuration.Default.WithDefaultLoader();
        // Реализован только парсинг моделей по примеру каталога моделей toyota - EU
        public static string carsAddress = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";

        public static IBrowsingContext context = BrowsingContext.New(config);
        public static IDocument document = context.OpenAsync(carsAddress).WaitAsync(CancellationToken.None).Result;


        public static ICatcher<string> carModelNameCatcher = new Parser.CarModelNameCatcher(document.GetElementById("Body"));
        public static ICatcher<Data.CarModel> carModelCatcher = new Parser.CarModelInfoCatcher(null);
        public static ICatcher<Data.Complectation> complCatcher = new Parser.ComplCatcher(null);


        // Используем класс Catcher для захвата значений.
        // Заполняем Data объект конкретными значениями и возвращаем заполненный Data объект
        public static IEnumerable<Data.CarModel> ParseCar(Data.Car car)
        {
            foreach (var carModelName in TestParse.carModelNameCatcher.Catch())
            {
                carModelCatcher.changeContext(carModelNameCatcher?.CurrentElement);

                foreach (var carModel in TestParse.parseCarModels(car, carModelName))
                {
                    TestParse.parseComplectations(carModel);
                    yield return carModel;
                }
            }
        }

        static IEnumerable<Data.CarModel>  parseCarModels(Data.Car car, string carModelName)
        {
            car.Models  = new HashSet<Data.CarModel>();

            foreach (Data.CarModel carModel in TestParse.carModelCatcher.Catch())
            {
                carModel.Car = car;
                carModel.Name = carModelName;

                var newUrl = Urls.baseFull + Urls.formComplectationPath("toyota", "EU", carModel.Code, carModel.DateRange); 

                var docCompl = context.OpenAsync(newUrl).WaitAsync(CancellationToken.None).Result;
                var el = docCompl.GetElementById("Body");
                complCatcher.changeContext(el);

                ((HashSet<Data.CarModel>)car.Models).Add(carModel);
            }

            return car.Models;
        }

        static IEnumerable<Data.Complectation> parseComplectations(Data.CarModel carModel)
        {
            carModel.Complectation =  new HashSet<Data.Complectation>();

            foreach (Data.Complectation compl in TestParse.complCatcher.Catch())
            {
                compl.CarModel = carModel;
                ((HashSet<Data.Complectation>)carModel.Complectation).Add(compl);
            }

            return carModel.Complectation;
        }
    }
}