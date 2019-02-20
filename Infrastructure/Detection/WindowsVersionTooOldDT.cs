using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    //Для Windows 10 проверяется выпуск (версия): 1803, 1809...
    //Если Windows <10, то проблема считается найденной, ибо W10 сегодня самая безопасная (проверка на 7, 8, 8.1). 
    class WindowsVersionTooOldDT : DetectionTechnique
    {
        public const int W10_UPDATES_BRANCHES_AMOUNT = 3; //Current Branch (CB), LTSB, Current Branch for Business (CBB)
        public const int W10_ACTUAL_VERSIONS_AMOUNT = 2; //1809, 1803, например на момент 19 февраля 2019
        private static readonly float[] SUPPORTED_OLD_OS_VERSIONS = new float[] { 7, 8, 8.1f };
        private static readonly List<int> ACTUAL_W10_VERSION_NULL_VALUE = null;
        private List<int> ActualW10Versions { get; set; } = ACTUAL_W10_VERSION_NULL_VALUE; //Получается из Интернета
        private DB db;

        public override void Execute()
        {
            db = DB.Load();

            if (db.WindowsUpdatesSite_DOM == DB.WINDOWS_UPDATES_SITE_DOM_NULL_VALUE)
                throw new Exception(Exceptions.MSG_DOM_NOT_INIT_IN_DB);
            if (db.LocalOSVersion == DB.LOCAL_OS_VERSION_NULL_VALUE)
                throw new Exception("Билд системы не был получен");

            if (IsNotW10OnComputer())
            {
                Status = DetectionStatus.Found;
                return;
            }

            ParseW10ActualVersions();

            if (IsActualW10OnComputer())
            {
                Status = DetectionStatus.NotFound;
            }
            else if (IsNotActualW10OnComputer())
            {
                Status = DetectionStatus.Found;
            }
            else //actual1 < version < actual2, version > actual2 
            {
                Status = DetectionStatus.Error;
                throw new Exception("Парсер сломался или сайт об обновлениях более не актуален");
            }

            bool IsNotW10OnComputer() => SUPPORTED_OLD_OS_VERSIONS.Contains(db.LocalOSVersion);
            bool IsActualW10OnComputer() => ActualW10Versions.Contains((int)db.LocalOSVersion);
            bool IsNotActualW10OnComputer() => db.LocalOSVersion < ActualW10Versions.Min();
        }

        private void ParseW10ActualVersions()
        {
            string versionsTable = db.WindowsUpdatesSite_DOM["div > table > tbody"].Eq(0).Text();
            Match w10VersionMatch = Regex.Match(versionsTable, @"\s\d{4}\s");
            if (!w10VersionMatch.Success)
                throw new Exception(Exceptions.MSG_W10_VERSION_NOT_PARSED);
            ActualW10Versions = new List<int>(W10_ACTUAL_VERSIONS_AMOUNT);

            int lastVersion = int.Parse(w10VersionMatch.Value); //Верхняя версия в таблице - последняя выпущенная
            ActualW10Versions.Add(lastVersion);

            FindAndParsePenultimateVersion(w10VersionMatch, lastVersion); //Ищем вторую актуальную версию W10 - ниже последней выпущенной версии
        }

        private void FindAndParsePenultimateVersion(Match w10VersionMatch, int lastVersion)
        {
            for (int i = 0; i < (W10_ACTUAL_VERSIONS_AMOUNT - 1) * W10_UPDATES_BRANCHES_AMOUNT; i++) //-1, ибо одно значение уже нашли
            {
                w10VersionMatch = w10VersionMatch.NextMatch();
                if (!w10VersionMatch.Success)
                    throw new Exception(Exceptions.MSG_W10_VERSION_NOT_PARSED);
                int penultimateVersion = int.Parse(w10VersionMatch.Value);
                if (lastVersion < penultimateVersion)
                {
                    throw new Exception("Не верно определены версии W10.");
                }
                else if (lastVersion > penultimateVersion)
                {
                    ActualW10Versions.Add(penultimateVersion);
                    break;
                }
            }
        }
    }
}
