using SecurityAdvisor.Infrastructure.Generic;
using static SecurityAdvisor.Infrastructure.Generic.OSAppsListAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class BackupAppDT : DetectionTechnique
    {
        //Поиск по ключевым словам: если в строке будет присутствовать ещё какая-нибудь информация типа версии или года, то это не испортит качество
        private List<string[]> backupAppNamesKeywordGroups = new List<string[]>()
        {
            //Кол-во ключевых слов может быть любым, как и групп ключевых слов
            new string[] {"True", "Image" },
            new string[] {"Kaspersky", "Total", "Security" },
            new string[] {"Backup"},
            new string[] {"Macrium", "Reflect" }
        };

        public override void Execute()
        {
            Status = IsAtLeastOneAppInstalledAlready(backupAppNamesKeywordGroups) ? DetectionStatus.NotFound : DetectionStatus.Found;
        }
    }
}
