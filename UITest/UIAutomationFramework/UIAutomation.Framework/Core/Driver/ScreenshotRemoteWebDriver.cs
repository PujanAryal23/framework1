using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;

namespace UIAutomation.Framework.Core.Driver
{
    /// <summary>
    /// Provides a way to use the RemoteWebdDriver and ITakesScreenshot.
    /// </summary>
    public sealed class ScreenshotRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
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
        /// Get or set name for current screenshot file.
        /// </summary>
        public static string FullQualifiedFileName { get; set; }

        /// <summary>
        /// Initializes a new instance of the ScreenshotRemoteWebDriver class.
        /// </summary>
        /// <param name="capabilities">An <see cref="ICapabilities"/> object containing the desired capabilities of the browser.</param>
        public ScreenshotRemoteWebDriver(ICapabilities capabilities)
            : base(capabilities)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ScreenshotRemoteWebDriver class.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver remote server (e.g. http://127.0.0.1:4444/wd/hub).</param>.
        /// <param name="capabilities">An <see cref="ICapabilities"/> object containing the desired capabilities of the browser.</param>
        public ScreenshotRemoteWebDriver(Uri remoteAddress, ICapabilities capabilities)
            : base(remoteAddress, capabilities)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ScreenshotRemoteWebDriver class.
        /// </summary>
        /// <param name="commandExecutor">URI containing the address of the WebDriver remote server (e.g. http://127.0.0.1:4444/wd/hub).</param>.
        /// <param name="capabilities">An <see cref="ICapabilities"/> object containing the desired capabilities of the browser.</param>
        public ScreenshotRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities capabilities)
            : base(commandExecutor, capabilities)
        {
        }

        /// <summary>
        /// Captures an object representing the image of the page on the screen.
        /// </summary>
        /// <param name="dirPath">A directory path for storing an object representing the image of the page on the screen.</param>
        /// <param name="name">A name for an object representing the image of the page on the screen.</param>
        public void CaptureScreenShot(string dirPath, string name)
        {
            BaseDirPath = dirPath;
            CaptureScreenShot(name);
        }

        /// <summary>
        /// Captures an object representing the image of the page on the screen.
        /// </summary>
        /// <param name="name">A name for an object representing the image of the page on the screen</param>
        public void CaptureScreenShot(string name)
        {
            var dateTime = DateTime.Now;
            var dirPath = Path.Combine(BaseDirPath, GetDirName(dateTime));
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            FullQualifiedFileName = GetFileName(dirPath, name, dateTime);
            var screenshot = GetScreenshot();
            screenshot.SaveAsFile(FullQualifiedFileName,ScreenshotImageFormat.Png);
        }

        /// <summary>
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen.
        /// </summary>
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
        public Screenshot GetScreenshot()
        {
            Response screenshotRespnose = this.Execute(DriverCommand.Screenshot, null);
            string base64 = screenshotRespnose.Value.ToString();

            return new Screenshot(base64);
        }

        /// <summary>
        /// Gets a value in the form of datetime for a directory name.
        /// </summary>
        /// <param name="dateTime">An <see cref="DateTime"/> object containing the datetime value.</param>
        /// <returns>A value contains date time.</returns>
        private string GetDirName(DateTime dateTime)
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
        private string GetFileName(string dirPath, string fileNamePart, DateTime dateTime)
        {
            var dateString = dateTime.ToString("yyyy-MM-dd-HH-mm-ss");
            var fileName = string.Format("{0}\\{1}-{2}.png", dirPath, fileNamePart, dateString);
            if (fileName.Length > 259)
            {
                fileNamePart = fileNamePart.Substring(0, fileNamePart.Length - (fileName.Length - 259));
                fileName = string.Format("{0}\\{1}-{2}.png", dirPath, fileNamePart, dateString);
            }
            var count = 1;
            while (File.Exists(fileName))
            {
                fileName = string.Format("{0}\\{1}-{2}-({3}).png", dirPath, fileNamePart, dateString, ++count);
                }

            return fileName;
        }
    }
}


