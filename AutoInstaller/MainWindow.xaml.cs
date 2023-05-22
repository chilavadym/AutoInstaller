using AutoInstaller.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AutoInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private string _targetPath = @"C:\MyProject";
        private string _sourcePath = @"MyProject";
        private CopyingControl _copyingControl;
        private RunnigControl _runnigControl;
        private RestoreSqlControl _restoreSqlControl;

        public MainWindow()
        {
            try
            {
                string vcfilePath = "MyProject/VC_redist.x64.exe";
                var absolutePath = System.IO.Path.GetFullPath(vcfilePath);

                Process process1 = new Process();
                process1.StartInfo.FileName = absolutePath;
                process1.Start();
                process1.WaitForExit();
            }
            catch { }

            InitializeComponent();
            _copyingControl = new CopyingControl(this, _sourcePath, _targetPath);
            _runnigControl = new RunnigControl(this, _targetPath);
            _restoreSqlControl = new RestoreSqlControl(this);
            UserControlContainer.Content = _copyingControl;

        }

        public void SetRunnigService()
        {
            _runnigControl.refrshControl();
            UserControlContainer.Content = _runnigControl;

        }

        public void SetRestoreSqlService()
        {
            UserControlContainer.Content = _restoreSqlControl;
        }
    }
}
