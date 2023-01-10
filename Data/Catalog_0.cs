using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    namespace Data
    {
        public class Catalog_0
        {
            public string? Name { get => name; set => name = value; }
            //public string? Complectation_id { get => complectation_id; set => complectation_id = value; }
            public IEnumerable<Catalog_1> Catalogs_1 { get => catalogs_1; set => catalogs_1 = value; }
            public Complectation? Complectation { get => complectation; set => complectation = value; }

            string? name;
            //string? complectation_id;
            IEnumerable<Catalog_1> catalogs_1 = Enumerable.Empty<Catalog_1>();
            Complectation? complectation;
        }
    }
}