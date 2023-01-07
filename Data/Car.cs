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
            private IEnumerable<CarModel> models = Enumerable.Empty<CarModel>();
            string? name = null;

            public override string ToString()
            {
                var strW = new StringWriter();

                strW.WriteLine($"{this.Name}");
                 foreach (Data.CarModel carModel in this.Models)
                 {
                     strW.WriteLine($"\t{carModel.ToString()}");
                 }

                 return strW.ToString();
            }
        }
    }
}