using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class UEFISecureBootDT : DetectionTechnique
    {
        private const string PATH = @"SYSTEM\CurrentControlSet\Control\SecureBoot\State";
        private const string KEY = "UEFISecureBootEnabled";
        private const int SECURE_VALUE = 1;

        public override void Execute()
        {
            int enabled;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(PATH))
            {
                enabled = (int) key.GetValue(KEY);
            }

            if (IsSecureValue())
            {
                Status = Generic.DetectionStatus.NotFound;
            }
            else
            {
                Status = Generic.DetectionStatus.Found;
            }

            bool IsSecureValue() => enabled == SECURE_VALUE;
        }
    }
}
