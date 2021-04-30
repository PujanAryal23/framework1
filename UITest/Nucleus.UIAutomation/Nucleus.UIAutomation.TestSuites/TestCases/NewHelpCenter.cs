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
using System.Threading.Tasks;
using Nucleus.Service;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.ChromeDownLoad;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class NewHelpCenter
    {
        #region PRIVATE FILEDS


        #endregion


        #region OVERRIDE METHODS

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}

        /// <summary>
        /// Override TestInit to add additional code.
        /// </summary>
        //protected override void TestInit()
        //{
        //    base.TestInit();
        //    CurrentPage = _helpCenter;
        //}

        #endregion

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTest")]
        public void Navigate_to_help_center_page_through_help_center_button()
        {
            using (var newAutomatedBase = new NewAutomatedBase())
            {
                NewHelpCenterPage _helpCenter;
                newAutomatedBase.ClassInit();
                newAutomatedBase.CurrentPage = _helpCenter = newAutomatedBase.QuickLaunch.NavigateToHelpCenter();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                StringFormatter.PrintMessageTitle("Verify the title of Page");
                _helpCenter.PageTitle.ShouldBeEqual(_helpCenter.CurrentPageTitle, "PageTitle");
                StringFormatter.PrintLineBreak();
                _helpCenter.SwitchTabAndNavigateToQuickLaunchPage(true);
                _helpCenter.SwitchToLastWindow();
                _helpCenter.CloseAnyPopupIfExist();
                newAutomatedBase.CurrentPage.Logout();
            }
            
        }



        [Test] //CAR-2903(CAR-2780)
        public void Verify_Help_center_contents_and_forms()
        {
            using (var newAutomatedBase = new NewAutomatedBase())
            {
                NewHelpCenterPage _helpCenter;
                newAutomatedBase.ClassInit();
                newAutomatedBase.CurrentPage = _helpCenter = newAutomatedBase.QuickLaunch.NavigateToHelpCenter();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    newAutomatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var holidayHeader =
                    $"Cotiviti {DateTime.Now.Year.ToString()}-{(DateTime.Now.Year + 1).ToString()} Holidays";
                var infoSectionDescriptions = testData["InfoSectionDescription"].Split(';').ToList();
                var helpDeskSectionHeaders = new List<string> {"Cotiviti", "Operating Hours", holidayHeader};
                var holidaysDesc = string.Join(" ", _helpCenter.GetHolidaysForCurrentYearFromDb());
                var formHeaders = Enum.GetValues(typeof(HelpCenterFormsEnum)).Cast<HelpCenterFormsEnum>()
                    .Select(x => x.GetStringValue()).ToList();
                var filenames = testData["Filenames"].Split(',').ToList();
                var urls = testData["Urls"].Split(',').ToList();
                var headerTexts = testData["HeaderTexts"].Split(',').ToList();
                string filename;
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
                    _helpCenter.GetAdobeAcrobatReaderText()
                        .ShouldBeEqual(headerTexts[0], "Adobe reader text should match");
                    // _helpCenter.GetHelpDeskHeader().ShouldBeEqual(headerTexts[1], "Help Desk header should match");

                    StringFormatter.PrintMessage(
                        "Verifying Cotiviti, Operating Hours and Cotiviti Holidays and texts under those headers");
                    _helpCenter.GetHelpDeskHeadersList().ShouldCollectionBeEqual(helpDeskSectionHeaders,
                        "Headers under Help Desk section should match");
                    for (int i = 0; i < helpDeskSectionHeaders.Count; i++)
                    {
                        if (i.Equals(2))
                        {
                            string.Join(" ",
                                    _helpCenter.GetInfoSectionDescriptionByHeader(helpDeskSectionHeaders[i]
                                        .Substring(0, 9)))
                                .ShouldBeEqual(holidaysDesc, "Holidays should match");
                        }
                        else
                        {
                            string.Join(" ", _helpCenter.GetInfoSectionDescriptionByHeader(helpDeskSectionHeaders[i]))
                                .ShouldBeEqual(infoSectionDescriptions[i],
                                    $"{helpDeskSectionHeaders} description should match");
                        }
                    }

                    StringFormatter.PrintMessage("Verifying Adobe Reader Link");
                    _helpCenter.ClickAdobeReader();
                    _helpCenter.SwitchToLastWindow();
                    _helpCenter.GetCurrentUrl()
                        .ShouldBeEqual(urls[0], "Clicking on Adobe link should navigate to Adobe Reader page");
                    _helpCenter.SwitchTab("0");
                    _helpCenter.CloseAnyTabIfExist();

                    //StringFormatter.PrintMessage("Verification of downloaded files");
                    //for (int i = 0; i < formHeaders.Count; i++)
                    //{
                    //    filename = _helpCenter.ClickDownloadIconByHeaderAndGetFilename(formHeaders[i]);
                    //    //_helpCenter.WaitForFileExists(@"C:/Users/i11143/Downloads/" + filename);
                    //    //filename.ShouldBeEqual(filenames[i]);
                    //    //ExcelReader.DeleteFileIfAlreadyExists(filename);
                    //    _helpCenter.ClickOnBrowserBackButton();
                    //    _helpCenter.WaitForPageToLoad();
                    //}

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
                    newAutomatedBase.CurrentPage.Logout();
                }


            }

            #endregion
        }
    }
}

