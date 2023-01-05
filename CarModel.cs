
using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    public class CarModel : DomElementContainer
    {
        public string? DateStart { get => dateStart; set => dateStart = value; }
        public string? DateEnd { get => dateEnd; set => dateEnd = value; }
        public string? ComplectationCode { get => complectationCode; set => complectationCode = value; }
        public string? Code { get => code; set => code = value; }
        public HashSet<Complectation>? Complectations { get => complectation; set => complectation = value; }
        public Car? Car { get => car; set => car = value; }
        public string? Url { get => url; set => url = value; }

        string? code = null;
        string? url = null;
        string? dateStart = null;
        string? dateEnd = null;
        string? complectationCode = null;
        HashSet<Complectation>? complectation = null;
        Car? car;

        public CarModel(IElement domElement, Car? car = null) : base(domElement)
        {
            this.car = car;
        }
    }
}