
namespace VladlenKazmiruk
{
    public class CarModel
    {
        public string? DateStart { get => dateStart; set => dateStart = value; }
        public string? DateEnd { get => dateEnd; set => dateEnd = value; }
        public string? ComplectationCode { get => complectationCode; set => complectationCode = value; }
        public int? Code { get => code; set => code = value; }
        public HashSet<Complectations>? Complectations { get => complectations; set => complectations = value; }

        int? code = null;
        string? dateStart = null;
        string? dateEnd = null;
        string? complectationCode = null;
        HashSet<Complectations>? complectations = null;
        
    }
}