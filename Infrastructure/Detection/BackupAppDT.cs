using System.Collections.Generic;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class BackupAppDT : AppsListSearchDT
    {
        protected override List<string[]> KeywordGroupsForSearch => new List<string[]>()
        {
            //Кол-во ключевых слов может быть любым, как и групп ключевых слов
            new string[] {"True", "Image" },
            new string[] {"Kaspersky", "Total", "Security" },
            new string[] {"Backup"},
            new string[] {"Macrium", "Reflect" }
        };
    }
}
