using Buchdatenbank.Methods.Reader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buchdatenbank.Methods.Writer
{
    class BackupWriter
    {

        internal static void BackupDBFile()
        {
            LogWriter backupLog = new LogWriter();
            ProgramConfiguration pConfig = new ProgramConfiguration();
            DirectoryInfo? dirInfo = null;

            string? dirPath = null;
            string? sourceDirectory = null;
            string? destinationDirectory = null;
            Dictionary<string, string> hashConfig = pConfig.getSettings(@".\settings.config");

            string dbFile = @"buchdatenbank.db";
            if (hashConfig.ContainsKey("backupFile"))
            {
                sourceDirectory = hashConfig.GetValueOrDefault("backupFile");
            }
            if (hashConfig.ContainsKey("backupDirectory"))
            {
                dirPath = hashConfig.GetValueOrDefault("backupDirectory");

                if (!Directory.Exists(dirPath))
                {
                    dirInfo = Directory.CreateDirectory(dirPath!);
                }

                dirInfo = new DirectoryInfo(dirPath);
                dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                destinationDirectory = dirPath;
            }

            FileSystemInfo fsi = new FileInfo(sourceDirectory + dbFile);

            DateTime actualDate = DateTime.Now;
            DateTime lastBackupDate = fsi.CreationTime;

            if (lastBackupDate < actualDate)
            {
                dirInfo!.Attributes &= ~FileAttributes.Hidden;
                File.Copy(sourceDirectory!, destinationDirectory + dbFile, true);
                dirInfo.Attributes |= FileAttributes.Hidden;
                backupLog.WriteLog($"[{DateTime.Now}] - [User: {Environment.UserName}] -" + " Sicherung erfolgreich!");
            }
            else
            {
                backupLog.WriteLog($"[{DateTime.Now}] - [User: {Environment.UserName}] -" + " Sicherung nicht erfolgreich!");
            }
        }
    }
}
