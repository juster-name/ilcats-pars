
using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    namespace Data
    {
        public class Complectation
        {

            public CarModel? CarModel { get => carModel; set => carModel = value; }
            public string? DateRange { get => dateRange; set => dateRange = value; }
            public string? Code { get => code; set => code = value; }
            public Dictionary<string, string> Data { get => data; set => data = value; }

            string? code;
            string? dateRange;
            Dictionary<string, string> data = new Dictionary<string, string>();

            CarModel? carModel;

            public override string ToString()
            {
                var strW = new StringWriter();

                strW.WriteLine($"Code: {this.Code}, Date: {this.DateRange}");
                strW.WriteLine("Data:");
                foreach (var item in this.Data)
                {
                    strW.WriteLine($"\tkey = {item.Key} | Value = {item.Value}");
                }
                return strW.ToString();
            }
        }
    }
}