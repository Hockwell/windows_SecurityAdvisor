using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
{
    public static class UniversalStaticMethods
    {
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
                            appsNames.Add("-- Error --");
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
        public static bool IsAtLeastOneAppInstalledAlready(List<string[]> appNameKeywordGroups)
        {
            List<string> installedAppsNames = DB.Load().GetInstalledProgramsList();

            int indexOf_funcValue = 0;
            int keywordsCounter = 0; //фиксирует кол-во найденных слов группы в строке

            foreach (var appName in installedAppsNames)
            {
                if (appName == null)
                    continue;

                foreach (string[] keywordsGroup in appNameKeywordGroups)
                {
                    keywordsCounter = 0;
                    for (int i = 0; i < keywordsGroup.Length; i++)
                    {
                        indexOf_funcValue = appName.IndexOf(keywordsGroup[i], StringComparison.CurrentCultureIgnoreCase); //IndexOf возвращает -1, если ничего не нашёл. 
                        //Если хотя бы одно ключевое слово группы не найдено в строке, значит пропускаем 
                        //оставшиеся ключевые слова группы и переходим к следующей группе. 
                        if (IsKeywordNotContainedInAppNameString())
                            break;
                        else
                            keywordsCounter ++;
                    }
                    if (IsAllKeywordsGroupContainedInAppNameString())
                        return true;

                    bool IsKeywordNotContainedInAppNameString() => indexOf_funcValue == -1;
                    bool IsAllKeywordsGroupContainedInAppNameString() => keywordsCounter == keywordsGroup.Length;
                }
            }
            return false;
        }
    }

    
}
