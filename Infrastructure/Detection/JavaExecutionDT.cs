using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class JavaExecutionDT : AppsListSearchDT
    {
        protected override List<string[]> AppNamesKeywordGroups => new List<string[]>()
        {
            new string[] {"Java"}
        };

        public override void Execute()
        {
            Status = IsAtLeastOneAppInstalledAlready() ? DetectionStatus.Found : DetectionStatus.NotFound;
        }
    }
}
