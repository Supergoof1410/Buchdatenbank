using Microsoft.Data.Sqlite;
using System;

namespace Buchdatenbank
{
    public class Sqlite
    {
        internal SqliteConnection? connection;
        
        private static readonly SqliteErrorHandle sqliteErrorHandle = new();
        internal SqliteErrorHandle error = sqliteErrorHandle;

        public NotifyMethodChanged online = NotifyMethodChanged.Instance;

        internal SqliteConnection? ConnectToSqlite(string Source)
        {
            connection = new SqliteConnection(Source);
            try
            {
                connection.Open();
            }
            catch (Exception exSql)
            {
                error.ErrorOutput(exSql.Message);
                return null;
            }
            return connection;
        }
    }
}