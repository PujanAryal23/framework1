using System;
using System.IO;

namespace CredentialsEncryptor
{
    public class CsvFileReader : StreamReader
    {
        private char _delimiter = ','; 

        public CsvFileReader(string fileName)
            : base(fileName)
        {

        }

        public CsvFileReader(string fileName, char delimiter)
            : base(fileName)
        {
            _delimiter = delimiter;
        }

        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;
            string[] values = row.LineText.Split(_delimiter);
            foreach (string value in values)
                row.Add(value);
            return (row.Count > 0);
        }
    }
}
