using Buchdatenbank.Methods;
using Buchdatenbank.Methods.Writer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Buchdatenbank;

public class NotifyMethodChanged : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private static volatile NotifyMethodChanged? _instance;
    internal LogWriter writeToLogSql = new LogWriter();

    public static NotifyMethodChanged Instance
    {
        get
        {
            // DoubleLock
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new NotifyMethodChanged();
                    }
                }
            }
            return _instance;
        }
    }

    // Hilfsfeld für eine sichere Threadsynchronisierung
    private static object _lock = new();

    private NotifyMethodChanged() { }

    private int _countBooks = 0;
    
    private string _online = "Nicht verbunden";

    private string _debugInfo = "";

    public string Online
    {
        get { return _online; }
        set
        {
            _online = value;
            OnPropertyChanged();
        }
    }
    public int CountBooks
    {
        get { return _countBooks; }
        set
        {

            _countBooks = value;
            OnPropertyChanged();
        }
    }

    

    public string DebugInfo
    {
        get { return _debugInfo; }
        set
        {
            _debugInfo = value;
            OnPropertyChanged();
        }
    }

    private int _countBooksBm = 0;

    public int CountBooksBm
    {
        get { return _countBooksBm; }
        set
        {
            _countBooksBm = value;
            OnPropertyChanged();
        }
    }

    private readonly string _summary = "Zusammenfassung " + "(" + DateTime.Now.ToString("dd.MM.yy") + ")";


    public string Summary
    {
        get { return _summary; }
    }

    private readonly string _summary_bm = "Buchmaxe " + "(" + DateTime.Now.ToString("dd.MM.yy") + ")";


    public string Summary_Bm
    {
        get { return _summary_bm; }
    }

    private readonly string _summary_box = "Anzahl Bücher pro Box " + "(" + DateTime.Now.ToString("dd.MM.yy") + ")";


    public string Summary_Box
    {
        get { return _summary_box; }
    }

    private string? _box_number;

    public string Box_Number
    {
        get { return _box_number!; }
        set
        {
            _box_number = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Books> _listQueryAll = new();

    public ObservableCollection<Books> ListQueryAll
    {
        get
        {
            return _listQueryAll;
        }
        set
        {
            _listQueryAll = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Books> _listQueryAllBm = new();

    public ObservableCollection<Books> ListQueryAllBm
    {
        get
        {
            return _listQueryAllBm;
        }
        set
        {
            _listQueryAllBm = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<BooksCounter> _bookCounterAll = new();

    public ObservableCollection<BooksCounter> BookCounterAll
    {
        get
        {
            return _bookCounterAll;
        }
        set
        {
            new BooksCounter();
            _bookCounterAll = value;
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
