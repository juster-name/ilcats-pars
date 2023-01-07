using AngleSharp;
using VladlenKazmiruk;


namespace VladlenKazmiruk
{
    namespace Parser
    {
        public static class Selectors
        {
            public static string carTopLevel = ".Multilist";
            public static string carCell = ":scope > div[class='List']"; // :scope для topLevel
            public static string carName = "div[class='name']";

            public static string carModelTop = ":scope > div[class='List ']"; // : scope для carCell
            public static string carModel =  "div[class='List']";
            public static string carModelDates = "div[class='dateRange']";
            public static string carModelComplCode = "div[class='modelCode']";
            
            public static string complTop = "tbody";
            public static string complCell = "tr:nth-child(n+2)";
            public static string complDataNames = "th:nth-child(n+3)";
            public static string complDataValues = "td:nth-child(n+3) div";

            public static string dateRange = "div[class='dateRange']";
            public static string code = "a";
        }
    }
}