using System;
using System.IO;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class UnauthorizedAccessToOS_DT : DetectionTechnique
    {
        //Пытаетмся создать в защищённой системной папке новую папку. Мы должны получить исключение, если система защищена. 
        //С выключенным UAC Windows 10 также не позволяет получить доступ к данным папкам.

        private const string PATH_TO_BAD_FOLDER = @"C:\Windows\System32\bad";

        public override void Execute()
        {
            //true - protected, false - not protected
            Status = CheckSystemFilesProtection() ? Generic.DetectionStatus.NotFound : Generic.DetectionStatus.Found; 
        }

        private bool CheckSystemFilesProtection()
        {
            try
            {
                Directory.CreateDirectory(PATH_TO_BAD_FOLDER);
                return false; //исключение строчка выше не вызвала
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }
    }
}
