using AngleSharp;
using VladlenKazmiruk;

namespace VladlenKazmiruk
{
    namespace Parser
    {
        public static class Selectors
        {
            public static string carTopLevelSelector = ".Multilist";
            public static string carCellSelector = ":scope > div[class='List']"; // :scope для topLevelSelector
            public static string carNameSelector = "div[class='name']";
            public static string carInfoTopSelector = ":scope > div[class='List ']"; // : scope для carCellSelector
            public static string carInfoSelector =  "div[class='List']";
            public static string carDatesSelector = "div[class='dateRange']";
            public static string carComplCodeSelector = "div[class='modelCode']";
        }
    }
}