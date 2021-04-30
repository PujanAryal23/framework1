using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jmeter.TestSettingsConfigurer
{
    class Program
    {
        static void Main(string[] args)
        {
            string keyString = "";
            string valueString = "";

            try
            {
                
                FileStream fs = new FileStream(args[0], FileMode.Open,FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(fs);
                StreamWriter streamWriter = new StreamWriter(fs);
                Dictionary<string, string> keyvalues = args.Select(a => a.Split(new[] { '=' }, 2))
                    .GroupBy(a => a[0], a => a.Length == 2 ? a[1] : null)
                    .ToDictionary(g => g.Key, g => g.FirstOrDefault());

                keyString = streamReader.ReadLine();
                valueString = streamReader.ReadLine();

                List<string> keyList = keyString.Split(',').ToList();
                List<string> valueList = valueString.Split(',').ToList();

                foreach (KeyValuePair<string, string> kv in keyvalues)
                {
                    if (keyList.Contains(kv.Key))
                    {
                            int indexKey = keyList.IndexOf(kv.Key);
                            valueList[indexKey] = kv.Value;
                    }


                }
                fs.SetLength(0);

                keyString = String.Join(",", keyList);
                valueString = String.Join(",", valueList);
                streamWriter.WriteLine(keyString);
                streamWriter.WriteLine(valueString);
                streamWriter.Close();

                fs.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("The File could not be read:");
                Console.WriteLine(e.Message);

                Console.ReadLine();
            }
        }
    }
}
