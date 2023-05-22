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
using System.Diagnostics;
using System.Net.Http;
using MySql.Data.MySqlClient;
using System.IO;
using System.Security.Policy;
using System.Threading;
using System.ServiceProcess;

namespace AutoInstaller.View
{
    /// <summary>
    /// Interaction logic for RunnigControl.xaml
    /// </summary>
    public partial class RunnigControl : UserControl
    {
        private string _strApachePath;
        private string _strMySqlPath;
        public string _connectionString = "server=localhost;user id=root;password=;database=tbapi";
        public string _sqlFilePath;
        public string _url = "http://localhost/";
        private MainWindow  _mainWindow;
        public RunnigControl(MainWindow mainWindow, string targetPath)
        {
            InitializeComponent();
            _strApachePath = targetPath + @"\apache\bin";
            _strMySqlPath = targetPath + @"\mysql\bin";
            _sqlFilePath = targetPath + @"\project\TB\SQL\tbhost.sql";
            _mainWindow = mainWindow;
            
            InitButtons();
        }
        
        private void Btn_Run_Click(object sender, RoutedEventArgs e)
        {
            //run apache service 
            if (!isRunnigApache())
            {
                if (!Run_ApacheSevice())
                {
                    MessageBox.Show("Runnig Apache Service failed.");
                    return;
                }
                InitButtons();
            }
            //
            if (!isRunningMySql())
            {
                if (!Run_MysqlService())
                {
                    MessageBox.Show("Runnig Mysql failed.");
                    return;
                }
                InitButtons();
            }

            Run_Apache();
            
            Restore_MySqlDatabase();
            
            Run_Url();

        }

        private void InitButtons()
        {
            if (!isRunnigApache())
            {
                Btn_Apache.Content = "Start";
                Apache_Status.Text = "";
                
            }
            else
            {
                Btn_Apache.Content = "Stop";
                Apache_Status.Text = "Running";
            }
            if(!isRunningMySql())
            {
                Btn_Mysql.Content = "Start";
                MySql_Status.Text = "";
            }
            else
            {
                Btn_Mysql.Content = "Stop";
                MySql_Status.Text = "Running";
            }
        }

        private bool isRunnigApache()
        {
            Process[] processes = Process.GetProcessesByName("httpd");
            if (processes.Length > 0)
            {
                // process is running
                return true;
            }
            
            return false;
        }

        private bool isRunningMySql()
        {
            Process[] processes = Process.GetProcessesByName("mysqld");
            if (processes.Length > 0)
            {
                // process is running
                return true;
            }
            return false;
        }

        private bool Run_ApacheSevice()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "httpd.exe";
                startInfo.Arguments = "-k install";
                Thread.Sleep(1000);
                startInfo.WorkingDirectory = _strApachePath;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;

                startInfo.Verb = "runas";

                Process.Start(startInfo);
                startInfo.Arguments = "-k start";
                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void refrshControl()
        {
            InitButtons();
        }

        private bool Run_MysqlService()
        {
            try
            {
                using (Process process = new Process()) {
                   
                    



                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "mysqld.exe";
                    startInfo.Arguments = "-install";
                    Thread.Sleep(1000);
                    startInfo.WorkingDirectory = _strMySqlPath;
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = true;

                    startInfo.Verb = "runas";

                    Process.Start(startInfo);
                    startInfo.Arguments = "-start";
                    Process.Start(startInfo);

                    Thread.Sleep(3000);


                    ServiceController mysqlService = new ServiceController("MySQL");
                    if (mysqlService.Status == ServiceControllerStatus.Stopped)
                    {
                        mysqlService.Start();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private bool Restore_MySqlDatabase()
        {
            try
            {

                Thread.Sleep(1000);
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = File.ReadAllText(_sqlFilePath);

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void Btn_Restore_Click(object sender, RoutedEventArgs e)
        {
            if (Restore_MySqlDatabase())
            {
                MessageBox.Show("Success restore");
            }
        }

        private void Run_Url()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(_url)
            {
                UseShellExecute = true,
                Verb = "open"
            };

            Process.Start(startInfo);
        }

        private void killProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }

        private void Run_Apache()
        {
            if (isRunnigApache())
            {
                killProcess("httpd");
                Btn_Apache.Content = "Start";
                Apache_Status.Text = "";
            }
            else
            {
                if (!Run_ApacheSevice())
                {
                    MessageBox.Show("Runnig Apache Service failed.");
                    return;
                }
                Btn_Apache.Content = "Stop";
                Apache_Status.Text = "Running";
            }
        }

        private void Btn_Apache_Click(object sender, RoutedEventArgs e)
        {
            Run_Apache();
        }

        private void Btn_Mysql_Click(object sender, RoutedEventArgs e)
        {
            if(isRunningMySql())
            {
                killProcess("mysqld");
                Btn_Mysql.Content = "Start";
                MySql_Status.Text = "";
            }
            else
            {
                if (!Run_MysqlService())
                {
                    MessageBox.Show("Runnig Mysql failed.");
                    return;
                }
                Btn_Mysql.Content = "Stop";
                MySql_Status.Text = "Running";
            }
        }
    }
}
