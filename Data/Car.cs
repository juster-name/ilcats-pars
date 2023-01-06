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
        }
    }
}