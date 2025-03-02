using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Buchdatenbank
{
    public class BooksCounter : INotifyPropertyChanged
    {
        
        public BooksCounter() { }

        private string? _boxCategory { get; set; }
        public string? BoxCategory
        {
            get { return _boxCategory; }
            set
            {
                _boxCategory = value;
            }
        }

        private string? _boxNumberAll { get; set; }
        public string? BoxNumberAll
        {
            get { return _boxNumberAll; }
            set
            {
                _boxNumberAll = value;
            }
        }

        private int _bookCountAll { get; set; }
        public int BookCountAll
        {
            get { return _bookCountAll; }
            set
            {
                _bookCountAll = value;
                OnPropertyChanged();
            }
        }

        private int _countBooksBm { get; set; }
        public int CountBooksBm
        {
            get { return _countBooksBm; }
            set
            {
                _countBooksBm = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
