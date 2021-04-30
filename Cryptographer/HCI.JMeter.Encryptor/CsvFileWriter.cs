using System.IO;
using System.Text;

namespace CredentialsEncryptor
{
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(string fileName) : base(fileName, true)
        {
           
        }

        public void WriteRow(CsvRow row)
        {
            var builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }
}
