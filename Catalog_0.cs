namespace VladlenKazmiruk
{
    public class Catalog_0
    {
        public string? Name { get => name; set => name = value; }
        public int? Complectation_id { get => complectation_id; set => complectation_id = value; }
        public HashSet<Catalog_1>? Catalogs_1 { get => catalogs_1; set => catalogs_1 = value; }
        public Complectation? Complectation { get => complectation; set => complectation = value; }

        string? name;
        int? complectation_id;
        HashSet<Catalog_1>? catalogs_1;
        Complectation? complectation;
    }
}