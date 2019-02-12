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
            InitInstalledProgramsList();
            InitProblemsList();
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
                "Некоторые антивирусы могут с опозданием отреагировать на размещение тестового файла. " +
                "Если после окончания работы программы файл таки будет обнаружен, проблемы нет.",
                Detection = new AntivirusDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Отсутствует известная система резервного копирования",
                AdviceForUser = "Проверьте установлена ли система резервного копирования",
                Raiting = ProblemRaiting.Info,
                Description = "Резервные копии позволяют избежать потери данных вследствие сбоев или повреждений вредоносным ПО. " +
                "Рекомендуется использовать копирование по расписанию, а не фоновое - 1, 2 - в облако, а не локальное, чтобы не подтвергать резервные копии воздействию " +
                "Ransomware, сбоев хранилищ или ошибок ОС. BackUp-программа отличается от клиента облачной синхронизации (OneDrive, GoogleDrive...) наличием " +
                "продвинутых настроек и дополнительными защитными возможностями. Последние могут подвергаться атакам Ransomware, как минимум. OneDrive защищает от Ransomware " +
                "путём сохранения состояний файлов (механизм отката), но для этого нужна premium-подписка.",
                Detection = new BackupAppDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Небезопасная разрядность",
                AdviceForUser = "Поменяйте процессор на 64bit-ный, либо установите 64bit-ную версию ОС",
                Raiting = ProblemRaiting.Info,
                Description = "Все современные процессоры – с 64bit-ной архитектурой, " +
                "в современных процессорах реализованы аппаратные методы защиты от угроз " +
                "и эксплойтов. 64bit-ная версия ОС безопаснее 32bit-ной (по крайней мере, это было актуально во времена Windows Vista). ",
                Detection = new OSBitDepthDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Не установлен безопасный браузер",
                AdviceForUser = "Перенесите свои закладки в один из указанных браузеров и используйте как основной именно его",
                Raiting = ProblemRaiting.Recomended,
                Description = "Один из следующих браузеров должен использоваться как основной для надёжной защиты " +
                "системы: Google Chrome, Opera, Chromium, Edge, FireFox, Яндекс браузер. Данные браузеры постоянно " +
                "обновляются и имеют слаженный оперативный процесс поиска и исправления уязвимостей.",
                Detection = new TrustedInternetBrowsersDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Неверное время в системе",
                AdviceForUser = "Установите правильно время, включите автоматическое определение часового пояса и времени в Параметрах/Панели управления Windows",
                Raiting = ProblemRaiting.Recomended,
                Description = "Ряд служб ОС и антивирус могут работать неправильно, если время в системе установлено не корректное.",
                Detection = new TimeDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Опасные настройки службы обновления Windows",
                AdviceForUser = "Проверьте настройки, используя Панель управления или Параметры в Windows 10",
                Raiting = ProblemRaiting.Critical,
                Description = "Обновления нужны не только для повышения стабильности ОС, но и для защиты от угроз и эксплойтов. " +
                "Обновления должны устанавливаться как можно скорее",
                Detection = new UpdatesServiceDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Установлено ПО Java для исполнения программ на данном языке",
                AdviceForUser = "Удалите его из системы, если вы не используйте программы на Java",
                Raiting = ProblemRaiting.Info,
                Description = "Большое кол-во вредоносного ПО написано на Java и плохо обнаруживается антивирусным ПО. Если есть возможность," +
                "лучше себя обезопасить от исполнения подобного ПО.",
                Detection = new JavaExecutionDT()
            });
        }

        private void InitInstalledProgramsList()
        {
            installedPrograms = AppsListSearchDT.GetInstalledAppNamesList();
        }
        #endregion
    }
}
