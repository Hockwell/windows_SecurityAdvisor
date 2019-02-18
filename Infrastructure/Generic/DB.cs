using SecurityAdvisor.Infrastructure.Detection;
using static SecurityAdvisor.Infrastructure.Generic.UniversalMethods;
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
            return db ?? (db = new DB());
        }

        public List<WindowsOSProblem> GetProblemsList()
        {
            return problems;
        }

        public List<string> GetInstalledProgramsList()
        {
            return installedPrograms;
        }

        public bool IsActualTimeNotDetermined()
        {
            return ActualTime.Equals(ACTUAL_TIME_NULL_VALUE);
        }

        public bool IsOSBuildValuesNotDetermined()
        {
            return LocalOSBuild == OS_BUILD_NULL_VALUE || ActualOSBuild == ACTUAL_OS_BUILD_NULL_VALUE;
        }

        public bool IsOSVersionValuesNotDetermined()
        {
            return LocalOSVersion == OS_VERSION_NULL_VALUE || ActualOSVersions == ACTUAL_OS_VERSION_NULL_VALUE;

        }
        #endregion

        #region Data
        public const float OS_BUILD_NULL_VALUE = -1;
        public const float OS_VERSION_NULL_VALUE = -1;
        public const float ACTUAL_OS_BUILD_NULL_VALUE = -2;
        public static readonly float[] ACTUAL_OS_VERSION_NULL_VALUE = null;
        public static readonly DateTime ACTUAL_TIME_NULL_VALUE = new DateTime(0); //У DateTime нет null-значения, поэтому пришлось создать своё

        private List<WindowsOSProblem> problems;
        private List<string> installedPrograms; //Сохранено именно здесь, чтобы >1 техники могли пользоваться этой информацией
        public DateTime ActualTime { get; set; } = ACTUAL_TIME_NULL_VALUE; //Текущеее актуальное время, с помощью которого программа определяет ряд 
        //системных проблем, используя его как точку отсчёта. 
        //Формируется на основе данных из Интернета и установленного в системе путем экпертизы.
        public double LocalOSBuild { get; private set; } = OS_BUILD_NULL_VALUE; //17763.316, 17763.194...
        public float LocalOSVersion { get; private set; } = OS_VERSION_NULL_VALUE; //7,8, 1803, 1809...
        public float ActualOSBuild { get; private set; } = ACTUAL_OS_BUILD_NULL_VALUE;
        public float[] ActualOSVersions { get; private set; } = ACTUAL_OS_VERSION_NULL_VALUE;

        private DB()
        {
            GetNamesOfInstalledProgramsFromRegistry();
            InitProblemsList();
        }

        public void GetLocalOSBuildAndVersionFromRegistry()
        {
            LocalOSVersion = float.Parse(GetKeyValueFromRegistry(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId").ToString());
            string buildNumber = GetKeyValueFromRegistry(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber").ToString();
            string buildSuffix = GetKeyValueFromRegistry(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR").ToString();
            LocalOSBuild = double.Parse(buildNumber + "," + buildSuffix);
        }

        public void GetActualOSBuildAndVersionFromInternet()
        {
            WindowsUpdatesSiteParser parser = new WindowsUpdatesSiteParser();

            if (parser.Run()) //Если парсер отработал неудачно, то обновлять значения ни к чему.
            {
                ActualOSBuild = parser.ActualBuild;
                ActualOSVersions = parser.ActualVersions;
            }
        }

        private void GetNamesOfInstalledProgramsFromRegistry()
        {
            installedPrograms = AppsListSearchDT.GetInstalledAppNamesList();
        }

        private void InitProblemsList()
        {
            problems = new List<WindowsOSProblem>();

            problems.Add(new WindowsOSProblem //FalseTimeDT должна быть на нулевом месте списка, ибо она должна исполняться раньше всех
            {
                Name = "Неверное время в системе",
                AdviceForUser = "Установите правильно время, включите автоматическое определение часового пояса и времени в Параметрах/Панели управления Windows",
                Raiting = ProblemRaiting.Critical,
                Description = "Ряд служб ОС и антивирус могут работать неправильно, если время в системе установлено не корректное.",
                Detection = new FalseTimeDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Технология UEFI Secure boot не включена",
                AdviceForUser = @"Попытайтесь включить её в реестре: HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State, UEFISecureBootEnabled = 1. 
Не все производители UEFI поддерживают её.",
                Raiting = ProblemRaiting.Recomended,
                Description = "Технология осуществляет контроль подписей образа загружаемой ОС и драйверов. Если образ будет модифицирован вредоносным ПО (буткит, руткит), " +
                "то он не загрузится, поскольку вредоносный драйвер будет заблокирован. Предотвращается загрузка как неподписанных данных, так и данных из чёрного списка. ",
                Detection = new UEFISecureBootDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "BIOS не обновлялся более 2 лет",
                AdviceForUser = "Обновить BIOS. Пройдите на сайт производители в раздел Поддержка и следуйте инструкциям.",
                Raiting = ProblemRaiting.Info,
                Description = "BIOS важно обновлять для защиты от уязвимостей (например, «нашумевшей» Spectre) и обеспечения поддержки нового оборудования.",
                Detection = new OldBIOS_DT()
            });

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
                Name = "Не установлены последние обновления Windows",
                AdviceForUser = "Проверьте настройки, используя Панель управления или Параметры в Windows 10",
                Raiting = ProblemRaiting.Critical,
                Description = "Обновления нужны не только для повышения стабильности ОС, но и для защиты от угроз и эксплойтов. " +
                "Обновления должны устанавливаться как можно скорее",
                Detection = new OutdatedWindowsUpdatesDT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Не самая новая версия Windows",
                AdviceForUser = "W10: установите предпоследнее/последение крупное обновление. <W10: установите W10.",
                Raiting = ProblemRaiting.Recomended,
                Description = "Чем новее ОС, тем безопаснее. Зачастую невозможно в старые версии ОС включить новейшие " +
                "механизмы защиты. Более старые ОС в первую очередь поддерживаются лишь для реактивной защиты от эксплойтов " +
                "путём установки патчей исправления.",
                Detection = new WindowsVersionTooOldDT()
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

            problems.Add(new WindowsOSProblem
            {
                Name = "Не работает защита системных файлов",
                AdviceForUser = "Проверьте лицензионная ли у вас ОС. Включите UAC.",
                Raiting = ProblemRaiting.Critical,
                Description = "UAC позволяет контролировать доступ программ к системным программам и папкам. W10 даже с выключенным UAC это реализует." +
                "Пиратское ПО нарушает целостность лицензионного аналога и опасно внедрением шпионского ПО. ",
                Detection = new UnauthorizedAccessToOS_DT()
            });

            problems.Add(new WindowsOSProblem
            {
                Name = "Установлен Adobe Flash",
                AdviceForUser = "На всякий случай лучше удалить данное ПО из системы, тем более, что оно не имеет смысла, однако может стать причиной атаки. ",
                Raiting = ProblemRaiting.Info,
                Description = "Данное ПО позволяет воспроизводить веб-медиа-контент, однако часто используется злоумышленниками для совершения зловредных действий в " +
                "системе минуя браузер. Менее 4% сайтов сегодня используют Flash, а браузеры отказываются воспроизводить соответствующий контент с помощью Flash по " +
                "умолчанию во избежание проблем.",
                Detection = new AdobeFlashDT()
            });
        }

        #endregion
    }
}
