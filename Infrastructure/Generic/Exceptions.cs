using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
{
    class Exceptions
    {
        public const string ERRORS_LOG_FILE_NAME = "errors.txt";

        public const string MSG_W10_VERSION_NOT_PARSED = "Номер версии W10 не распознан на сайте.";
        public const string MSG_DOM_NOT_INIT_IN_DB = "DOM сайта не был получен";
        public const string MSG_PARSING_FAILED_OR_BAD_SITE = "Парсер нуждаеся в доработке или сайт об обновлениях более не актуален";

        public static void PrintInfoAboutException(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            File.AppendAllText(ERRORS_LOG_FILE_NAME, e.Message + ": " + e.StackTrace + Environment.NewLine + Environment.NewLine);
        }
    }
}
