using System;
using System.Collections.ObjectModel;

namespace Buchdatenbank
{
    internal class HtmlParser
    {
        private static readonly string _booklookerMain = "https://www.booklooker.de/B%C3%BCcher/Angebote/isbn=";
        private static readonly string _zvabMain = "https://www.zvab.com/servlet/SearchResults?ch_sort=t&cm_sp=sort-_-SRP-_-Results&kn=";

        internal static string HtmlParserMain(string? isbn, Books bookParserMain)
        {
            string path;
            SqliteQueryGet sqliteQueryExists = new();

            if (sqliteQueryExists.QuerySQLiteOnly(isbn!, "isbnNumbers", "book_isbn13") == true)
            {
                // Daten werden falls vorhanden aus der Datenbank geholt.
                // Ansonsten über das Internet.
                sqliteQueryExists.GetValuesFromISBN(isbn!, bookParserMain);
                return "DB";
            }
            else
            {
                // Booklooker.de
                path = _booklookerMain + isbn + "?sortOrder=preis_euro&recPerPage=50";
                BookLookerParser.HtmlBooklookerParserMain(GetHtmlWebsite(path), bookParserMain);

                // ZVAB.de
                path = _zvabMain + isbn + "&sortby=2";
                ZVABParser.HtmlZVABParserMain(GetHtmlWebsite(path), bookParserMain);

                return "";
            }
        }
        internal static string GetHtmlWebsite(string pathToWebsite)
        {
            // Holen des Html-Codes für die anschliessende Untersuchung und Filterung.
            string htmlCodeWebsite = HttpClientHtml.GetHtmlCode(pathToWebsite).Result;
            return htmlCodeWebsite;
        }
    }
}
