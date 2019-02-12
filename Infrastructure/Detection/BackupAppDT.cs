using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class BackupAppDT : AppsListSearchDT
    {
        protected override List<string[]> AppNamesKeywordGroups => new List<string[]>()
        {
            //Кол-во ключевых слов может быть любым, как и групп ключевых слов
            new string[] {"True", "Image" },
            new string[] {"Kaspersky", "Total", "Security" },
            new string[] {"Backup"},
            new string[] {"Macrium", "Reflect" }
        };
    }
}
