
using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    namespace Data
    {
        public class Complectation
        {

            public CarModel? CarModel { get => carModel; set => carModel = value; }
            public string? DateRange { get => dateRange; set => dateRange = value; }
            public string? ModCode { get => modCode; set => modCode = value; }
            // Modifications
            public List<KeyValuePair<string, string>> ModData { get => data; set => data = value; }
            public string? Url { get => url; set => url = value; }

            string? modCode;
            string? dateRange;
            // Dictionary не используем так как key может повторяться
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>> ();

            CarModel? carModel;
            string? url = null;
            public override string ToString()
            {
                var strW = new StringWriter();

                strW.WriteLine($"Mod Code: {this.ModCode} | Date: {this.DateRange}");
                strW.WriteLine($"Data:");
                foreach (var item in this.ModData)
                {
                    strW.WriteLine($"\tkey = {item.Key}: {item.Value}");
                }
                return strW.ToString();
            }
        }
    }
}