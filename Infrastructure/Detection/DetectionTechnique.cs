using SecurityAdvisor.Infrastructure.Generic;

namespace SecurityAdvisor.Infrastructure
{
    public abstract class DetectionTechnique
    {
        public DetectionStatus Status { get; set; } = DetectionStatus.Error; //Лучше сообщить об ошибке, чем говорить о наличии или отсутствии проблемы

        public abstract void Execute();
    }
}
