using Buchdatenbank.Methods.Writer;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;

namespace Buchdatenbank
{
    public class SqliteQueryGet
    {
        private readonly string DataSourceReadOnly = @"Data Source=.\DatabaseSqlite\buchdatenbank.db;Mode=ReadOnly";

        internal SqliteConnection? connection;
        internal SqliteCommand? command;
        private static readonly SqliteErrorHandle sqliteErrorHandle = new();
        internal SqliteErrorHandle error = sqliteErrorHandle;

        readonly Sqlite sqliteQueryGet = new();

        public NotifyMethodChanged sqlGetQueryInfo = NotifyMethodChanged.Instance;
        


        // Diese Methoden dienen nur zum lesen der angeforderten Daten.
        // Da sie nur Daten holen, wird die Datenbank nur zum lesen geöffnet,
        // damit auch andere gleichzeitig darauf (lesend) zugreifen können.

        #region Get Status-Info
        Status statusSql = new();
        internal void GetStatus(ObservableCollection<Status> listStatus)
        {
            connection = sqliteQueryGet.ConnectToSqlite(DataSourceReadOnly);
            try
            {
                command = connection!.CreateCommand();
                command.CommandText = @"SELECT status_id, statusname FROM status;";

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        statusSql.StatusID = reader.GetInt32(0);
                        statusSql.StatusName = reader.GetString(1);
                        listStatus.Add(statusSql);
                        statusSql = new Status();
                    }

                }
                connection.Close();
            }
            catch (Exception exStatus)
            {
                connection!.Close();
                error.ErrorOutput(exStatus.Message);
            }
        }
        #endregion

        #region Get Cover-Info
        Cover coverSql = new();
        internal void GetCover(ObservableCollection<Cover> listCover)
        {
            connection = sqliteQueryGet.ConnectToSqlite(DataSourceReadOnly);
            try
            {
                command = connection!.CreateCommand();
                command.CommandText = @"SELECT cover_id, covername FROM cover;";

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        coverSql.CoverId = reader.GetInt32(0);
                        coverSql.CoverName = reader.GetString(1);
                        listCover.Add(coverSql);
                        coverSql = new Cover();
                    }

                }
                connection.Close();
            }
            catch (Exception exCover)
            {
                connection!.Close();
                error.ErrorOutput(exCover.Message);
            }
        }
        #endregion

        #region Get Category-Info
        Category categorySql = new();
        internal void GetCategory(ObservableCollection<Category> listCategory)
        {
            connection = sqliteQueryGet.ConnectToSqlite(DataSourceReadOnly);
            try
            {
                command = connection!.CreateCommand();
                command.CommandText = @"SELECT category_id, categoryname FROM category;";

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        categorySql.CategoryId = reader.GetInt32(0);
                        categorySql.CategoryName = reader.GetString(1);
                        listCategory.Add(categorySql);
                        categorySql = new Category();
                    }
                }

                connection.Close();
            }
            catch (Exception exCategory)
            {
                connection!.Close();
                error.ErrorOutput(exCategory.Message);
            }
        }
        #endregion

        #region Query-Get-Methods SQLite

        internal bool GetValuesFromISBN(string? isbn13, Books book)
        {
            connection = sqliteQueryGet.ConnectToSqlite(DataSourceReadOnly);

            command = connection!.CreateCommand();
            command.CommandText = $"SELECT book_isbn13 FROM isbnNumbers WHERE book_isbn13 = {isbn13}";
            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    //MessageBox.Show("Hurra bin in der Datenbank drin.");
                    command = connection!.CreateCommand();
                    command.CommandText = $"" +
                        $"SELECT authorname, book_title, publishername, published_at, covername, cover_id, price FROM " +
                        $"books, author_books, author, isbnNumbers, publisher, cover " +
                        $"WHERE books.book_id = author_books.book_id " +
                        $"AND author_books.author_id = author.author_id " +
                        $"AND publisher.publisher_id = books.fk_publisher_id " +
                        $"AND cover.cover_id = books.fk_cover_id " +
                        $"AND isbnNumbers.isbn_id = books.fk_isbn_id " +
                        $"AND isbnNumbers.book_isbn13 = '{isbn13}';";

                    result = command.ExecuteReader();

                    while (result.Read())
                    {
                        book.Author = result.GetString(0);
                        book.Title = result.GetString(1);
                        book.Publisher = result.GetString(2);
                        book.Published = result.GetString(3);
                        book.CoverName = result.GetString(4);
                        book.Cover = result.GetInt16(5);
                        book.Price = result.GetString(6);
                    }
                    OfferOrNotOffer.PassBookToOffer(true);
                    connection!.Close();
                    return true;
                }
                else
                {
                    OfferOrNotOffer.PassBookToOffer(false);
                }
            }
            catch (Exception exGetValues)
            {
                connection!.Close();
                error.ErrorOutput(exGetValues.Message);
            }
            return false;
        }

        internal bool QuerySQLite(string value, string table, string column, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"SELECT {column} FROM {table} WHERE {column} = '{value.Replace("'", "''")}'";

            try
            {
                var result = command.ExecuteReader();
                return result.HasRows;
            }
            catch (Exception exQuery)
            {
                error.ErrorOutput(exQuery.Message);
                //return false;
            }
            return false;
        }
        internal bool QuerySQLiteOnly(string value, string table, string column)
        {
            connection = sqliteQueryGet.ConnectToSqlite(DataSourceReadOnly);

            command = connection!.CreateCommand();
            command.CommandText = $"SELECT {column} FROM {table} WHERE {column} = '{value}'";

            try
            {
                var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    connection.Close();
                    return true;
                }
                return false;
            }
            catch (Exception exQuery)
            {

                error.ErrorOutput(exQuery.Message);
                connection.Close();
                return false;
            }
            
        }

        internal bool QueryRelations_Author_Books(string valueID1, string valueID2, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"SELECT author_id, book_id FROM author_books WHERE author_id = " +
            $"(SELECT author_id FROM author WHERE authorname = '{valueID1}') AND book_id = " +
            $"(SELECT book_id FROM books WHERE book_title = '{valueID2}');";

            try
            {
                var result = command.ExecuteReader();
                return result.HasRows;
            }
            catch (Exception exQuery)
            {
                error.ErrorOutput(exQuery.Message);
                return false;
            }
        }

        internal bool QueryRelations_Box_Books(string valueID1, string valueID2, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"SELECT box_id, book_id FROM books_boxnumber WHERE box_id = " +
            $"(SELECT box_id FROM boxnumber WHERE box_number = '{valueID1}') AND book_id = " +
            $"(SELECT book_id FROM books WHERE book_title = '{valueID2}');";

            try
            {
                var result = command.ExecuteReader();
                return result.HasRows;
            }
            catch (Exception exQuery)
            {

                error.ErrorOutput(exQuery.Message);
                return false;
            }
        }
        internal bool QueryRelations_User_Books(string valueID1, string valueID2, SqliteConnection conn)
        {
            connection = conn;

            command = connection!.CreateCommand();
            command.CommandText = $"SELECT user_id, book_id FROM user_book WHERE user_id = " +
            $"(SELECT user_id FROM users WHERE username = '{valueID1}') AND book_id = " +
            $"(SELECT book_id FROM books WHERE book_title = '{valueID2}');";

            try
            {
                var result = command.ExecuteReader();
                return result.HasRows;
            }
            catch (Exception exQuery)
            {
                error.ErrorOutput(exQuery.Message);
                return false;
            }
        }
        #endregion

    }
}