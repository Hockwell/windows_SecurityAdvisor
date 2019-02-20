using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    //Проверка билда конкретного выпуска/версии Windows 10 (для 1803, 1809 и прочих свои семейста билдов). По билду вычисляется актуальность установленных обновлений. 
    //Делается запрос в Интернет для выяснения актуальных билдов и если билд системы < текущего, то проблема найдена, если  = не найдена, если > - Error
    //Не работает для Windows <10.
    class OutdatedWindowsUpdatesDT : DetectionTechnique
    {
        private const float LAST_W10_BUILD_NULL_VALUE = -2;
        private float LastW10Build { get; set; } = LAST_W10_BUILD_NULL_VALUE; //Получается из Интернета
        private DB db;

        public override void Execute()
        {
            db = DB.Load();

            if (db.WindowsUpdatesSite_DOM == DB.WINDOWS_UPDATES_SITE_DOM_NULL_VALUE)
                throw new Exception(Exceptions.MSG_DOM_NOT_INIT_IN_DB);
            if (db.LocalOSVersion == DB.LOCAL_OS_VERSION_NULL_VALUE)
                throw new Exception("Билд системы не был получен");

        }

        //Для каждой версии W10 своя таблица с билдами на сайте
        private void ParseW10LastBuild()
        {
            if (db.LocalOSBuild == DB.LOCAL_OS_BUILD_NULL_VALUE)
                throw new Exception("Билд ОС компьютера до сих пор не найден. Без него невозможно найти последний билд с суффиксом на сайте");

            string buildsTable = db.WindowsUpdatesSite_DOM["div > table > tbody"].Eq(0).Text();
            //Match lastW10Build = Regex.Match()
        }
    }
}
