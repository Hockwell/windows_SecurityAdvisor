using CsQuery;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using static SecurityAdvisor.Infrastructure.Generic.UniversalMethods;

namespace SecurityAdvisor.Infrastructure.Generic
{
    class WindowsUpdatesSiteProvider
    {
        private const string URI = @"https://winreleaseinfoprod.blob.core.windows.net/winreleaseinfoprod/en-US.html";

        public CQ DOM { get; set; } //document object modele
        private DB db = DB.Load();

        public bool GetSiteDOMAndCheckSiteRelevance()
        {
            try
            {
                GetPageInHTML();
                CheckSiteRelevance();
                return true;
            }
            catch (Exception e)
            {
                Exceptions.PrintInfoAboutException(e);
                return false;
            }
        }

        private bool GetPageInHTML()
        {
            string html;

            WebRequest request = WebRequest.Create(URI);
            request.Proxy = null;

            using (WebResponse response = request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                html = reader.ReadToEnd();
            }
            DOM = CQ.Create(html);

            return true;
        }

        private void CheckSiteRelevance() //Проверка поля сайта Last Update. Если дата в нём отличается более, чем на пол года, то сайт не актуален и данные не загружаем
        {
            int halfYear_hours = (365 / 2) * 24;
            string div = DOM["div"].Text();

            Match lastUpdateDateMatch = Regex.Match(div, @"\d{4}-\d{2}-\d{2}"); //2019-04-16 (ISO 8601)
            if (!lastUpdateDateMatch.Success)
                throw new Exception("Регулярное выражение не обнаружило дату на сайте");
            string[] date = lastUpdateDateMatch.Value.Split('-');
            DateTime lastUpdateDate = new DateTime(year: int.Parse(date[0]), month: int.Parse(date[1]), day: int.Parse(date[2]));
        
            DB db = DB.Load();
            if (db.IsActualTimeNotDetermined())
                throw new Exception("Время не было определено средствами FalseTimeDT");

            double timeDifference_hours = CalcTimesDifferenceInHours(lastUpdateDate, db.ActualTime);
            if (timeDifference_hours >= halfYear_hours)
                throw new Exception("Сайт для проверки обновлённости ОС не актуален");
        }

    }
}
