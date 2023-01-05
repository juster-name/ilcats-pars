using AngleSharp;
using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    public class Car : DomElementContainer
    {
        
        public string? Name { get => name; set => name = value; }
        public IEnumerable<CarModel>? Models { get => models; set => models = value; }

        private IEnumerable<CarModel>? models = null;
        string? name = null;

        public Car(IElement domElement) : base(domElement)
        {
        }
    }
}