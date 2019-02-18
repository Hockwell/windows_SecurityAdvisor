using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
{
    static class UniversalMethods
    {
        //Метод возвращает подстроку из строки на основе опорного индекса строки, дистанции, на которой располагается начало подстроки от опорного индекса и длины подстроки
        public static string GetSubstringByReferencePoint(int referencePoint, string @string, int distanceFromReferencePoint, int substringLength)
        {
            string substring = string.Empty;

            for (int i = distanceFromReferencePoint; i > distanceFromReferencePoint - substringLength; i--)
            {
                substring += @string[referencePoint - i];
            }

            return substring;
        }

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

        public static void ShowConsoleMessagesAboutException(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
    }
}
