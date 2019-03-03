using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Text.RegularExpressions;

namespace SecurityAdvisor.Infrastructure.Detection
{
    //Проверка билда конкретного выпуска/версии Windows 10 (для 1803, 1809 и прочих свои семейста билдов). По билду вычисляется актуальность установленных обновлений. 
    //Делается запрос в Интернет для выяснения актуальных билдов и если билд системы < текущего, то проблема найдена, если  = не найдена, если > - Error
    //Важно заметить: 
    //Не работает для Windows <10.
    class OutdatedW10UpdatesDT : DetectionTechnique
    {
        public const int W10_ACTUAL_VERSIONS_AMOUNT = 4; //Самый безопасных версий 2 - это последние, а поддерживает Microsoft 4 крупных обновления
        private const float LAST_W10_BUILD_NULL_VALUE = -2;
        private float LastW10Build { get; set; } = LAST_W10_BUILD_NULL_VALUE; //Получается из Интернета
        private DB db;

        public override void Execute()
        {
            db = DB.Load();

            if (db.WindowsUpdatesSite_DOM == DB.WINDOWS_UPDATES_SITE_DOM_NULL_VALUE)
                throw new Exception(Exceptions.MSG_DOM_NOT_INIT_IN_DB);
            if (db.LocalOSBuild == DB.LOCAL_OS_BUILD_NULL_VALUE)
                throw new Exception("Билд системы не был получен");
            if (WindowsVersionTooOldDT.IsNotW10OnComputer())
                throw new Exception("Данная система не является Windows 10 и проверить актуальность обновлений невозможно");

            double lastBuild = ParseW10LastBuild();
            if (lastBuild == db.LocalOSBuild)
                Status = DetectionStatus.NotFound;
            else if (lastBuild > db.LocalOSBuild)
                Status = DetectionStatus.Found;
            else
                throw new Exception(Exceptions.MSG_PARSING_FAILED_OR_BAD_SITE);
        }

        //Для каждой версии W10 своя таблица с билдами на сайте
        private double ParseW10LastBuild()
        {
            int h4Counter = 0; //Счётчик h4 тэгов. Он нужен для определения где находится таблица с нужными билдами. H4 тэги находятся над этими таблицами.
            //Учитывая, что до h4 тегов была лишь одна таблица, то индекс таблицы с билдами на сайте = h4Counter + 1.
            bool isW10VersionFound = false;
            for (;h4Counter < W10_ACTUAL_VERSIONS_AMOUNT; h4Counter ++) //кол-во таблиц с билдами = кол-ву актуальных версий W10
            {
                string h4Content = db.WindowsUpdatesSite_DOM["div > h4 > a"].Eq(h4Counter).Text();
                Match W10VersionMatch = Regex.Match(h4Content, WindowsVersionTooOldDT.W10_VERSION_REGEX_PATTERN);
                if (isW10VersionFound = int.Parse(W10VersionMatch.Value) == db.LocalOSVersion)
                    break;
            }
            if (!isW10VersionFound)
                throw new Exception("Парсер не нашёл на сайте таблицу с билдами текущей версии W10");

            int buildsTableIndex = h4Counter + 1; //см. h4Counter
            string buildsTable = db.WindowsUpdatesSite_DOM["div > table > tbody"].Eq(buildsTableIndex).Text();
            Match lastBuild = Regex.Match(buildsTable, @"\d{5,7}.\d{3,5}\s");

            return double.Parse(lastBuild.Value.Replace('.', ','));
        }
    }
}
