using AngleSharp;
using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    namespace Data
    {
        public class Car
        {

            public string? Name { get => name; set => name = value; }
            public IEnumerable<CarModel> Models { get => models; set => models = value; }

            public static string topLevelSelector = ".Multilist";
            public static string carCellSelector = ":scope > div[class='List']"; // :scope для topLevelSelector

            public static string carNameSelector = "div[class='name']";
            public static string carInfoSelector = "div[class='List'] div[class='List']";
            public static string carDatesSelector = "div[class='dateRange']";
            public static string carComplCodeSelector = "div[class='modelCode']";

            private IEnumerable<CarModel> models = Enumerable.Empty<CarModel>();
            string? name = null;
        }
    }
}