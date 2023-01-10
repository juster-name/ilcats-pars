// See https://aka.ms/new-console-template for more information
using AngleSharp;
using AngleSharp.Dom;
using System.Collections.Concurrent;
using VladlenKazmiruk.Parser;

// TODO: Несколько значений из Complectation subquery в insert, откуда-то дупликаты.
// Загрузить документы для парсинга в файл, а не ддосить их сайт каждый раз.

namespace VladlenKazmiruk
{
    public static class TestParse
    {
        public static IConfiguration config = Configuration.Default.WithDefaultLoader();
        public static string carsAddress = "https://www.ilcats.ru/toyota&language=ru/";

        public static IBrowsingContext context = BrowsingContext.New(config);
        public static IDocument document = context.OpenAsync(carsAddress).WaitAsync(CancellationToken.None).Result;


        public static ICatcher<string> carModelNameCatcher = new Parser.CarModelNameCatcher(document.GetElementById("Body"));
        public static ICatcher<Data.CarModel> carModelCatcher = new Parser.CarModelInfoCatcher(null);
        public static ICatcher<Data.Complectation> complCatcher = new Parser.ComplCatcher(null);


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
            var buffCarModels = new HashSet<Data.CarModel>();

            foreach (Data.CarModel carModel in TestParse.carModelCatcher.Catch())
            {
                carModel.Car = car;
                carModel.Name = carModelName;

                var newUrl = Urls.baseFull + Urls.formComplectationPath("toyota", "EU", carModel.Code, carModel.DateRange); 

                var docCompl = context.OpenAsync(newUrl).WaitAsync(CancellationToken.None).Result;
                var el = docCompl.GetElementById("Body");
                complCatcher.changeContext(el);

                buffCarModels.Add(carModel);
            }

            car.Models = buffCarModels;

            return car.Models;
        }

        static IEnumerable<Data.Complectation> parseComplectations(Data.CarModel carModel)
        {
            var buffCompls = new HashSet<Data.Complectation>();

            foreach (Data.Complectation compl in TestParse.complCatcher.Catch())
            {
                compl.CarModel = carModel;
                buffCompls.Add(compl);
            }
            carModel.Complectation = buffCompls;

            return carModel.Complectation;
        }
    }
}