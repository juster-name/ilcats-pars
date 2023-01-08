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
        static class Urls
        {
            public static string baseFull = "https://www.ilcats.ru";
            public static string toyota= "/toyota";

            public static string getCars = "/?function=getModels";
            public static string getCompl = "/?function=getComplectations";

            public static string marketArg = "&market=";
            public static string modelArg = "&model=";
            public static string startDateArg = "&startDate=";
            public static string endDateArg = "&endDate=";
            public static string yearArg = "$year=";

            public static string formComplectationUrl(string carType, string market, string? idModel, string? dateRange)
            {
                if (idModel is null)
                    throw new NullReferenceException("idModel must not be null to form an URL");

                var dateArgs = dateRange is null ? "" : parseDateRangeToArgs(dateRange);

                return baseFull + $"/{carType}" + getCompl + 
                $"{marketArg}{market}" + $"{modelArg}{idModel}" + dateArgs;
            }

            public static string parseDateRangeToArgs(string dateRange)
            {
                int year = -1;
                if(dateRange.Length == 4 && int.TryParse(dateRange, out year))
                {
                    return yearArg + year.ToString();
                }

                var dateStart = DateOnly.MinValue;
                var dateEnd = DateOnly.MinValue;
                var splitDates = dateRange.Split(separator: " - ");

                DateOnly.TryParse(splitDates[0], out dateStart);

                if (dateStart == DateOnly.MinValue)
                    return "";

                string retStr = startDateArg + dateStart.Year + 
                    (dateStart.Month < 10 ? "0" + dateStart.Month : dateStart.Month);

                if (splitDates.Length > 1)
                    DateOnly.TryParse(splitDates[1], out dateEnd);
                
                if (dateEnd == DateOnly.MinValue)
                    return retStr;

                return retStr + endDateArg + dateEnd.Year +
                    (dateEnd.Month < 10 ? "0" + dateEnd.Month : dateEnd.Month);;
            }
        }
    }
}