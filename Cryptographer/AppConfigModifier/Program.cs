using System;
using System.Configuration;

namespace AppConfigModifier
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                var fileMap = new ExeConfigurationFileMap
                              {
                                  ExeConfigFilename = args[0]
                              };
            // Open App.Config of executable
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            /******************************* Key/Value ************************************************/
            int limit = args.Length - 1;
            for (int i = 1; i < limit; )
            {
                config.AppSettings.Settings.Remove(args[i]);
                config.AppSettings.Settings.Add(args[i], args[++i]);
                i++;
            }
            /******************************************************************************************/
             
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);

            /******************************************************************************************/

            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException("Error found - " + exception.Message);
            }
        }
    }
}
