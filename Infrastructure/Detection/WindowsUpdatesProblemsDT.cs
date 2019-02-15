using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class WindowsUpdatesProblemsDT : RegistryDT
    {
        public override void Execute()
        {
            DateTime actualTime = DB.Load().ActualTime;
            if (actualTime.Equals(DB.NULL_TIME))
                throw new Exception("Для работы данной техники нужно запустить BadTimeDT раньше, а не позже!");

        }
    }
}
