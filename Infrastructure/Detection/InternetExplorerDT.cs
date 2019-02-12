using SecurityAdvisor.Infrastructure.Generic;
using static SecurityAdvisor.Infrastructure.Generic.OSAppsListAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class InternetExplorerDT : DetectionTechnique
    {
        private List<string[]> internetExplorerKeywords = new List<string[]>()
        {
            new string[] {"Internet", "Explorer" },
        };
        public override void Execute()
        {
            Status = IsAtLeastOneAppInstalledAlready(internetExplorerKeywords) ? DetectionStatus.NotFound : DetectionStatus.Found;
        }
    }
}
