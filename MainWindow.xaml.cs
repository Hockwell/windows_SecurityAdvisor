using SecurityAdvisor.Infrastructure;
using SecurityAdvisor.Infrastructure.FileExport;
using SecurityAdvisor.Infrastructure.Generic;
using static SecurityAdvisor.Infrastructure.Generic.UniversalMethods;
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
using System.IO;

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
            CleanupExcessFiles();

            Check_Btn.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;

            DelegateForAsync asyncJob = new DelegateForAsync(MainJob);
            asyncJob.BeginInvoke(null, null);

            
        }

        private void CleanupExcessFiles()
        {
            File.Delete(Exceptions.ERRORS_LOG_FILE_NAME);
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
                Exceptions.PrintInfoAboutException(e);
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
            //Зачем отделять FalseTimeDT от других проблем и запускать раньше всех? По той причине, что на основе актуального времени работают подготовительные 
            //операции в БД и после этого выполняются остальные детектирующие техники

            if (!(problems[0].Detection is Infrastructure.Detection.FalseTimeDT))
                throw new Exception("FalseTimeDT должно содержаться в БД на 0 месте - его инфа нужна остальным техникам и вспомогательным операциям");

            DetectProblem(0);

            try
            {
                db.GetLocalOSBuildAndVersionFromRegistry();
                db.GetLastOSBuildAndActualVersionFromInternet();
            }
            catch (Exception e)
            {
                Exceptions.PrintInfoAboutException(e);
            }

            for (int i = 1; i < problems.Count; i++)
            {
                DetectProblem(i);
            }
        }

        private void DetectProblem(int problemIndex)
        {
            try
            {
                problems[problemIndex].Detection.Execute();
            }
            catch (Exception e)
            {
                Exceptions.PrintInfoAboutException(e);
                problems[problemIndex].Detection.Status = DetectionStatus.Error;
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
