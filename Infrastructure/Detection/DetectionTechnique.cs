using SecurityAdvisor.Infrastructure.Generic;
using SecurityAdvisor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure
{
    public abstract class DetectionTechnique
    {
        public DetectionStatus Status { get; set; } = DetectionStatus.Error; //Лучше сообщить об ошибке, чем говорить о наличии или отсутствии проблемы

        public abstract void Execute();
    }
}
