using CsQuery;
using System;
using System.Collections.Generic;
using static SecurityAdvisor.Infrastructure.Generic.UniversalMethods;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
{
    class WindowsUpdatesSiteParser
    {
        private const string URI = @"https://winreleaseinfoprod.blob.core.windows.net/winreleaseinfoprod/en-US.html";

        public float ActualBuild { get; set; }
        public float[] ActualVersions { get; set; } //Их 2, на февраль 2019 1803 и 1809

        private CQ dom;

        public bool Run()
        {
            try
            {
                GetPageInHTML();
                CheckSiteRelevance();
                //ParseActualVersions();
                return true;
            }
            catch (Exception e)
            {
                ShowConsoleMessagesAboutException(e);
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
            dom = CQ.Create(html);

            return true;
        }

        private void CheckSiteRelevance() //Проверка поля сайта Last Update. Если дата в нём отличается более, чем на пол года, то сайт не актуален и данные не загружаем
        {
            string stringForSearch = "Last Updated: ";
            int halfYear_hours = (365 / 2) * 24;
            string div = dom["div"].Text();

            int referenceIndex = div.IndexOf(stringForSearch) + stringForSearch.Length;
            int dateStringEndIndex = div.IndexOf('\n', referenceIndex);
            string dateString = div.Substring(referenceIndex, dateStringEndIndex - referenceIndex);
            string[] MM_dd_yyyy = dateString.Split('/');
            DateTime lastUpdateDate = new DateTime(year: int.Parse(MM_dd_yyyy[2]), month: int.Parse(MM_dd_yyyy[0]), day: int.Parse(MM_dd_yyyy[1]));

            DB db = DB.Load();
            if (db.IsActualTimeNotDetermined())
                throw new Exception("Время не было определено средствами FalseTimeDT");

            double timeDifference_hours = CalcTimesDifferenceInHours(lastUpdateDate, db.ActualTime);
            if (timeDifference_hours >= halfYear_hours)
                throw new Exception("Сайт для проверки обновлённости ОС не актуален");
        }

        private void ParseActualVersions()
        {
            string versionsTable = dom["div > table > tbody"].Text();

        }
    }
}
