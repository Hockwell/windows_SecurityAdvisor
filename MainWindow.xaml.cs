using SecurityAdvisor.Infrastructure;
using SecurityAdvisor.Infrastructure.FileExport;
using SecurityAdvisor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecurityAdvisor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DB db;
        private List<WindowsOSProblem> problems;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Check_Btn_Click(object sender, RoutedEventArgs e)
        {
            LoadProblemsList();
            DetectProblems();
            DetectionReportTXTExport.SaveDetectionReport(problems);
        }

        private void LoadProblemsList()
        {
            db = DB.Load();
            problems = db.GetProblemsList();
        }

        private void DetectProblems()
        {
            for (int i = 0; i < problems.Count; i++)
            {
                problems[i].Detection.Execute();
            }
        }
    }
}
