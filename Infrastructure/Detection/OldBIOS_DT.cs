using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Globalization;
using static SecurityAdvisor.Infrastructure.Generic.UniversalMethods;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class OldBIOS_DT : DetectionTechnique
    {
        private const string PATH = @"SYSTEM\CurrentControlSet\Control\SystemInformation";
        private const string KEY = "BIOSReleaseDate";
        private const int TWO_YEARS = 365*2*24; //часы

        public override void Execute()
        {
            DB db = DB.Load();

            if (db.IsActualTimeNotDetermined())
                throw new Exception("Для работы данной техники нужно запустить FalseTimeDT раньше, а не позже!");

            string biosDate = (string) GetKeyValueFromRegistry(PATH, KEY);
            DateTime biosDate_DT = DateTime.ParseExact(biosDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            if (IsTrueDateDifference())
                Status = DetectionStatus.NotFound;
            else
                Status = DetectionStatus.Found;

            bool IsTrueDateDifference() => CalcTimesDifferenceInHours(db.ActualTime, biosDate_DT) <= TWO_YEARS;
        }
    }
}
