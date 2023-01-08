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
            public IEnumerable<Complectation> Complectations { get => complectations; set => complectations = value; }
            public Car? Car { get => car; set => car = value; }
            public string? Url { get => url; set => url = value; }

            string? code = null;
            string? dateRange = null;
            string? complectationCode = null;
            IEnumerable<Complectation> complectations = Enumerable.Empty<Complectation>();
            Car? car;
            string? url = null;

            public override string ToString()
            {
                var strW = new StringWriter();
                  strW.WriteLine($"Model Code: {this.Code} | Date: {this.DateRange} | {this.ComplectationCode}");
                  strW.WriteLine($"{this.Url}");
                  strW.WriteLine($"Complectations:");
                  foreach (var compl in this.complectations)
                  {
                    strW.WriteLine($"\t{compl.ToString()}");
                  }

                  return strW.ToString();
            }
        }
    }
}