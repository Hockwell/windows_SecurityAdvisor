using CsQuery;
using System;
using System.Collections.Generic;
using static SecurityAdvisor.Infrastructure.Generic.UniversalMethods;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SecurityAdvisor.Infrastructure.Detection;

namespace SecurityAdvisor.Infrastructure.Generic
{
    class WindowsUpdatesSiteParser
    {
        public const int W10_UPDATES_BRANCHES_AMOUNT = 3; //Current Branch (CB), LTSB, Current Branch for Business (CBB)
        private const string EXC_MSG_W10_VERSION_NOT_PARSED = "Номер версии W10 не распознан на сайте.";
        private const string URI = @"https://winreleaseinfoprod.blob.core.windows.net/winreleaseinfoprod/en-US.html";

        private CQ dom;
        private DB db = DB.Load();

        public void ParseAndInitDBFields()
        {
            try
            {
                GetPageInHTML();
                CheckSiteRelevance();
                ParseActualVersions();
            }
            catch (Exception e)
            {
                ShowConsoleMessagesAboutException(e);
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

            Match lastUpdateDateMatch = Regex.Match(div, @"\d{1,2}/\d{2}/\d{4}");
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

        private void ParseActualVersions()
        {
            string versionsTable = dom["div > table > tbody"].Eq(0).Text();
            Match m = Regex.Match(versionsTable, @"\s\d{4}\s");
            if (!m.Success)
                throw new Exception(EXC_MSG_W10_VERSION_NOT_PARSED);
            db.ActualW10Versions = new List<int>(WindowsVersionTooOldDT.W10_ACTUAL_VERSIONS_AMOUNT);

            int lastVersion = int.Parse(m.Value); //Верхняя версия в таблице - последняя выпущенная
            db.ActualW10Versions.Add(lastVersion);

            FindAndParsePenultimateVersion(m, lastVersion); //Ищем вторую актуальную версию W10 - ниже последней выпущенной версии
        }

        private void FindAndParsePenultimateVersion(Match m, int lastVersion)
        {
            for (int i = 0; i < (WindowsVersionTooOldDT.W10_ACTUAL_VERSIONS_AMOUNT - 1) * W10_UPDATES_BRANCHES_AMOUNT; i++) //-1, ибо одно значение уже нашли
            {
                m = m.NextMatch();
                if (!m.Success)
                    throw new Exception(EXC_MSG_W10_VERSION_NOT_PARSED);
                int penultimateVersion = int.Parse(m.Value); 
                if (lastVersion < penultimateVersion)
                {
                    throw new Exception("Не верно определены версии W10.");
                }
                else if (lastVersion > penultimateVersion)
                {
                    db.ActualW10Versions.Add(penultimateVersion);
                    break;
                }
            }
        }
    }
}
