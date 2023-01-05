using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    public class Catalog_1 : DomElementContainer
    {
        public string? Name { get => name; set => name = value; }
        public Catalog_0? Catalog_0 { get => catalog_0; set => catalog_0 = value; }

        string? name;
        Catalog_0? catalog_0;

        public Catalog_1(IElement domElement, Catalog_0? catalog_0 = null) : base(domElement)
        {
            this.catalog_0 = catalog_0;
        }
    }
}