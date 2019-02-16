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
    class SuspiciousBIOSDateDT : DetectionTechnique
    {
        private const string PATH = @"SYSTEM\CurrentControlSet\Control\SystemInformation";
        private const string KEY = "BIOSReleaseDate";
        private const int TWO_YEARS = 365*2;

        public override void Execute()
        {
            string biosDate = (string) GetKeyValueFromRegistry(PATH, KEY);
            DateTime biosDate_DT = DateTime.ParseExact(biosDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            if (IsTrueDateDifference())
                Status = DetectionStatus.NotFound;
            else
                Status = DetectionStatus.Found;

            bool IsTrueDateDifference() => CalcTimesDifferenceInDays(DB.Load().ActualTime, biosDate_DT) <= TWO_YEARS;
        }
    }
}
