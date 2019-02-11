using SecurityAdvisor.Infrastructure.Detection;
using SecurityAdvisor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
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

        public List<string> GetInstalledProgramsList()
        {
            return installedPrograms;
        }
        #endregion

        #region Data
        private List<WindowsOSProblem> problems;
        private List<string> installedPrograms; //Сохранено именно здесь, чтобы >1 техники могли пользоваться этой информацией

        private DB()
        {
            InitProblemsList();
            InitInstalledProgramsList();
        }

        private void InitProblemsList()
        {
            problems = new List<WindowsOSProblem>();

            problems.Add(new WindowsOSProblem //Не следует в текст вставлять escape-последовательности для выравнивания в 
                                              //файлах txt - он выравнивается с помощью специального метода
            {
                Name = "Антивирус не защищает ОС",
                AdviceForUser = "Проверьте установлен ли антивирус, включён ли он",
                Raiting = ProblemRaiting.Critical,
                Description = "Антивирус обязательно должен быть установлен для предотвращения повреждения и кражи данных. " +
                "Некоторые антивирусы могут с опозданием отреагировать на размещение тестового файла." +
                "Если после окончания работы программы файл таки будет обнаружен, проблемы нет.",
                Detection = new AntivirusDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Отсутствует известная система резервного копирования",
                AdviceForUser = "Проверьте установлена ли система резервного копирования",
                Raiting = ProblemRaiting.Info,
                Description = "Резервные копии позволяют избежать потери данных вследствие сбоев или повреждений вредоносным ПО. " +
                "Рекомендуемые программы: Acronis, ",
                Detection = new BackupAppDT()
            });

            /*
            problems.Add(new WindowsOSProblem
            {
                Name = "Опасные настройки службы обновления Windows",
                AdviceForUser = "Проверьте настройки, используя Панель управления или Параметры в Windows 10",
                Raiting = ProblemRaiting.Critical,
                Description = "Обновления нужны не только для повышения стабильности ОС, но и для защиты от угроз и эксплойтов. " +
                "Обновления должны устанавливаться как можно скорее",
                Detection = new UpdatesServiceDT()
            });
            */
        }

        private void InitInstalledProgramsList()
        {
            installedPrograms = UniversalStaticMethods.GetInstalledAppNamesList();
        }
        #endregion
    }
}
