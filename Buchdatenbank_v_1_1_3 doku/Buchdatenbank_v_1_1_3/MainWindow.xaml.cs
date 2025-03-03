using Buchdatenbank.Methods.Writer;
using System;
using System.ComponentModel;
using System.Windows;

namespace Buchdatenbank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NotifyMethodChanged online = NotifyMethodChanged.Instance;
        internal LogWriter writeToLog = new();

        public MainWindow()
        {
            writeToLog.WriteLog($"[{DateTime.Now}] - [User: {Environment.UserName}] - Login");
            
            InitializeComponent();
            
            DataContext = this;
            State.DataContext = online;
            online.Online = "User: " + Environment.UserName;
        }
        
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            BackupWriter.BackupDBFile();
            writeToLog.WriteLog($"[{DateTime.Now}] - [User: {Environment.UserName}] - Logout");
        }
    }
}
