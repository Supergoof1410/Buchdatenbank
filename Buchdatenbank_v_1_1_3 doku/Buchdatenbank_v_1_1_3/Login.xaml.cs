using System.Windows;
using System.Windows.Controls;

namespace Buchdatenbank
{
    /// <summary>
    /// Interaktionslogik für Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        MainWindow main = new();
        Sqlite liteSql = new();
        public Login()
        {
            //Sqlite.ConnectToSqlite();
            InitializeComponent();
            //Sqlite.ConnectToSqlite();
        }
/* 
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            main.Close();
            this.Close();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (liteSql.checkUser(username.Text, passwd.Password))
            {
                //MessageBox.Show(passwd.Password);
                main.Show();
                this.Close();
            } else {
                MessageBox.Show("Für einen Zugang den Entwickler fragen!", "Du kommst hier net rein!", MessageBoxButton.OK, MessageBoxImage.Stop);
                username.Clear();
                passwd.Clear();
            }
        }
        private void username_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (username.Text.Length > 0)
            {
                BtnLogin.IsEnabled = true;
            }
            else BtnLogin.IsEnabled = false;
        } */
    }
}
