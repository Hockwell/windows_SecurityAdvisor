using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class TrustedInternetBrowsersDT : AppsListSearchDT
    {
        protected override List<string[]> AppNamesKeywordGroups => new List<string[]>()
        {
            new string[] {"Chrome" },
            new string[] {"Opera" },
            new string[] {"Firefox" },
            new string[] {"Chromium" },
            new string[] {"Edge"},
            new string[] {"Yandex", "Browser"}
        };
        //Edge невозможно удалить в W10, в более ранних ОС он наверное присутствует в списке программ
    }
}
