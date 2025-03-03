using System.Windows.Controls;

namespace Buchdatenbank.UserControls
{
    public partial class Summary : UserControl
    {
        public NotifyMethodChanged notifyChangedInfo = NotifyMethodChanged.Instance;

        public Summary() 
        {
            InitializeComponent();
            DataContext = this;
            summary_buchmaxe.DataContext = notifyChangedInfo;
            boxnumbers.DataContext = notifyChangedInfo;
            databaseBm.DataContext = notifyChangedInfo.ListQueryAllBm;
            databaseBox.DataContext = notifyChangedInfo.BookCounterAll;
        }
    }
}