using Buchdatenbank.Methods;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Buchdatenbank.UserControls
{
    /// <summary>
    /// Interaktionslogik für BuchDetails.xaml
    /// </summary>
    public partial class BuchDetails : UserControl
    {
        #region Allgemeine Variablen die, die Klasse braucht

        private static readonly ObservableCollection<Books> books = new();
        public ObservableCollection<Books> list = books;

        public Books book = new();
        public BooksCounter bookCounter = new();

        private readonly BitmapImage imgCheck = new(new Uri("pack://application:,,,/Images/check.png"));
        private readonly BitmapImage imgUnCheck = new(new Uri("pack://application:,,,/Images/uncheck.png"));

        internal static bool blNoOffer;
        internal static bool zvabNoOffer;

        // Da hier zwei Usercontrol verwendet werden wird hier die Klasse als
        // Singleton initialisiert. Weil sonst zwei verschiedene Objekte vorhanden
        // sind, und für die Anzeige ist es wichtig das es nicht zwei unterschiedliche
        // Ansichten werden.
        public NotifyMethodChanged notifyChangedInfo = NotifyMethodChanged.Instance;

        // Die erforderlichen Variablen für das verbinden und holen bzw. setzen der Daten
        // in die Datenbank.
        internal Sqlite sqlite = new();
        internal SqliteQueryGet getQuery = new();
        internal SqliteQuerySet setQuery = new();

        #endregion

        #region Listen für die Comboboxen (Rubrik, Zustand und Einband)
        private readonly ObservableCollection<Category> _categoryList = new();
        public ObservableCollection<Category> CategoryList
        {
            get
            {
                return _categoryList;
            }
        }

        private readonly ObservableCollection<Cover> _coverList = new();
        public ObservableCollection<Cover> CoverList
        {
            get
            {
                return _coverList;
            }
        }

        private readonly ObservableCollection<Status> _statusList = new();
        public ObservableCollection<Status> StatusList
        {
            get
            {
                return _statusList;
            }
        }
        #endregion

        public BuchDetails()
        {
            // Hier werden die Comboboxen automatisch vor der Initialisierung 
            // befüllt. Die Daten kommen direkt aus einer Datenbank und sind daher
            // mit der DB verbunden, jeder neue Eintrag in der Datenbank wird 
            // direkt aktualisiert (beim erneuten Programmstart).
            getQuery.GetCategory(CategoryList);
            getQuery.GetCover(CoverList);
            getQuery.GetStatus(StatusList);

            InitializeComponent();

            debugging.Visibility = Visibility.Visible;

            ToolTipService.SetIsEnabled(isbnNumber, true);
            ToolTipService.SetShowOnDisabled(cbCategory, true);
            ToolTipService.SetIsEnabled(cbBuchmaxe, true);

            DataContext = this;
            info.DataContext = notifyChangedInfo;
            summary.DataContext = notifyChangedInfo.Summary;
            database.DataContext = notifyChangedInfo.ListQueryAll;

            BtnNewBook.IsEnabled = false;
            cbSure.IsChecked = (bool?)false;

            noOffer.Visibility = Visibility.Hidden;
            noOffer.Text = "";
        }

        // Die Methoden sind für die Eingabe und das auswählen der
        // Comboboxen verantwortlich. Hier werden die Daten in das 
        // Objekt (Books) übertragen.
        #region Methoden für die Erforderlichen Daten
        private void IsbnNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            noOffer.Visibility = Visibility.Hidden;

            if (isbnNumber.Text.Length > 0)
            {
                BtnClearIsbn.IsEnabled = true;
            }

            if (isbnNumber.Text.Length >= 10)
            {
                isbnIcon.Visibility = Visibility.Visible;
                isbnIcon.Source = imgCheck;
                BtnGetData.IsEnabled = true;
            }
            else
            {
                isbnIcon.Visibility = Visibility.Hidden;
            }
        }
        private void IsbnNumber_OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && isbnNumber.IsFocused)
            {
                BtnGetData_Click(sender, e);
            }
        }
        private void Boxnumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (boxnumber.Text.Length > 0)
            {
                boxIcon.Source = imgCheck;
                book.Boxnumber = boxnumber.Text;
                cbCategory.IsEnabled = true;
            }
            else
            {
                boxIcon.Source = imgUnCheck;
            }
            cbCategory.SelectedIndex = 0;
        }
        #endregion

        #region Methoden für die Buchinformationen
        private void BookAuthor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bookAuthor.Text.Length > 0)
            {
                authorIcon.Visibility = Visibility.Visible;
                authorIcon.Source = imgCheck;
                book.Author = bookAuthor.Text;
            }
            else
            {
                authorIcon.Visibility = Visibility.Hidden;
            }
        }
        private void BookTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bookTitle.Text.Length > 0)
            {
                titleIcon.Source = imgCheck;
                book.Title = bookTitle.Text;
                noOffer.Visibility = Visibility.Hidden;
                noOffer.Text = "";
            }
            else
            {
                titleIcon.Source = imgUnCheck;
            }
        }
        private void BookPublisher_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bookPublisher.Text.Length > 0)
            {
                publisherIcon.Visibility = Visibility.Visible;
                publisherIcon.Source = imgCheck;
                book.Publisher = bookPublisher.Text;
            }
            else
            {
                publisherIcon.Visibility = Visibility.Hidden;
            }
        }
        private void BookPublished_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bookPublished.Text.Length == 4)
            {
                publishedIcon.Visibility = Visibility.Visible;
                publishedIcon.Source = imgCheck;
                book.Published = bookPublished.Text;
            }
            else
            {
                publishedIcon.Visibility = Visibility.Hidden;
            }
        }

        private void BookPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bookPrice.Text.Length > 1)
            {
                priceIcon.Source = imgCheck;
                noOffer.Visibility = Visibility.Hidden;
                noOffer.Text = "";
            }
            else
            {
                priceIcon.Source = imgUnCheck;
            }
        }

        private void CbCover_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCover.SelectedIndex > 0)
            {
                coverIcon.Source = imgCheck;
                book.Cover = cbCover.SelectedIndex + 1;
                book.CoverName = cbCover.SelectedValue.ToString();
                noOffer.Visibility = Visibility.Hidden;
                noOffer.Text = "";
            }
            else
            {
                coverIcon.Source = imgUnCheck;
            }
        }

        private void CbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbStatus.SelectedIndex > 0)
            {
                statusIcon.Source = imgCheck;
                book.Status = cbStatus.SelectedIndex + 1;
                book.StatusName = cbStatus.SelectedValue.ToString();
                noOffer.Visibility = Visibility.Hidden;
                noOffer.Text = "";
            }
            else
            {
                statusIcon.Source = imgUnCheck;
            }
        }
        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            if (cbCategory.SelectedIndex > 0)
            {
                categoryIcon.Source = imgCheck;
                book.Category = cbCategory.SelectedIndex + 1;
                book.CategoryName = cbCategory.SelectedValue.ToString();
                cbCategory.IsEnabled = false;
                noOffer.Visibility = Visibility.Hidden;
                noOffer.Text = "";
            }
            else
            {
                categoryIcon.Source = imgUnCheck;
            }
        }
        #endregion

        // Die Click-Methoden holen oder setzen die Daten in die Datenbank.
        #region Click-Ereignisse
        private void BtnClearIsbn_Click(object sender, RoutedEventArgs e)
        {
            isbnNumber.Clear();
            isbnIcon.Visibility = Visibility.Hidden;
            BtnClearIsbn.IsEnabled = false;
        }
       
        private void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            isbnNumber.Text = isbnNumber.Text.Trim();

            if (isbnNumber.Text.Contains('-'))
            {
                isbnNumber.Text = MyRegex().Replace(isbnNumber.Text, "");
            }
            if (CheckIsbn.CheckForISBN(isbnNumber.Text))
            {
                if (isbnNumber.Text.Length == 10)
                {
                    book.Isbn13 = ISBN.Convert10to13(isbnNumber.Text);
                    book.Isbn10 = isbnNumber.Text.Remove(10);
                }
                else if (isbnNumber.Text.Length == 13)
                {
                    book.Isbn10 = ISBN.Convert13to10(isbnNumber.Text);
                    book.Isbn13 = isbnNumber.Text;
                }

                string? resultQuery = HtmlParser.HtmlParserMain(isbnNumber.Text, book);

                if (blNoOffer == false && zvabNoOffer == false && resultQuery == "")
                {
                    isbnIcon.Visibility = Visibility.Visible;
                    noOffer.Text = "Keine Angebote gefunden";
                    noOffer.Foreground = Brushes.Red;
                    noOffer.Visibility = Visibility.Visible;
                }
                else
                {
                    
                    if (resultQuery == "DB")
                    {
                        
                        bookAuthor.Text = book.Author;
                        bookPublisher.Text = book.Publisher;
                        bookTitle.Text = book.Title;
                        bookPublished.Text = book.Published;
                        cbCover.SelectedIndex = book.Cover - 1;
                        bookPrice.Text = book.Price;
                        notifyChangedInfo.DebugInfo = "Daten in der Datenbank gefunden";
                    }
                    else
                    {
                        bookAuthor.Text = book.Author;
                        bookPublisher.Text = book.Publisher;
                        bookTitle.Text = book.Title;
                        bookPublished.Text = book.Published;
                        cbCover.SelectedIndex = book.Cover;
                        bookPrice.Text = book.Price;

                        bookAuthor.Text = book.Author;
                        bookPublisher.Text = book.Publisher;
                        bookTitle.Text = book.Title;
                        bookPublished.Text = book.Published;
                        cbCover.SelectedIndex = book.Cover + 1;
                        bookPrice.Text = book.Price;
                        notifyChangedInfo.DebugInfo = "Daten im Internet gefunden";
                    }
                }

                book.Category = cbCategory.SelectedIndex + 1;
                book.Boxnumber = boxnumber.Text;
                book.Price = bookPrice.Text;
            }
            else
            {
                isbnIcon.Source = imgUnCheck;
                isbnIcon.Visibility = Visibility.Visible;
                noOffer.Text = "ungültige Isbn";
                noOffer.Foreground = Brushes.Red;
                noOffer.Visibility = Visibility.Visible;
            }
        }
        private void CbSure_Click(object sender, RoutedEventArgs e)
        {
            if (cbSure.IsChecked!.Value)
            {
                BtnNewBook.IsEnabled = true;
            }
            else BtnNewBook.IsEnabled = false;
        }
         private void CbBuchmaxe_Click(object sender, RoutedEventArgs e)
        {
            if (cbBuchmaxe.IsChecked!.Value)
            {
                bookPrice.Clear();
                book.Buchmaxe_source = true;
                book.Another_source = false;
                
            }
            else
            {
                book.Another_source = true;
                book.Buchmaxe_source = false;
            }
        }
        private void BtnNewBook_Click(object sender, RoutedEventArgs e)
        {
            // Hier werden bei allen Eingaben die jeweils am Anfang und am Ende Leerzeichen
            // enthalten, die Leerzeichen entfernt. Passiert beim sog. Copy-and-Paste
            // öfters :-)

            bookTitle.Text = bookTitle.Text.Trim();
            bookAuthor.Text = bookAuthor.Text.Trim();
            bookPublisher.Text = bookPublisher.Text.Trim();
            bookPublished.Text = bookPublished.Text.Trim();
            bookPrice.Text = bookPrice.Text.Trim();

            try
            {
                if (bookTitle.Text == string.Empty)
                {
                    ErrorInput("Titel wurde nicht eingegeben!\n");
                    cbSure.IsChecked = false;
                }

                if (cbCategory.SelectedIndex == 0)
                {
                    ErrorInput("Rubrik wurde nicht ausgewählt!\n");
                    cbSure.IsChecked = false;
                }
                if (cbCover.SelectedIndex == 0)
                {
                    ErrorInput("Einband wurde nicht ausgewählt!\n");
                    cbSure.IsChecked = false;
                }
                if (cbStatus.SelectedIndex == 0)
                {
                    ErrorInput("Zustand wurde nicht ausgewählt!\n");
                    cbSure.IsChecked = false;
                }
                if (bookPrice.Text == string.Empty)
                {
                    ErrorInput("Preis wurde nicht eingegeben!\n");
                    cbSure.IsChecked = false;
                }
                if (cbSure.IsChecked == false)
                {
                    ErrorInput("Bitte sicher ankreuzen!\n");
                }

                // Wenn alles in Ordnung ist werden die Daten in das Objekt Books
                // eingefügt und anschließend in Listen gepackt damit sie angezeigt
                // werden und danach sauber in die DB eingefügt werden können.
                else if (cbSure.IsChecked == true)
                {
                    book.Boxnumber = boxnumber.Text;
                    book.Price = bookPrice.Text;
                    book.Category = cbCategory.SelectedIndex + 1;
                    book.CategoryName = cbCategory.SelectedValue.ToString();

                    if (book.Isbn10 != string.Empty && book.Isbn13 != string.Empty)
                    {
                        if (book.Isbn10 == string.Empty) book.Isbn13 = ISBN.Convert10to13(isbnNumber.Text);
                        if (book.Isbn13 == string.Empty) book.Isbn10 = ISBN.Convert13to10(isbnNumber.Text);
                    }

                    // Hier werden die gefundenen Bücher aufgelistet einmal für Buchmaxe im zweiten Tab,
                    // und für die anderen im ersten Tab.
                    if (book.Buchmaxe_source)
                    {
                        notifyChangedInfo.ListQueryAllBm.Add(book);
                    }
                    else
                    {
                        notifyChangedInfo.ListQueryAll.Add(book);
                    }

                    // Hier werden die Bücher von Buchmaxe und den anderen gezählt und
                    // für die Liste bereitgestellt und angezeigt. Die Liste ist im zweiten
                    // Tab des Programms.
                    #region Zähler für die Bücher
                    var listItem = notifyChangedInfo.BookCounterAll.Where(x => x.BoxNumberAll == boxnumber.Text);

                    if (listItem.Any(i => i.BoxNumberAll == boxnumber.Text))
                    {
                        foreach (var item in notifyChangedInfo.BookCounterAll.ToList())
                        {
                            if (item.BoxNumberAll == boxnumber.Text && book.Another_source)
                            {
                                item.BookCountAll += 1;
                            }
                            else if (book.Buchmaxe_source) {
                                item.CountBooksBm += 1;
                            }
                        }
                    }
                    else
                    {
                        bookCounter = new BooksCounter
                        {
                            BoxNumberAll = boxnumber.Text,
                            BoxCategory = cbCategory.Text
                        };

                        if (book.Buchmaxe_source) bookCounter.CountBooksBm++;
                        else bookCounter.BookCountAll++;

                        notifyChangedInfo.BookCounterAll.Add(bookCounter);
                    }
                    #endregion

                    list.Add(book);

                    // Wenn die Daten nicht eingefügt werden
                    // können wird auch nichts zurückgesetzt.
                    if (!BtnDataBase())
                    {
                        MessageBox.Show("Buch konnte nicht in die Datenbank eingetragen werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        
                        // Da weitere Daten eingegeben werden, wird hier alles zurückgesetzt und
                        // ein neues Books-Objekt erstellt.
                        book = new Books();
                        cbSure.IsChecked = false;
                        isbnNumber.Focus();

                        isbnNumber.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Buch konnte nicht in die Datenbank eingetragen werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception newBtn)
            {
                MessageBox.Show(newBtn.Message);
            }
            
        }

        // Methode für das eintragen in die Datenbank.
        private bool BtnDataBase()
        {
            try
            {
                // Bevor die Daten in die Datenbank geschrieben werden, müssen noch einige
                // leere Felder befüllt werden, damit es beim einfügen oder abfragen keine
                // bösen Überraschungen gibt.
                if (isbnNumber.Text == "")
                {
                    book.Isbn13 = "unbekannt";
                    book.Isbn10 = "unbekannt";
                }

                if (bookAuthor.Text == "") book.Author = "unbekannt";
                if (bookPublisher.Text == "") book.Publisher = "unbekannt";

                book.CategoryName = cbCategory.SelectedValue.ToString();
                book.CoverName = cbCover.SelectedValue.ToString();

                if (setQuery.InsertToSqlite(list) == false)
                {
                    notifyChangedInfo.DebugInfo += "\nDaten konnten nicht hinzugefügt werden!\n";
                    return false;
                }
                else
                {
                    list.Clear();

                    notifyChangedInfo.DebugInfo += "\nDaten erfolgreich eingefügt";

                    SetAllTextToEmpty();
                }
            }
            catch
            {
                notifyChangedInfo.DebugInfo = "Daten konnten nicht hinzugefügt werden!\n";
            }
            return true;
        }
       
        #endregion

        #region Alle Inhalte löschen
        private void SetAllTextToEmpty()
        {
            BtnGetData.IsEnabled = false;
            BtnNewBook.IsEnabled = false;

            bookAuthor.Text = string.Empty;
            bookPublisher.Text = string.Empty;
            bookTitle.Text = string.Empty;
            bookPublished.Text = string.Empty;
            bookPrice.Text = "1,00";
            cbCover.SelectedIndex = 0;
            cbStatus.SelectedIndex = 0;
            cbSure.IsChecked = false;
            cbBuchmaxe.IsChecked = false;

            isbnIcon.Visibility = Visibility.Hidden;
            authorIcon.Visibility = Visibility.Hidden;
            publisherIcon.Visibility = Visibility.Hidden;
            publishedIcon.Visibility = Visibility.Hidden;

        }
        #endregion

        // Methode zum entfernen der Bindestriche in der ISBN, wenn welche
        // vorhanden sind.
        [GeneratedRegex("^*[-]*")]
        private static partial Regex MyRegex();

        private void ErrorInput (string message)
        {
                noOffer.Text += message;
                noOffer.Foreground = Brushes.Red;
                noOffer.Visibility = Visibility.Visible;
        }
    }

}
