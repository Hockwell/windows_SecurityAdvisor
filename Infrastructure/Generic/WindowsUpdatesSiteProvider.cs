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

            Match lastUpdateDateMatch = Regex.Match(div, @"\d{1,2}/\d{2}/\d{4}");
            if (!lastUpdateDateMatch.Success)
                throw new Exception("Регулярное выражение не обнаружило дату на сайте");
            string[] MM_dd_yyyy = lastUpdateDateMatch.Value.Split('/'); //Именно так из-за того, что месяц и день могут писаться одной цифрой на сайте, а не двумя через 0: 01, 02...
            //Если использовать парсинг DateTime строки, то там придётся указывать либо две цифры, либо одну, а может быть и то, и другое:
            // 2/19/2019, 2/2/2019, 14/3/2019... Инициализация через конструктор поддерживает такие различия
            DateTime lastUpdateDate = new DateTime(year: int.Parse(MM_dd_yyyy[2]), month: int.Parse(MM_dd_yyyy[0]), day: int.Parse(MM_dd_yyyy[1]));
        
            DB db = DB.Load();
            if (db.IsActualTimeNotDetermined())
                throw new Exception("Время не было определено средствами FalseTimeDT");

            double timeDifference_hours = CalcTimesDifferenceInHours(lastUpdateDate, db.ActualTime);
            if (timeDifference_hours >= halfYear_hours)
                throw new Exception("Сайт для проверки обновлённости ОС не актуален");
        }

    }
}
