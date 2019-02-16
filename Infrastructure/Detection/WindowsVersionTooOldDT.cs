using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    //Для Windows 10 проверяется выпуск (версия): 1803, 1809...
    //Если Windows <10, то проблема считается найденной, ибо W10 сегодня самая безопасная. 
    class WindowsVersionTooOldDT : DetectionTechnique
    {
        public override void Execute()
        {
            
        }
    }
}
