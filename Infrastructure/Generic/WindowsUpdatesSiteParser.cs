using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
{
    class WindowsUpdatesSiteParser
    {
        private const string URI = @"https://winreleaseinfoprod.blob.core.windows.net/winreleaseinfoprod/en-US.html";

        public float ActualBuild { get; set; }
        public float[] ActualVersions { get; set; } //Их 2, на февраль 2019 1803 и 1809

        public void GetAndParse()
        {
            WebRequest request = WebRequest.Create(URI);
            //using ()
        }
    }
}
