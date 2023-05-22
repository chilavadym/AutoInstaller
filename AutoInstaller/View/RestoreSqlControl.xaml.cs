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
using MySql.Data.MySqlClient;
using System.IO;

namespace AutoInstaller.View
{
    /// <summary>
    /// Interaction logic for RestoreSql.xaml
    /// </summary>
    public partial class RestoreSqlControl : UserControl
    {
        public string _connectionString = "server=localhost;user id=root;password=;database=mydb";
        public string _sqlFilePath = "F:/My_work/mydb.sql";
        private MainWindow _mainWindow;
        public RestoreSqlControl(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private bool Restore_MySqlDatabase()
        {
            try
            {
                

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
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void Btn_Restore_Click(object sender, RoutedEventArgs e)
        {
            if(Restore_MySqlDatabase()){
                MessageBox.Show("Success restore"); 
            }
        }
    }
}
