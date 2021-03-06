﻿using Microsoft.Win32;
using System;

namespace SecurityAdvisor.Infrastructure.Generic
{
    static class UniversalMethods
    {
        public static double CalcTimesDifferenceInHours(DateTime time1, DateTime time2)
        {
            return new TimeSpan(Math.Abs(time1.Ticks - time2.Ticks)).TotalHours;
        }

        public static object GetKeyValueFromRegistry(string path, string key)
        {
            object value;
            using (RegistryKey branch = Registry.LocalMachine.OpenSubKey(path))
            {
                value = branch.GetValue(key);
            }
            return value;
        }
    }
}
