using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DateTime actualTime = DB.Load().ActualTime;
            if (actualTime.Equals(DB.NULL_TIME))
                throw new Exception("Для работы данной техники нужно запустить BadTimeDT раньше, а не позже!");

            string biosDate = (string) GetKeyValueFromRegistry(PATH, KEY);
            DateTime biosDate_DT = DateTime.ParseExact(biosDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            if (IsTrueDateDifference())
                Status = DetectionStatus.NotFound;
            else
                Status = DetectionStatus.Found;

            bool IsTrueDateDifference() => CalcTimesDifferenceInHours(actualTime, biosDate_DT) <= TWO_YEARS;
        }
    }
}
