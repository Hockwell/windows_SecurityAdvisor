using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure.Generic
{
    static class UniversalStringMethods
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
    }
}
