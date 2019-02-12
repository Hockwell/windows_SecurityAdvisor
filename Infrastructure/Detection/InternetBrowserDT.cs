using SecurityAdvisor.Infrastructure.Generic;
using static SecurityAdvisor.Infrastructure.Generic.OSAppsListAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class InternetBrowserDT : DetectionTechnique
    {
        private List<string[]> browserNamesKeywordGroups = new List<string[]>()
        {
            new string[] {"Chrome" },
            new string[] {"Opera" },
            new string[] {"Firefox" },
            new string[] {"Chromium" },
            new string[] {"Edge"},
            new string[] {"Yandex", "Browser"}
        };
        //Edge невозможно удалить в W10, в более ранних ОС он наверное присутствует в списке программ

        public override void Execute()
        {
            Status = IsAtLeastOneAppInstalledAlready(browserNamesKeywordGroups) ? DetectionStatus.NotFound : DetectionStatus.Found;
        }
    }
}
