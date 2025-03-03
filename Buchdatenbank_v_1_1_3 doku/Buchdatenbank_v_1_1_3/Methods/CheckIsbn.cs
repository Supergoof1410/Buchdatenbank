

namespace Buchdatenbank
{
    internal static class CheckIsbn
    {
        // Letzte Änderungen: 28.06.2023
        private static int checksum;
        private static int check_number;
        internal static string? isbn;
        //static long counterGueltigeBooks;

        // Prüfen ob die ISBN gültig ist und über eine gültige Prüfnummer verfügt!
        // Referenz: https://de.wikipedia.org/wiki/Internationale_Standardbuchnummer#Formeln_zur_Berechnung_der_Pr%C3%BCfziffer
        // Rückgabewert: bool
        #region Check Methode (Main)
        internal static bool CheckForISBN(string? isbnToCheckNumber)
        {
            isbn = isbnToCheckNumber;
            int summe = 0;
            int cacheY = 1;
            check_number = 0;
            checksum = 0;

            // ISBN-13 Prüfung
            #region ISBN-13
            if (isbn?.Length == 13)
            {
                for (int x = 0; x < isbn.Length - 1; x++)
                {
                    if (x % 2 == 0)
                    {
                        summe += (int)char.GetNumericValue(isbn[x]);
                    }
                    else
                    {
                        summe += (int)char.GetNumericValue(isbn[x]) * 3;
                    }
                }

                // Prüfen auf die Prüfnummer
                checksum = (10 - (summe % 10)) % 10;

                // Prüfen auf Gültigkeit der ISBN um mögliche Vertauschungen vorzubeugen!
                check_number = ((summe + checksum) % 10);

                if (check_number == 0 && checksum == (int)char.GetNumericValue(isbn[12]))
                {
                    PassCheckISBN();
                    return true;
                }
                else
                {
                    ErrorCheckISBN(" ungültig\n", isbn);
                }
            }
            #endregion

            // ISBN-10 Prüfung
            #region ISBN-10
            else if (isbn?.Length == 10)
            {
                for (int x = 0; x < isbn.Length - 1; x++)
                {
                    check_number += cacheY * (int)char.GetNumericValue(isbn[x]);
                    cacheY++;
                }

                // Für den Fall das die letzte Ziffer ein X ist. Das X steht für das 
                // römische Zeichen 10.
                if (isbn[9] == 'X' || isbn[9] == 'x')
                {
                    _ = check_number + ((10 * 10) % 11);
                }

                // Prüfen auf die Prüfnummer
                check_number %= 11;

                // Prüfen auf Gültigkeit der ISBN um mögliche Vertauschungen vorzubeugen!
                checksum = (check_number + (int)char.GetNumericValue(isbn[9]) * 10) % 11;

                if ((checksum == 0 && check_number == (int)char.GetNumericValue(isbn[9])) || (checksum == 0 && check_number == 10))
                {
                    PassCheckISBN();
                    return true;
                }
                else
                {
                    ErrorCheckISBN(" ungültig\n", isbn);
                }

            }

            // Für den Fall das die eingegebene ISBN zu kurz oder zu lang ist,
            // oder einfach nur ungültig ist.
            else
            {
                string? error = " Die ISBN ist zu ";

                if (isbn!.Length < 10) { error += "kurz\n"; }
                else if (isbn.Length > 13) { error += "lang\n"; }
                else error = " ungültig\n";

                ErrorCheckISBN(error, isbn);
            }
            #endregion

            return false;
        }
        #endregion

        // Eine Methode zur Anzeige was der Fehler ist, deswegen wird auch die Prüfnummer
        // und Checknummer angezeigt. Die Checksumme muss bei einer gültigen ISBN immer 0
        // sein und die Prüfnummer gleich der letzten Zahl der ISBN auf dem Buch
        #region Pass und Error Methoden
        internal static string ErrorCheckISBN(string error, string isbn)
        {
            string CheckErrorOut = $"Checknummer: {checksum} Prüfziffer: {check_number}\n ISBN: {isbn}";
            return CheckErrorOut;
        }

        internal static string PassCheckISBN()
        {
            string CheckPassOut = "gültig";
            return CheckPassOut;
        }
        #endregion
    }
}