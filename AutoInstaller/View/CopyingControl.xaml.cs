using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Markup;
using System.ServiceProcess;
using System.Configuration.Install;

namespace AutoInstaller.View
{
    /// <summary>
    /// Interaction logic for CopyingControl.xaml
    /// </summary>
    public partial class CopyingControl : UserControl
    {
        
        MainWindow _mainwindow;
        string _sourcePath;
        string _targetPath;

        string _sourceApachePath;
        string _targetApachePath;
        string _sourceMySqlPath;
        string _targetMySqlPath;
        string _sourceProjectPath;
        string _targetProjectPath;
        string _sourcePhpPath;
        string _targetPhpPath;

        int _totalSize;
        int _currentSize = 0;
        int _progressBar = 0;
        int _totalProgress = 0;
        int _totalCurrentSize = 0;
        int _totalCopySize = 0;
        int _step;
        //Thread _copyThread;
        public CopyingControl(MainWindow mainWindow, string sourcePath, string targetPath)
        {
            

            _mainwindow = mainWindow;
            _sourcePath = sourcePath;
            _targetPath = targetPath;

            _sourceApachePath = _sourcePath + @"\apache";
            _sourceMySqlPath = _sourcePath + @"\mysql";
            _sourceProjectPath = _sourcePath + @"\project";
            _sourcePhpPath = _sourcePath + @"\php";

            _targetApachePath = _targetPath + @"\apache";
            _targetMySqlPath = _targetPath + @"\mysql";
            _targetProjectPath = _targetPath + @"\project";
            _targetPhpPath = _targetPath + @"\php";


            killApacheService();
            killMySQLService();

            InitializeComponent();

            Btn_Next.IsEnabled = false;
        }
        
        private void killApacheService()
        {
            try
            {
                string apacheServiceName = "";
                
                ServiceController[] services = ServiceController.GetServices();
               
                foreach (ServiceController service in services)
                {
                    if (service.DisplayName.StartsWith("Apache"))
                    {
                        apacheServiceName = service.ServiceName;
                        ServiceInstaller serviceInstaller = new ServiceInstaller();
                        serviceInstaller.ServiceName = apacheServiceName;

                        // Create a TransactedInstaller to install the ServiceInstaller
                        TransactedInstaller transactedInstaller = new TransactedInstaller();
                        transactedInstaller.Installers.Add(serviceInstaller);

                        // Uninstall the Apache service
                        transactedInstaller.Context = new InstallContext();
                        transactedInstaller.Uninstall(null);
                        break;
                    }
                   
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void killMySQLService()
        {
            try
            {
                string apacheServiceName = "";

                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    if (service.DisplayName.StartsWith("MySQL"))
                    {
                        apacheServiceName = service.ServiceName;
                        ServiceInstaller serviceInstaller = new ServiceInstaller();
                        serviceInstaller.ServiceName = apacheServiceName;

                        // Create a TransactedInstaller to install the ServiceInstaller
                        TransactedInstaller transactedInstaller = new TransactedInstaller();
                        transactedInstaller.Installers.Add(serviceInstaller);

                        // Uninstall the Apache service
                        transactedInstaller.Context = new InstallContext();
                        transactedInstaller.Uninstall(null);
                        break;
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Dowork()
        {
            try
            {
                _totalCurrentSize = 0;
                _totalCopySize = getTotalSize(_sourcePath);

                Dispatcher.Invoke(() =>
                {
                    TBox_Status.Text = "Copying Apache...";
                });
                _currentSize = 0;
                _progressBar = 0;
                _step = 1;
                _totalSize = getTotalSize(_sourceApachePath);
                CopyDirectory(_sourceApachePath, _targetApachePath);
                Dispatcher.Invoke(() =>
                {
                    TBox_Status.Text = "Copying Mysql...";
                });

                _currentSize = 0;
                _progressBar = 0;
                _step = 2;
                _totalSize = getTotalSize(_sourceMySqlPath);
                CopyDirectory(_sourceMySqlPath, _targetMySqlPath);


                Dispatcher.Invoke(() =>
                {
                    TBox_Status.Text = "Copying Php...";
                });

                _currentSize = 0;
                _progressBar = 0;
                _step = 3;
                _totalSize = getTotalSize(_sourcePhpPath);
                CopyDirectory(_sourcePhpPath, _targetPhpPath);

                Dispatcher.Invoke(() =>
                {
                    TBox_Status.Text = "Copying Project...";
                });

                _currentSize = 0;
                _progressBar = 0;
                _step = 4;
                _totalSize = getTotalSize(_sourceProjectPath);
                CopyDirectory(_sourceProjectPath, _targetProjectPath);

                Dispatcher.Invoke(() =>
                {
                    TBox_Status.Text = "Completed.";
                });

                Dispatcher.Invoke(() =>
                {
                    //Btn_Install.IsEnabled = true;
                    Btn_Next.IsEnabled = true;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Copying.");
                Btn_Install.IsEnabled = true;
                Btn_Next.IsEnabled= false;
            }

            
        }

        

        private int getTotalSize(string path)
        {

            //int size = Directory.GetFiles(path).Length;
            int size = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
            return size;

        }

        private void Btn_Install_Click(object sender, RoutedEventArgs e)
        {
            //COPY
            //CopyDirectory(_sourcePath, _targetPath);
            //_mainwindow.SetRunnigService();
            if (isRunnigApache())
            {
                killProcess("httpd");
            }
            if (isRunningMySql())
            {
                killProcess("mysqld");
            }

            if (System.IO.Directory.Exists(_targetPath))
            {
                MessageBoxResult result = MessageBox.Show("Project already exists. Would you like to overwrite?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    // User clicked Yes
                    TotalCopyProgress.Value = 100;
                    ApacheProgress.Value = 100;
                    MysqlProgress.Value = 100;
                    ProjectProgress.Value = 100;
                    PhpProgress.Value = 100;
                    TBox_Status.Text = "Completed.";
                    Btn_Install.IsEnabled = false;
                    Btn_Next.IsEnabled = true;
                    return;
                }
                
                
            }

            Btn_Install.IsEnabled = false;
            Thread _thread = new Thread(new ThreadStart(Dowork));
            _thread.Start();



            //end copy

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

        private void killProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!System.IO.Directory.Exists(targetDir))
            {
                System.IO.Directory.CreateDirectory(targetDir);
            }

            foreach (string file in System.IO.Directory.GetFiles(sourceDir))
            {
                string targetFile = System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(file));
                
                
                System.IO.File.Copy(file, targetFile, true);
                _currentSize++;
                _totalCurrentSize++;
                int tmp = _currentSize * 100 / _totalSize;
                int tmpCopy = _totalCurrentSize * 100 / _totalCopySize;
                
                Dispatcher.Invoke(() =>
                {
                    // Update the UI here
                    if (tmp != _progressBar)
                    {
                        _progressBar = tmp;
                        if (_step == 1)
                        {
                            ApacheProgress.Value = _progressBar;
                        }
                        else if (_step == 2)
                        {
                            MysqlProgress.Value = _progressBar;
                        }
                        else if(_step == 3)
                        {
                            PhpProgress.Value = _progressBar;
                        }
                        else { ProjectProgress.Value = _progressBar; }
                    }
                    if(tmpCopy != _totalProgress)
                    {
                        _totalProgress = tmpCopy;
                        TotalCopyProgress.Value = _totalProgress;                        
                    }

                });
            }

            foreach (string directory in System.IO.Directory.GetDirectories(sourceDir))
            {
                string targetDirectory = System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(directory));
                CopyDirectory(directory, targetDirectory);
            }
        }

        void killProcessByPortnum(int portnumber)
        {
             // Replace with the actual port number
            
        }

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            _mainwindow.SetRunnigService();
        }
    }
}
