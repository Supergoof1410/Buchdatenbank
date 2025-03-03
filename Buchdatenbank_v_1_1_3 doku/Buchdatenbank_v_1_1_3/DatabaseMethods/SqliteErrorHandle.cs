using Buchdatenbank.Methods.Writer;
using System;

namespace Buchdatenbank
{
    internal class SqliteErrorHandle
    {
        internal LogWriter writeToLogSql = new();
        public NotifyMethodChanged sqlGetQueryInfo = NotifyMethodChanged.Instance;

        #region Fehlerausgabe
        internal void ErrorOutput(string message)
        {
            sqlGetQueryInfo.DebugInfo += $"[{DateTime.Now}] - [User: {Environment.UserName}] - [SQLError] - " + message + "\n";
            writeToLogSql.WriteLog($"[{DateTime.Now}] - [User: {Environment.UserName}] - [SQLError] - " + message);
        }
        #endregion
    }
}