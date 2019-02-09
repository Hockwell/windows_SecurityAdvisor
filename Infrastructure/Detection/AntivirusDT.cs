using SecurityAdvisor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class AntivirusDT : DetectionTechnique
    {
        #region Consts
        private const string EICAR_STRING = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
        private const string TEST_FILE_NAME = "eicar.txt";
        #endregion

        public override void Execute()
        {
            try
            {
                PlaceTestFile();
                CheckFileAvailability();
            }
            catch (UnauthorizedAccessException e)
            {
                Status = DetectionStatus.NotFound; //расчёт на то, что именно антивирус блокирует доступ к файлу, а значит систему защищена
                //(такое возможно при повторном запуске программы уже после обнаружения антивирусом файла)
            }
        }

        private void PlaceTestFile()
        {
            File.WriteAllText(TEST_FILE_NAME, EICAR_STRING, Encoding.Default);
        }

        private void CheckFileAvailability()
        {
            int loop = 0;
            int loopMax = 5;

            do
            {
                Thread.Sleep(1000);
                loop ++;
            }
            while (File.Exists(TEST_FILE_NAME) && loop < loopMax); //~5 секунд ждём удаления файла

            Status = (loop == loopMax) ? DetectionStatus.Found : DetectionStatus.NotFound;

        }
    }
}
