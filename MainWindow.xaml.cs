using SecurityAdvisor.Infrastructure;
using SecurityAdvisor.Infrastructure.FileExport;
using SecurityAdvisor.Infrastructure.Generic;
using SecurityAdvisor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private delegate void DelegateForAsync();

        private DB db;
        private List<WindowsOSProblem> problems;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Check_Btn_Click(object sender, RoutedEventArgs e)
        {
            Check_Btn.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;

            DelegateForAsync asyncJob = new DelegateForAsync(MainJob);
            asyncJob.BeginInvoke(null, null);

            
        }

        private void MainJob()
        {
            try
            {
                LoadProblemsList();
                DetectProblems();
                DetectionReportTXTExport.SaveDetectionReport(problems);
                UpdateGUI();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Возникла ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Console.WriteLine(e.StackTrace);
                CloseApp();
            }
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
                try
                {
                    problems[i].Detection.Execute();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    problems[i].Detection.Status = DetectionStatus.Error;
                }
                
            }
        }

        private void UpdateGUI()
        {
            Action action = () => { progressBar.Visibility = Visibility.Hidden; Check_Btn.IsEnabled = true; CloseApp(); };
            Dispatcher.BeginInvoke(action);
        }

        private void CloseApp()
        {
            Environment.Exit(1);
        }
    }
}
