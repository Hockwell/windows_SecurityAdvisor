using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
{
    class Exceptions
    {
        public const string MSG_W10_VERSION_NOT_PARSED = "Номер версии W10 не распознан на сайте.";
        public const string MSG_DOM_NOT_INIT_IN_DB = "DOM сайта не был получен";
        public const string MSG_PARSING_FAILED_OR_BAD_SITE = "Парсер нуждаеся в доработке или сайт об обновлениях более не актуален";
    }
}
