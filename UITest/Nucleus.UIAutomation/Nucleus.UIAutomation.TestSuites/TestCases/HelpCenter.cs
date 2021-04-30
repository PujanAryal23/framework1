using NUnit.Framework;
using Nucleus.Service.PageServices.HelpCenter;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.UIAutomation.TestSuites.Base;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Nucleus.Service.Data;
using static System.String;
using Nucleus.Service.PageServices.ChromeDownLoad;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class HelpCenter : NewAutomatedBase
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
        public void Verify_Help_center_contents_and_forms()
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
                _helpCenter.GetHelpDeskHeadersList().ShouldCollectionBeEqual(helpDeskSectionHeaders,
                    "Headers under Help Desk section should match");

                for (int i = 0; i < helpDeskSectionHeaders.Count; i++)
                {
                    if (i.Equals(2))
                    {
                        _helpCenter.GetCountOfHolidays(holidayHeader)
                            .ShouldBeGreater(0, "Holiday list should not be empty");
                    }
                    else
                    {
                        Join(" ", _helpCenter.GetInfoSectionDescriptionByHeader(helpDeskSectionHeaders[i]))
                            .ShouldBeEqual(infoSectionDescriptions[i], $"{helpDeskSectionHeaders} description should match");
                    }
                }

                StringFormatter.PrintMessage("Verifying Adobe Reader Link");
                _helpCenter.ClickAdobeReader();
                _helpCenter.SwitchToLastWindow();
                _helpCenter.GetCurrentUrl()
                    .ShouldBeEqual(urls[0], "Clicking on Adobe link should navigate to Adobe Reader page");
                _helpCenter.SwitchTab("0");
                _helpCenter.CloseAnyTabIfExist();

                StringFormatter.PrintMessage("Verification of downloaded files");
                string filename;

                for (int i = 0; i < formHeaders.Count; i++)
                {
                    filename = _helpCenter.ClickDownloadIconByHeaderAndGetFilename(formHeaders[i]);
                    _helpCenter.WaitForFileExists(@"C:/Users/uiautomation/Downloads/" + filename);

                    StringFormatter.PrintMessage($"Verifying PDF contents for {formHeaders[i]}");
                    ValidatePDFContents(filename, listofFormContents[i]);
                    
                    ExcelReader.DeleteFileIfAlreadyExists(filename);
                    _helpCenter.ClickOnBrowserBackButton();
                    _helpCenter.WaitForPageToLoad();
                }

                StringFormatter.PrintMessage("Verification of Cotiviti link");
                _helpCenter.ClickOnCotivitiUrl();
                _helpCenter.SwitchToLastWindow();
                _helpCenter.GetCurrentUrl()
                    .ShouldBeEqual(urls[1], "Clicking Cotiviti url should navigate to Cotiviti page");
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

        //If_Client_is_designated_as_part_of_the_Fraud_Alliance_and_user_does_not_have_Fraud_Alliance_authority_then_user_does_not_see_Alliance_menu_option_or_Quick_Launch_tile[Test, Category("SmokeTest")]
        public void Verify_help_center_download_link_works()
        {
            StringFormatter.PrintMessageTitle("Verify Help Center Links");
            _helpCenter.IsClaimsEditingQuickStartGuideLinkValid().ShouldBeTrue("ClaimsEditingQuickStartGuideLink Valid");
            _helpCenter.IsFraudPreventionQuickStartGuideLinkValid().ShouldBeTrue("FraudPreventionQuickStartGuideLink Valid");
            _helpCenter.IsFraudPreventionOnlineHelpLinkValid().ShouldBeTrue("FraudPreventionOnlineHelpLink Valid");
            _helpCenter.IsClaimsEditingOnlineHelpLinkValid().ShouldBeTrue("ClaimsEditingOnlineHelpLink Valid");
            
            _helpCenter.ClickFormsTab();
            _helpCenter.IsNewUserRequestFormLinkValid().ShouldBeTrue("NewUserRequestFormLink Valid");
            _helpCenter.IsClientCustomizationFormLinkValid().ShouldBeTrue("ClientCustomizationFormLink Valid");
            _helpCenter.IsUserTerminationRequestFormLinkValid().ShouldBeTrue("UserTerminationRequestFormLink Valid");
            _helpCenter.IsUserReportRequestFormLinkValid().ShouldBeTrue("UserReportRequestFormLink Valid");

            _helpCenter.ClickSystemTab();
            _helpCenter.IsFraudPreventionEobFlagDescLinkValid().ShouldBeTrue("FraudPreventionEobFlagDescLink Valid");
            _helpCenter.IsClaimsEditingEobFlagDescLinkValid().ShouldBeTrue("ClaimsEditingEobFlagDescLink Valid");

            _helpCenter.ClickReleaseTab();
            _helpCenter.IsRulesEngineReleaseNotesLinkValid().ShouldBeTrue("RulesEngineReleaseNotesLink Valid");
            StringFormatter.PrintLineBreak();
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
