
using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    public class Complectation : DomElementContainer
    {

        public string? Body { get => body; set => body = value; }
        public string? Grade { get => grade; set => grade = value; }
        public string? Atm_mtm { get => atm_mtm; set => atm_mtm = value; }
        public string? Gear_shift_type { get => gear_shift_type; set => gear_shift_type = value; }
        public string? Driver_position { get => driver_position; set => driver_position = value; }
        public string? Doors_number { get => doors_number; set => doors_number = value; }
        public string? Destination1 { get => destination1; set => destination1 = value; }
        public string? Destination2 { get => destination2; set => destination2 = value; }
        public CarModel? CarModel { get => carModel; set => carModel = value; }

        string? body;
        string? grade;
        string? atm_mtm;
        string? gear_shift_type;
        string? driver_position;
        string? doors_number;
        string? destination1;
        string? destination2;

        CarModel? carModel;

        public Complectation(IElement domElement, CarModel? carModel = null) : base(domElement)
        {
            this.carModel = carModel;
        }
    }

}