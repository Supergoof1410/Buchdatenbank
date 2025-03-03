using Buchdatenbank.Methods.Writer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Buchdatenbank.Methods.Reader
{
    class ProgramConfiguration
    {
        internal Dictionary<string, string> getSettings(string path)
        {

            Dictionary<string, string> settings = new Dictionary<string, string>();
            LogWriter settingsLog = new();

            if (File.Exists(path))
            {
                FileInfo hiddenFile = new FileInfo(path);
                hiddenFile.Attributes &= ~FileAttributes.Hidden;

                StreamReader reader = new StreamReader
                    (
                    new FileStream
                        (
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read
                        )
                    );
                XmlDocument xmlDoc = new XmlDocument();
                string xmlReader = reader.ReadToEnd();
                reader.Close();

                settingsLog.WriteLog($"[{DateTime.Now}] - Konfiguration erfolgreich geladen!");

                xmlDoc.LoadXml(xmlReader);

                foreach(XmlNode child in xmlDoc.ChildNodes)
                {
                    if(child.Name.Equals("configuration"))
                    {
                        foreach(XmlNode node in child.ChildNodes)
                        {
                            if(node.Name.Equals("add"))
                            {
                                settings.Add(
                                    node.Attributes["key"].Value,
                                    node.Attributes["value"].Value
                                    );
                            }
                        }
                    }
                }
                hiddenFile.Attributes |= FileAttributes.Hidden;
            }
            else
            {
                settingsLog.WriteLog("[Error] - Konfigurationsdatei konnte nicht geladen werden");
            }

            return settings;
        }
    }
}
