using SecurityAdvisor.Infrastructure.Detection;
using SecurityAdvisor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure
{
    public class DB
    {
        #region API
        private static DB db;

        public static DB Load()
        {
            return db ?? new DB();
        }

        public List<WindowsOSProblem> GetProblemsList()
        {
            return problems;
        }
        #endregion

        #region Data
        private List<WindowsOSProblem> problems;
        private DB()
        {
            problems = new List<WindowsOSProblem>(1);

            problems.Add(new WindowsOSProblem //Не следует в текст вставлять escape-последовательности для выравнивания в 
                //файлах txt - он выравнивается с помощью специального метода
            {
                Name = "Антивирус не защищает ОС",
                AdviceForUser = "Проверьте установлен ли антивирус, включён ли он",
                Raiting = ProblemRaiting.Critical,
                Description = "В современных реалиях антивирус обязательно должен быть установлен для предотвращения повреждения и кражи данных",
                Detection = new AntivirusDT()
            });
        }
        #endregion
    }
}
