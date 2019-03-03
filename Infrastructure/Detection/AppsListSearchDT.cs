using Microsoft.Win32;
using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;

namespace SecurityAdvisor.Infrastructure.Detection
{
    abstract class AppsListSearchDT : DetectionTechnique
    {
        private const string APP_NAME_BY_ERROR = "-- Error --";

        //Поиск по ключевым словам: если в строке будет присутствовать ещё какая-нибудь информация типа версии или года, то это не испортит качество
        protected abstract List<string[]> KeywordGroupsForSearch { get; }

        public override void Execute()
        {
            Status = IsAtLeastOneAppInstalledAlready() ? DetectionStatus.NotFound : DetectionStatus.Found;
        }

        public static List<string> GetInstalledAppNamesList()
        {
            List<string> appsNames = new List<string>();

            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            string displayName = sk.GetValue("DisplayName") as string;
                            appsNames.Add(displayName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                            appsNames.Add(APP_NAME_BY_ERROR);
                        }
                    }
                }
            }

            return appsNames;
        }

        //Метод ищет по ключевым словам наличие определённых программ (одной программы, если нужно) в списке установленных программ. Если хотя бы одна
        //присутствует - возвращает true. Группа ключевых слов отвечает за конкретную программу, её название без учёта фирмы разбито на ключевые слова 
        //для улучшения качества поиска. 
        //Принцип: Метод ищет строки в списке строк, где есть группы ключевых слов, одна группа или одно слово-выражение, если находит хотя бы одну такую - возвращает true. 
        //Поиск не зависит от регистра
        protected bool IsAtLeastOneAppInstalledAlready()
        {
            List<string> installedAppsNames = DB.Load().GetInstalledProgramsList();

            if (installedAppsNames == null) 
            {
                throw new Exception("installedAppsNames is null");
            }

            int keywordsCounter = 0; //фиксирует кол-во найденных слов группы в строке

            foreach (string appName in installedAppsNames)
            {
                if (IsBadAppName())
                    continue;

                foreach (string[] keywordsGroup in KeywordGroupsForSearch)
                {
                    keywordsCounter = 0;
                    for (int i = 0; i < keywordsGroup.Length; i++)
                    {
                        //IndexOf возвращает -1, если ничего не нашёл. 
                        //Если хотя бы одно ключевое слово группы не найдено в строке, значит пропускаем 
                        //оставшиеся ключевые слова группы и переходим к следующей группе. 
                        if (appName.IndexOf(keywordsGroup[i], StringComparison.CurrentCultureIgnoreCase) == -1)
                            break;
                        else
                            keywordsCounter++;
                    }
                    if (IsAllKeywordsGroupContainedInAppNameString())
                        return true;

                    bool IsAllKeywordsGroupContainedInAppNameString() => keywordsCounter == keywordsGroup.Length;
                }

                bool IsBadAppName() => appName == null || appName == APP_NAME_BY_ERROR;
            }
            return false;
        }
    }
}

