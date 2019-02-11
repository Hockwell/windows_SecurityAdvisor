using SecurityAdvisor.Infrastructure.Generic;
using SecurityAdvisor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SecurityAdvisor.Infrastructure.FileExport
{
    static class DetectionReportTXTExport
    {
        #region Consts
        private const string FILE_NAME = "Report_by_SecurityAdvisor.txt";
        #endregion

        public static void SaveDetectionReport(List<WindowsOSProblem> problems)
        {
            using (FileStream file = new FileStream(FILE_NAME, FileMode.Create))
            using (TextWriter writer = new StreamWriter(file, Encoding.Default))
            {
                writer.WriteLine("-------- НАЙДЕННЫЕ ПРОБЛЕМЫ --------");
                PrintInfoAboutProblem(problems, writer, printCondition: problem => problem.Detection.Status == DetectionStatus.Found);

                writer.WriteLine("-------- НЕ УДАЛОСЬ ПРОВЕРИТЬ --------");
                PrintInfoAboutProblem(problems, writer, printCondition: problem => problem.Detection.Status == DetectionStatus.Error);

                writer.WriteLine("-------- НЕНАЙДЕННЫЕ ПРОБЛЕМЫ --------");
                PrintInfoAboutProblem(problems, writer, printCondition: problem => problem.Detection.Status == DetectionStatus.NotFound);
            }

            System.Diagnostics.Process.Start(FILE_NAME);
        }

        private static void PrintInfoAboutProblem(List<WindowsOSProblem> problems, TextWriter writer, Func<WindowsOSProblem, bool> printCondition)
        {
            for (int i = 0; i < problems.Count; i++)
            {
                if (!printCondition(problems[i]))
                    continue;

                writer.WriteLine("Проблема: " + TidyTextLenght(problems[i].Name));
                writer.WriteLine("Описание: " + TidyTextLenght(problems[i].Description));
                writer.WriteLine("Серьёзность: " + TidyTextLenght(problems[i].Raiting.ToString()));
                writer.WriteLine("Совет: " + TidyTextLenght(problems[i].AdviceForUser));

                writer.WriteLine("---");
            }
            writer.WriteLine("");
        }

        private static string TidyTextLenght(string input)
        {
            int lengthLimit = 80;
            string output = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                output += input[i];
                if (i != 0 && i % lengthLimit == 0 && i != input.Length - 5)
                    output += Environment.NewLine;
            }

            return output;
        }
    }
}
