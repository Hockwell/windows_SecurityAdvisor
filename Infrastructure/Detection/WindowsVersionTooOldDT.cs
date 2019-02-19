using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    //Для Windows 10 проверяется выпуск (версия): 1803, 1809...
    //Если Windows <10, то проблема считается найденной, ибо W10 сегодня самая безопасная (проверка на 7, 8, 8.1). 
    class WindowsVersionTooOldDT : DetectionTechnique
    {
        public const int W10_ACTUAL_VERSIONS_AMOUNT = 2; //1809, 1803, например на момент 19 февраля 2019
        private static readonly float[] SUPPORTED_OLD_OS_VERSIONS = new float[] { 7, 8, 8.1f };

        public override void Execute()
        {
            DB db = DB.Load();
            if (db.IsOSVersionValuesNotDetermined())
            {
                throw new Exception("OSVersionValuesNotDetermined");
            }

            if (IsNotW10OnComputer())
            {
                Status = DetectionStatus.Found;
            }
            else if (IsActualW10OnComputer())
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
            bool IsActualW10OnComputer() => db.ActualW10Versions.Contains((int)db.LocalOSVersion);
            bool IsNotActualW10OnComputer() => db.LocalOSVersion < db.ActualW10Versions.Min();
        }
    }
}
