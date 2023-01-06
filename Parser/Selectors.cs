using AngleSharp;
using VladlenKazmiruk;

namespace VladlenKazmiruk
{
    namespace Parser
    {
        public static class Selectors
        {
            static string topLevelSelector = ".Multilist";
            static string carCellSelector = ":scope > div[class='List']"; // :scope для topLevelSelector
            static string carNameSelector = "div[class='name']";
            static string carInfoSelector = "div[class='List'] div[class='List']";
            static string carDatesSelector = "div[class='dateRange']";
            static string carComplCodeSelector = "div[class='modelCode']";
        }
    }
}