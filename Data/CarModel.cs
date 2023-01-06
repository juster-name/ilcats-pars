using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    namespace Data
    {
        public class CarModel
        {
            public string? DateRange { get => dateRange; set => dateRange = value; }
            public string? ComplectationCode { get => complectationCode; set => complectationCode = value; }
            public string? Code { get => code; set => code = value; }
            public IEnumerable<Complectation> Complectations { get => complectation; set => complectation = value; }
            public Car? Car { get => car; set => car = value; }
            public string? Url { get => url; set => url = value; }

            string? code = null;
            string? url = null;
            string? dateRange = null;
            string? complectationCode = null;
            IEnumerable<Complectation> complectation = Enumerable.Empty<Complectation>();
            Car? car;
    }
    }
}