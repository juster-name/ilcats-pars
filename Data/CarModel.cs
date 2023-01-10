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
            public IEnumerable<Complectation> Complectation { get => complectation; set => complectation = value; }
            public Car? Car { get => car; set => car = value; }
            public string? Url { get => url; set => url = value; }
            public string? Name { get => name; set => name = value; }

            string? code = null;
            string? name = null;
            string? dateRange = null;
            string? complectationCode = null;
            IEnumerable<Complectation> complectation = Enumerable.Empty<Complectation>();
            Car? car;
            string? url = null;

            public override string ToString()
            {
                var strW = new StringWriter();
                  strW.WriteLine($"Code: {this.ComplectationCode} | Date: {this.DateRange} | Model Code: {this.Code}");
                  strW.WriteLine($"{this.Url}");
                  strW.WriteLine($"Modifications:");
                  foreach (var compl in this.complectation)
                  {
                    strW.WriteLine($"\t{compl.ToString()}");
                  }

                  return strW.ToString();
            }
        }
    }
}