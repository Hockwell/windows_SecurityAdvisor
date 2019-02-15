using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class AdobeFlashDT : AppsListSearchDT
    {
        protected override List<string[]> KeywordGroupsForSearch => new List<string[]>()
        {
            new string[] {"Flash", "Adobe"}
        };

        public override void Execute()
        {
            Status = IsAtLeastOneAppInstalledAlready() ? DetectionStatus.Found : DetectionStatus.NotFound;
        }
    }
}
