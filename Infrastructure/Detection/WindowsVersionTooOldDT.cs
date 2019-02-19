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

            bool IsNot10Version = SUPPORTED_OLD_OS_VERSIONS.Contains(db.LocalOSVersion);
            bool IsActualW10Version = db.ActualW10Versions.Contains((int)db.LocalOSVersion);


            if (IsNot10Version)
            {
                Status = DetectionStatus.Found;
            }
            else if (IsActualW10Version)
            {
                Status = DetectionStatus.NotFound;
            }
        }
    }
}
