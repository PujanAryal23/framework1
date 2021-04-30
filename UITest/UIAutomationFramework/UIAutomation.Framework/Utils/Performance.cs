using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Xsl;

namespace UIAutomation.Framework.Utils
{
    public static class Performance
    {
        /// <summary>
        /// Path of current application directory.
        /// </summary>
        private static string _dirPath;

        /// <summary>
        /// Base directory of an application in current domain.
        /// </summary>
        public static string BaseDirPath
        {
            get { return _dirPath ?? AppDomain.CurrentDomain.BaseDirectory; }
            set { _dirPath = value; }
        }
        /// <summary>
        /// Calculates the and save load times.
        /// </summary>
        /// <param name="webTimings">The web timings.</param>
        public static void CalculateAndSaveLoadTimes(Dictionary<string, object> webTimings, string pagename, int runNo)
        {
            // Calculate and convert the page load times
            var pageLoadTimes = CalculateLoadTimes(webTimings);

            // Write the results to our xml file
            WritePerformanceTimingsToXml(pageLoadTimes, pagename, runNo);
        }

        /// <summary>
        /// Calculates the load times.
        /// </summary>
        /// <param name="webTimings">The web timings.</param>
        /// <returns></returns>
        public static PageLoadTimes CalculateLoadTimes(Dictionary<string, object> webTimings)
        {
            PageLoadTimes pageLoadTimes = new PageLoadTimes();

            if (webTimings != null)
            {
                long pagefullyloaded = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                           ? Convert.ToInt64(webTimings["loadEventEnd"]) -
                                             Convert.ToInt64(webTimings["navigationStart"])
                                           : 0;

                long pageFetchTime = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                         ? Convert.ToInt64(webTimings["responseEnd"]) -
                                           Convert.ToInt64(webTimings["navigationStart"])
                                         : 0;

                long domComplete = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                       ? Convert.ToInt64(webTimings["domComplete"]) -
                                         Convert.ToInt64(webTimings["navigationStart"])
                                       : 0;

                long connect = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                   ? Convert.ToInt64(webTimings["connectEnd"]) -
                                     Convert.ToInt64(webTimings["navigationStart"])
                                   : 0;

                pageLoadTimes.PageFullyLoadedTime = pagefullyloaded;
                pageLoadTimes.PageFetchTime = pageFetchTime;
                pageLoadTimes.PageConnectTime = connect;
                pageLoadTimes.PageDomCompleteTime = domComplete;
            }

            return pageLoadTimes;
        }

        /// <summary>
        /// Writes the performance timings to XML.
        /// </summary>
        /// <param name="pageLoadTimes">The page load times.</param>
        /// <param name="pagename">Name of the page whose performance time is being measured.</param>
        /// <param name="runNo">The value of the runNo of the test to consider cache test</param>
        public static void WritePerformanceTimingsToXml(PageLoadTimes pageLoadTimes, string pagename, int runNo)
        {
            var dateTime = DateTime.Now;
            // Create the xml document containe
            XmlDocument xmlDocument = new XmlDocument();

            // Create the XML Declaration, and append it to XML document
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", null, null);
            xmlDocument.AppendChild(xmlDeclaration);

            // Create the root element
            XmlElement root = xmlDocument.CreateElement("Performance");
            xmlDocument.AppendChild(root);

            AddElementToXml(xmlDocument, root, "TestTime", dateTime.ToString("yyyy-MM-dd-HH-mm-ss"));
            AddElementToXml(xmlDocument, root, "PageName", pagename);
            AddElementToXml(xmlDocument, root, "PageFullyLoadedTime", pageLoadTimes.PageFullyLoadedTime + " ms");
            AddElementToXml(xmlDocument, root, "PageConnectTime", pageLoadTimes.PageConnectTime + " ms");
            AddElementToXml(xmlDocument, root, "PageDomCompleteTime", pageLoadTimes.PageDomCompleteTime+ " ms");
            AddElementToXml(xmlDocument, root, "PageFetchTime", pageLoadTimes.PageFetchTime + " ms");

            // Save the file with todays date and time the test ran
            // It is currently set to use the "mydocuments" folder, but this can be changed
            var dirPath = Path.Combine(BaseDirPath, GetDirName(dateTime));
            // Create the directory if it doesn't exist
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
           // string directory = Path.GetFullPath("UIAutomationBuild")+ "//Performance/";
            string fileName = GetFileName(dirPath, pagename, dateTime)+"-"+runNo+".xml";// directory + pagename + "_" + DateTime.Now.ToString("yyyy_MMMM_dd") + ".xml";
            //if (File.Exists(fileName))
            //{
            //   fileName = GetFileName(dirPath, pagename, dateTime) + "-2.xml";// as intention is to run the test twice everyday for cache issues.
            //} 
            xmlDocument.Save(fileName);

            var myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(BaseDirPath + "//Documents/performance stylesheet.xsl");
            myXslTrans.Transform(fileName, GetFileName(dirPath, pagename, dateTime) + "-" + runNo + ".html");  
           
        }

      

        /// <summary>
        /// Adds the element to XML document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="root">The root.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private static void AddElementToXml(XmlDocument doc, XmlElement root, string key, string value)
        {
            XmlElement xmlElement = doc.CreateElement(key);
            xmlElement.InnerText = value.ToString();

            root.AppendChild(xmlElement);
        }
        /// <summary>
        /// Gets a value in the form of datetime for a directory name.
        /// </summary>
        /// <param name="dateTime">An <see cref="DateTime"/> object containing the datetime value.</param>
        /// <returns>A value contains date time.</returns>
        private static string GetDirName(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Gets a file name for an object with following parameters.
        /// </summary>
        /// <param name="dirPath">An object containing the path of directory.</param>
        /// <param name="fileNamePart">An object containing the part of a file name.</param>
        /// <param name="dateTime">An <see cref="DateTime"/> object containing the datetime value.</param>
        /// <returns></returns>
        private static string GetFileName(string dirPath, string fileNamePart, DateTime dateTime)
        {
            var dateString = dateTime.ToString("yyyy-MM-dd");//-HH-mm-ss
            var fileName = string.Format("{0}\\{1}-{2}", dirPath, fileNamePart, dateString);
            if (fileName.Length > 259)
            {
                fileNamePart = fileNamePart.Substring(0, fileNamePart.Length - (fileName.Length - 259));
                fileName = string.Format("{0}\\{1}-{2}", dirPath, fileNamePart, dateString);
            }
            var count = 1;
            while (File.Exists(fileName))
            {
                fileName = string.Format("{0}\\{1}-{2}-({3})", dirPath, fileNamePart, dateString, ++count);
            }

            return fileName;
        }
    }
    public class PageLoadTimes
    {
        public long PageFullyLoadedTime { get; set; }

        public long PageFetchTime { get; set; }

        public long PageDomCompleteTime { get; set; }

        public long PageConnectTime { get; set; }
    }
}
