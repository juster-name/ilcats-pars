using AngleSharp;

namespace VladlenKazmiruk
{
    public class Car
    {
        
        public string? Name { get => name; set => name = value; }
        public HashSet<CarModel>? Models { get => models; set => models = value; }

        private HashSet<CarModel>? models = null;
        string? name = null;
    }
}