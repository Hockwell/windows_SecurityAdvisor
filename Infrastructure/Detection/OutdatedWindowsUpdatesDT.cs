using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    //Проверка билда конкретного выпуска/версии Windows 10 (для 1803, 1809 и прочих свои семейста билдов). По билду вычисляется актуальность установленных обновлений. 
    //Делается запрос в Интернет для выяснения актуальных билдов и если билд системы < текущего, то проблема найдена, если  = не найдена, если > - Error
    //Не работает для Windows <10.
    class OutdatedWindowsUpdatesDT : DetectionTechnique
    {
        public override void Execute()
        {
            DB db = DB.Load();
            if (db.IsOSBuildValuesNotDetermined())
            {
                throw new Exception("OSBuildValuesNotDetermined");
            }

        }
    }
}
