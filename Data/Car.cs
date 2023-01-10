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
            public string? MarketCode { get => marketCode; set => marketCode = value; }
            public string? Market { get => market; set => market = value; }

            private IEnumerable<CarModel> models = Enumerable.Empty<CarModel>();
            string? name = null;
            string? marketCode = null;
            string? market = null;
            string? path = null; 

            public override string ToString()
            {
                var strW = new StringWriter();

                strW.WriteLine($"Car Name: {this.Name}");
                strW.WriteLine($"Market:  {this.Name} ({this.MarketCode})");
                 foreach (Data.CarModel carModel in this.Models)
                 {
                     strW.WriteLine($"\t{carModel.ToString()}");
                 }

                 return strW.ToString();
            }
        }
    }
}