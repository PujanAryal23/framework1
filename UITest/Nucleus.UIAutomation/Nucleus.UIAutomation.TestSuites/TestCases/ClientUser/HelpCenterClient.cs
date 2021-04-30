using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.HelpCenter;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class HelpCenterClient : AutomatedBaseClient
    {
        #region PRIVATE FILEDS

        private HelpCenterPage _helpCenter;

        #endregion

        #region OVERRIDE METHODS

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                CurrentPage = _helpCenter = QuickLaunch.NavigateToHelpCenter();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Override TestInit to add additional code.
        /// </summary>
        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _helpCenter;
        }
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        #region TEST SUITES

        [Test] //CAR-2903(CAR-2780) CAR-2958(CAR-2949) + CAR-2965(CAR-2994)
        public void Verify_Help_center_contents_and_forms_in_client_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> testData = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var holidayHeader = $"Cotiviti {DateTime.Now.Year.ToString()}-{(DateTime.Now.Year + 1).ToString()} Holidays";
            var infoSectionDescriptions = testData["InfoSectionDescription"].Split(';').ToList();
            var helpDeskSectionHeaders = new List<string> { "Cotiviti", "Operating Hours", holidayHeader };
            var formHeaders = Enum.GetValues(typeof(HelpCenterFormsEnum)).Cast<HelpCenterFormsEnum>().Select(x => x.GetStringValue()).ToList();
            var filenames = testData["Filenames"].Split(',').ToList();
            var urls = testData["Urls"].Split(',').ToList();
            var headerTexts = testData["HeaderTexts"].Split(',').ToList();

            var listofFormContents = new List<string>
            {
                testData["NewUserRequestFormContent"],
                testData["UserTerminationRequestFormContent"],
                testData["ClientReportRequestFormContent"],
                testData["ClientCustomizationFormContent"],
                testData["SupportedWebBrowserPolicyFormContent"],
                testData["NucleusPasswordRequirementsFormContent"],
            };

            try
            {
                _helpCenter.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue());
                _helpCenter.IsFormHeaderPresent().ShouldBeTrue("Is Form header present?");
                _helpCenter.IsSystemInformationPresent().ShouldBeTrue("Is System Information header present?");

                StringFormatter.PrintMessage("Verification of presence of download icon as per forms");
                foreach (var header in Enum.GetValues(typeof(HelpCenterFormsEnum)).Cast<HelpCenterFormsEnum>()
                    .Select(x => x.GetStringValue()).ToList())
                {
                    _helpCenter.IsDownloadIconPresentByFormName(header).ShouldBeTrue($"Is {header} form present?");
                }

                StringFormatter.PrintMessage("Verification of header texts");
                _helpCenter.GetAdobeAcrobatReaderText().ShouldBeEqual(headerTexts[0], "Adobe reader text should match");
                _helpCenter.GetHelpDeskHeader().ShouldBeEqual(headerTexts[1], "Help Desk header should match");

                StringFormatter.PrintMessage("Verifying Cotiviti, Operating Hours and Cotiviti Holidays and texts under those headers");
                _helpCenter.GetHelpDeskHeadersList().ShouldCollectionBeEqual(helpDeskSectionHeaders, "Headers under Help Desk section should match");
                for (int i = 0; i < helpDeskSectionHeaders.Count; i++)
                {
                    if (i.Equals(2))
                    {
                        _helpCenter.GetCountOfHolidays(holidayHeader).ShouldBeGreater(0, "Holiday list should not be empty");
                    }
                    else
                    {
                        string.Join(" ", _helpCenter.GetInfoSectionDescriptionByHeader(helpDeskSectionHeaders[i]))
                            .ShouldBeEqual(infoSectionDescriptions[i], $"{helpDeskSectionHeaders} description should match");
                    }
                }

                StringFormatter.PrintMessage("Verifying Adobe Reader Link");
                _helpCenter.ClickAdobeReader();
                _helpCenter.SwitchToLastWindow();
                _helpCenter.GetCurrentUrl().ShouldBeEqual(urls[0], "Clicking on Adobe link should navigate to Adobe Reader page");
                _helpCenter.SwitchTab("0");
                _helpCenter.CloseAnyTabIfExist();

                StringFormatter.PrintMessage("Verification of downloaded files");
                string filename;

                for (int i = 0; i < formHeaders.Count; i++)
                {
                    filename = _helpCenter.ClickDownloadIconByHeaderAndGetFilename(formHeaders[i]);
                    _helpCenter.WaitForFileExists(@"C:/Users/uiautomation/Downloads/" + filename);
                    filename.ShouldBeEqual(filenames[i]);

                    StringFormatter.PrintMessage($"Verifying PDF contents for {formHeaders[i]}");
                    ValidatePDFContents(filename, listofFormContents[i]);

                    ExcelReader.DeleteFileIfAlreadyExists(filename);
                    _helpCenter.ClickOnBrowserBackButton();
                    _helpCenter.WaitForPageToLoad();
                }

                StringFormatter.PrintMessage("Verification of Cotiviti link");
                _helpCenter.ClickOnCotivitiUrl();
                _helpCenter.SwitchToLastWindow();
                _helpCenter.GetCurrentUrl().ShouldBeEqual(urls[1], "Clicking Cotiviti url should navigate to Cotiviti page");
                _helpCenter.SwitchTab("0");
                _helpCenter.CloseAnyTabIfExist();
            }

            finally
            {
                foreach (var file in filenames)
                {
                    ExcelReader.DeleteFileIfAlreadyExists(file);
                }
            }
        }
        #endregion

        #region PUBLIC METHODS

        public void ValidatePDFContents(string fileName, string formContent)
        {
            using (PdfReader reader = new PdfReader("C:/Users/uiautomation/Downloads/" + fileName))
            {
                string currentText = "";

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    ITextExtractionStrategy Strategy = new SimpleTextExtractionStrategy();
                    currentText += PdfTextExtractor.GetTextFromPage(reader, i, Strategy);
                }

                var trimmedExpectedReportText = formContent.Split('\n').Select(s => s.Replace("\r", ""))
                    .Select(s => Regex.Replace(s, @"\s+", "")).ToList();

                foreach (var content in trimmedExpectedReportText)
                {
                    var pageTrim = Regex.Replace(currentText, @"[\s+\(\)-]", "").Replace("ﬁ", "fi");
                    var contentTrim = Regex.Replace(content, @"[\s+\(\)-]", "");

                    pageTrim.AssertIsContained(contentTrim, "Is the expected text present in PDF Document?");
                }
            }
        }

        #endregion
    }
}
