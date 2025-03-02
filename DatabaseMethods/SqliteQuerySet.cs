using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;

namespace Buchdatenbank
{
    internal class SqliteQuerySet
    {

        private readonly string DataSourceReadWrite = @"Data Source=.\DatabaseSqlite\buchdatenbank.db;Mode=ReadWrite;";

        internal SqliteConnection? connection;
        internal SqliteCommand? command;
        internal SqliteErrorHandle error = new();
        
        private static readonly Sqlite sqlite = new();
        readonly Sqlite sqliteQuerySet = sqlite;
        private static readonly SqliteQueryGet sqliteQueryGet = new();
        readonly SqliteQueryGet getSqlQuery = sqliteQueryGet;

        public NotifyMethodChanged sqlSetQueryInfo = NotifyMethodChanged.Instance;


        #region Insert to Database (SQLite)
        internal bool InsertToSqlite(ObservableCollection<Books> insertBookIsbn)
        {
            try
            {
                
                foreach (Books book in insertBookIsbn)
                {
                    string bookTitle = book.Title.Replace("'", "''");
                    string bookPublisher = book.Publisher.Replace("'", "''");
                    string bookAuthor = book.Author.Replace("'", "''");

                    //sqlSetQueryInfo.DebugInfo = "Verbindung hergestellt...\n\n";
                    connection = sqliteQuerySet.ConnectToSqlite(DataSourceReadWrite);

                    // Wenn Boxnummer nicht vorhanden!
                    if (!getSqlQuery.QuerySQLite(book.Boxnumber, "boxnumber", "box_number", connection!))
                    {
                        // Boxnummer wird eingetragen
                        //sqlSetQueryInfo.DebugInfo += "Boxnummer wird eingetragen...\n";
                        InsertNoExistsSQLite(book.Boxnumber, "boxnumber", "box_number", connection!);
                    }
                    // Wenn Isbn nicht vorhanden!
                    if (!getSqlQuery.QuerySQLite(book.Isbn13, "isbnNumbers", "book_isbn13", connection!) && !getSqlQuery.QuerySQLite(bookTitle, "books", "book_title", connection!))
                    {
                        // Isbn wird eingetragen
                        //sqlSetQueryInfo.DebugInfo += "ISBN wird eingetragen...\n";
                        InsertExtraIsbn(book.Isbn10, book.Isbn13, connection!);
                    }


                    // Wenn Verlag nicht vorhanden
                    if (!getSqlQuery.QuerySQLite(bookPublisher, "publisher", "publishername", connection!))
                    {
                        // Verlag wird eingetragen
                        //sqlSetQueryInfo.DebugInfo += "Verlag wird eingetragen...\n";
                        InsertNoExistsSQLite(bookPublisher, "publisher", "publishername", connection!);
                    }
                    // Wenn Autor/in nicht vorhanden
                    if (!getSqlQuery.QuerySQLite(bookAuthor, "author", "authorname", connection!))
                    {
                        // Autor/in wird eingetragen
                        //sqlSetQueryInfo.DebugInfo += "Autor/in wird eingetragen...\n";
                        InsertNoExistsSQLite(bookAuthor, "author", "authorname", connection!);
                    }
                    if (!getSqlQuery.QuerySQLite(book.User, "users", "username", connection!))
                    {
                        // User/in wird eingetragen
                        //sqlSetQueryInfo.DebugInfo += "User/in wird eingetragen...\n";
                        InsertNoExistsSQLite(book.User, "users", "username", connection!);
                    }

                    // Für den Fall dass es Bücher gibt die keine Isbn haben, werden komplett
                    // eingetragen, dadurch kann es zu mehreren gleichen Bücher kommen.
                    if (getSqlQuery.QuerySQLite(bookTitle, "books", "book_title", connection!) == false)
                    {
                        command = connection!.CreateCommand();
                        command.CommandText =
                            $"INSERT INTO books (" +
                            $"book_title, published_at, price, listed_at, " +
                            $"update_listed_at, source_bm, source_another, " +
                            $"fk_isbn_id, fk_box_id, fk_publisher_id, fk_category_id, fk_cover_id, fk_status_id" +
                            $") VALUES (" +
                            $"'{bookTitle}', '{book.Published}', '{book.Price}', '{DateTime.Now}', '{DateTime.Now}', '{book.Buchmaxe_source}', " +
                            $"'{book.Another_source}', " +
                            $"(SELECT isbn_id FROM isbnNumbers WHERE book_isbn10 = '{book.Isbn10}')," +
                            $"(SELECT box_id FROM boxnumber WHERE box_number = '{book.Boxnumber}')," +
                            $"(SELECT publisher_id FROM publisher WHERE publishername = '{bookPublisher}')," +
                            $"(SELECT category_id FROM category WHERE category_id = '{book.Category}')," +
                            $"(SELECT cover_id FROM cover WHERE cover_id = '{book.Cover}')," +
                            $"(SELECT status_id FROM status WHERE status_id = '{book.Status}')" +
                            $");";
                        try
                        {
                            command.ExecuteNonQueryAsync();
                            sqlSetQueryInfo.DebugInfo = "";
                            if (!getSqlQuery.QueryRelations_Author_Books(bookAuthor, bookTitle, connection!))
                            {
                                InsertAuthorBooksRelation(bookAuthor, bookTitle, connection!);
                            }
                            if (!getSqlQuery.QueryRelations_Box_Books(book.Boxnumber, bookTitle, connection!))
                            {
                                InsertBoxBooksRelation(book.Boxnumber, bookTitle, connection!);
                            }
                            if (!getSqlQuery.QueryRelations_User_Books(book.User, bookTitle, connection!))
                            {
                                InsertUserBooksRelation(book.User, bookTitle, connection!);
                            }
                            sqlSetQueryInfo.DebugInfo += "Buch wurde eingetragen!\n";
                            
                        }
                        catch (Exception exInsert)
                        {
                            error.ErrorOutput(exInsert.Message);
                            return false;
                        }
                    }
                    
                    else
                    {
                        // Wenn Bücher vorhanden sind werden sie aktualisiert.
                        UpdateToSqlite(bookTitle, book.Price!, connection!);
                        
                        if (!getSqlQuery.QueryRelations_User_Books(book.User, bookTitle, connection!))
                        {
                            //sqlSetQueryInfo.DebugInfo += "Relation Buch zu User/in wird eingetragen...\n";
                            InsertUserBooksRelation(book.User, bookTitle, connection!);
                        }
                        if (!getSqlQuery.QueryRelations_Box_Books(book.Boxnumber, bookTitle, connection!))
                        {
                            //sqlSetQueryInfo.DebugInfo += "Relation Buch zur Boxnummer wird eingetragen...\n";
                            InsertBoxBooksRelation(book.Boxnumber, bookTitle, connection!);
                        }
                    }
                    //sqlSetQueryInfo.DebugInfo += "\nBuch wurde eingetragen!\n";
                    connection!.Close();
                }
                //sqlSetQueryInfo.DebugInfo += "\nVerbindung zu Datenbank geschlossen!\n";
                return true;

            }
            catch (Exception exInsert)
            {
                error.ErrorOutput(exInsert.Message);
                return false;
            }
           
        }
        #endregion

        #region Insert Methods SQLite

        internal void InsertExtraIsbn(string isbn10, string isbn13, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"INSERT INTO isbnNumbers (book_isbn10, book_isbn13) VALUES ('{isbn10}', '{isbn13}');";
            try
            {
                command.ExecuteNonQueryAsync();
            }
            catch (Exception exBoxnumber)
            {
                error.ErrorOutput(exBoxnumber.Message);
            }
        }

        internal void InsertAuthorBooksRelation(string valueAuthor, string valueBook, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"INSERT INTO author_books (author_id, book_id) VALUES (" +
            $"(SELECT author_id FROM author WHERE authorname = '{valueAuthor}')," +
            $"(SELECT book_id FROM books WHERE book_title = '{valueBook}'));";

            try
            {
                command.ExecuteNonQueryAsync();
            }
            catch (Exception exInsertRelation)
            {
                error.ErrorOutput(exInsertRelation.Message);
            }
        }

        internal void InsertBoxBooksRelation(string valueID1, string valueID2, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"INSERT INTO books_boxnumber (box_id, book_id) VALUES (" +
            $"(SELECT box_id FROM boxnumber WHERE box_number = '{valueID1}')," +
            $"(SELECT book_id FROM books WHERE book_title = '{valueID2}'));";

            try
            {
                command.ExecuteNonQueryAsync();
            }
            catch (Exception exInsertRelation)
            {

                error.ErrorOutput(exInsertRelation.Message);
            }
        }

        internal void InsertUserBooksRelation(string valueID1, string valueID2, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"INSERT INTO user_book (user_id, book_id) VALUES (" +
            $"(SELECT user_id FROM users WHERE username = '{valueID1}')," +
            $"(SELECT book_id FROM books WHERE book_title = '{valueID2}'));";

            try
            {
                command.ExecuteNonQueryAsync();
            }
            catch (Exception exInsertRelation)
            {
                error.ErrorOutput(exInsertRelation.Message);
            }
        }

        internal void InsertNoExistsSQLite(string value, string table, string column, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"INSERT INTO {table} ({column}) VALUES ('{value}');";
            try
            {
                command.ExecuteNonQueryAsync();
            }
            catch (Exception exBoxnumber)
            {
                error.ErrorOutput(exBoxnumber.Message);
            }
        }
        #endregion

        #region Update Database
        internal void UpdateToSqlite(string title, string price, SqliteConnection conn)
        {
            try
            {
                connection = conn;

                command = connection!.CreateCommand();
                command.CommandText = $"" +
                            $"UPDATE books SET " +
                            $"update_listed_at = '{DateTime.Now}'," +
                            $"count_book = count_book + 1, price = '{price}'" +
                            $"WHERE book_title = '{title}'";
                command.ExecuteNonQueryAsync();
            }
            catch (Exception exUpdate)
            {
                error.ErrorOutput(exUpdate.Message);
            }

            sqlSetQueryInfo.DebugInfo = "Datenbankeintrag aktualisiert!\n";
        }
        #endregion
    }
}