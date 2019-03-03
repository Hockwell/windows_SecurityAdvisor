using SecurityAdvisor.Infrastructure.Generic;
using System.Collections.Generic;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class JavaExecutionDT : AppsListSearchDT
    {
        protected override List<string[]> KeywordGroupsForSearch => new List<string[]>()
        {
            new string[] {"Java"}
        };

        public override void Execute()
        {
            Status = IsAtLeastOneAppInstalledAlready() ? DetectionStatus.Found : DetectionStatus.NotFound;
        }
    }
}
