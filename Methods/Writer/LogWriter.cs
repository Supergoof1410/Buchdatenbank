using System;
using System.IO;
using System.Windows;

namespace Buchdatenbank.Methods.Writer
{
    class LogWriter
    {
        public StreamWriter? writeLogFile;

        internal void WriteLog(string LogMessage)
        {
            string LogDir = @".\Log\";
            string LogFile = @"Log.txt";
            string LogPath = LogDir + LogFile;

            try
            {
                // Wenn das Verzeichnis nicht existiert, dann wird ein neues angelegt!
                if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);

                // Wenn die Datei nicht existiert, dann wird eine neue erstellt!
                if (!File.Exists(LogPath))
                {
                    File.Create(LogPath).Close();
                }

                writeLogFile = new StreamWriter(LogPath, append: true);
                writeLogFile.WriteLine(LogMessage);
                writeLogFile.Close();

            }
            catch (Exception exLogWriter)
            {
                MessageBox.Show(exLogWriter.Message);
                writeLogFile!.Close();
                throw;
            }
        }
    }
}
