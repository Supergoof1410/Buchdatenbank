using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Buchdatenbank;

public class CsvStreamWriter
{
    public StreamWriter? writeHtml;
    public FileInfo? fInfo;

    internal void WriteToFileCsv(List<Books> CsvFile)
    {
        string path = @".\DatabaseCsv\testCsvBook.csv";
        fInfo = new FileInfo(path);
        try
        {
            // Wenn die Datei nicht existiert, dann wird eine neue erstellt!
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            throw;
        }

        writeHtml = new(path, append: true);

        // Einstellungen für die CSV-Datei, hier besonders wichtig das der Delimiter
        // auf ein Semikolon gesetzt wird, default ist ein Komma, was bei Dezimalzahlen
        // zu Problemen führen kann.
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ";",
        };

        using (var csv = new CsvWriter(writeHtml, config))
        {
            // Für den Fall das die Datei noch keinen Header hat, wird hier ein Header erstellt.
            // Weitere Daten folgen danach ohne den Header nochmal zu schreiben.
            if (fInfo.Length == 0)
            {
                csv.WriteHeader<Books>();
                csv.NextRecord();
            }
            foreach (var record in CsvFile)
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }
        }
    }

}
