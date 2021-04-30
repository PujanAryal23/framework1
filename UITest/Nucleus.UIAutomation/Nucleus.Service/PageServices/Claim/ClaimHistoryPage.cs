using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using static System.String;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Database;
using RadioButton = UIAutomation.Framework.Elements.RadioButton;

namespace Nucleus.Service.PageServices.Claim
{
    public class ClaimHistoryPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private ClaimHistoryPageObjects _claimHistoryPage;
        private readonly string _originalWindow;
        #endregion

        #region CONSTRUCTOR

        public ClaimHistoryPage(INewNavigator navigator, ClaimHistoryPageObjects claimHistoryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, claimHistoryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _claimHistoryPage = (ClaimHistoryPageObjects)PageObject;
        }
        #endregion

        #region PUBLIC METHODS
        public string OriginalWindowHandle
        {
            get { return _originalWindow; }
        }

        public bool Is8MonthsFilterPresent() =>
            JavaScriptExecutor.IsElementPresent(ClaimHistoryPageObjects.EightMonthsHistoryCssSelector);

        public void Click8MonthsFilter()
        {
            JavaScriptExecutor.ClickJQuery(ClaimHistoryPageObjects.EightMonthsHistoryCssSelector);
            WaitForWorkingAjaxMessageOnly();
        }

        public List<string> GetAllDosFromPatientClaimHx() =>
            JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.AllDosFromPatientClaimHxCssSelector);

        public List<string> GetAllDosActiveFromPatientClaimHx() =>
            JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.AllDOSActiveFromPatientClaimHxCssSelector);

        public List<string> GetAllDxCodesFromPatientClaimHx() =>
            JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.AllDxCodesFromPatientClaimHxCssSelector);

        public List<string> GetAllProcCodesFromPatientClaimHx() =>
            JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.AllProcCodesFromPatientClaimHxCssSelector);


        public bool IsAppealIconPresent(string claimSeq)
        {
            return
                SiteDriver.IsElementPresent(
                    string.Format(ClaimHistoryPageObjects.AppealIconByClaimSeqXPathTemplate, claimSeq), How.XPath,1000);
        }

        public ClaimActionPage ClickOnClaimSequence(string claseq)
        {
            var claimAction = Navigator.Navigate(() =>
            {
               SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.ClaimSequenceXPathTemplate,claseq),How.XPath).Click();
                Console.WriteLine("Clicked on claim sequence.");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl(claseq.Split('-')[0]));
                SiteDriver.WaitForCondition(()=>SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimDetailsCssLocator,How.CssSelector));
            }, () => new ClaimActionPageObjects());
            return new ClaimActionPage(Navigator, claimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetAltClaimNoOfClaimSequence(string claimSeq)
        {
            Console.WriteLine("Receiving Alt Claim No of Claim Sequence: {0}", claimSeq);
            return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.AltClaimNoXPathTemplate, claimSeq), How.XPath).Text;
        }

        //public ClaimHistoryPage ClickOnExtendedClaimHx()
        //{
        //    var patientHistory = Navigator.Navigate(() =>
        //                         {
        //                             _claimHistoryPage.ExtendedClaimHistoryLink.Click();
        //                             SiteDriver.WaitForIe(3000);
        //                             Console.WriteLine("Clicked on Extended Claim History.");
        //                             SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ExtendedPageClaimHistory)));
        //                         }, () => new ClaimHistoryPageObjects("retro/Pages/PatientClaimHistory.aspx"));
        //    return new ClaimHistoryPage(Navigator, patientHistory);
        //}

        public void MouseOverDxCode(string tableIndex, string rowIndex)
        {
           JavaScriptExecutor.ExecuteMouseOver(string.Format(ClaimHistoryPageObjects.DxCodeWithIcdXPathTemplate, tableIndex, rowIndex), How.XPath);
           SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimHistoryPageObjects.RadAjaxLoading, How.CssSelector));
        }

        public void MouseOverTinHistory()
        {
            JavaScriptExecutor.ExecuteMouseOver(ClaimHistoryPageObjects.TinHistoryRadioButtonInExtendedPageId,How.Id);
            
        }

        public void MouseOverPatientDX()
        {
           JavaScriptExecutor.ExecuteMouseOver(ClaimHistoryPageObjects.PatientDxRadioButtonInExtendedPageId, How.Id);
            
        }

        public string GetValueOfDxCodeInPopup(string index)
        {
            try
            {
                return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.DxCodeInPopupXPath, index), How.XPath).Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is thrown and catch:  " + ex.Message);
                return string.Empty;
            }
        }

        public void MouseOverDxCodeOfExtendedClaimHx(string rowIndex)
        {
            JavaScriptExecutor.ExecuteMouseOver(string.Format(ClaimHistoryPageObjects.DxCodeWithIcdExtendedClaimHistoryXPath, rowIndex), How.XPath);
            SiteDriver.WaitForCondition(()=>!SiteDriver.IsElementPresent(ClaimHistoryPageObjects.RadAjaxLoading,How.CssSelector));
        }

        public string GetValueOfDxCodeInPopupOfExtendedClaimHx(string dxCodeIndex)
        {
            try
            {
                return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.IcdDxCodeExtendedClaimHistoryXPathTemplates, dxCodeIndex), How.XPath).Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is thrown and catch:  " + ex.Message);
                return string.Empty;
            }
        }

        public bool IsProviderHistoryTextLabelSelected()
        {
            // return _claimHistoryPage.ProviderHistoryTextLabel();
            SiteDriver.WaitForCondition(() => SiteDriver.FindElement(ClaimHistoryPageObjects.ProviderHistoryTextLabelId, How.CssSelector)
                .GetAttribute("class").Contains("history_option selected"), 7000);
            return SiteDriver.FindElement(ClaimHistoryPageObjects.ProviderHistoryTextLabelId, How.CssSelector)
                .GetAttribute("class").Contains("history_option selected");
        }

        public string GetProviderSequenceValue()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.ProviderSequenceTextLabelId, How.Id).Text;
        }

        public ClaimActionPage SwitchToNewClaimActionPage(bool closeClaimHistory = false)
        {
            if(closeClaimHistory)
                SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimAction));
            return new ClaimActionPage(Navigator, new ClaimActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsTinHistoryRadioButtonPresent()
        {
            return (SiteDriver.FindElement(ClaimHistoryPageObjects.TinHistoryTextLabelId, How.Id).Displayed && SiteDriver.FindElement(ClaimHistoryPageObjects.TinHistoryTextLabelId, How.Id).Text == "TIN History");
        }

        public bool IsPatientDxRadioButtonPresent()
        {
            return (SiteDriver.FindElement(ClaimHistoryPageObjects.PatientDxRadioButtonId, How.Id).Displayed && SiteDriver.FindElement(ClaimHistoryPageObjects.PatientDxRadioButtonId, How.Id).Text == "Patient DX");
        }

        public bool IsTinHistoryRadioButtonPresentInExtendedHistoryPage()
        {
            return (SiteDriver.FindElement(ClaimHistoryPageObjects.TinHistoryRadioButtonInExtendedPageId,How.Id).Displayed);
        }

        public bool IsPatientDxRadioButtonPresentInExtendedHistoryPage()
        {
            return (SiteDriver.FindElement(ClaimHistoryPageObjects.PatientDxRadioButtonInExtendedPageId, How.Id).Displayed);
        }

        public void MouseOverOnTableRowFieldCss(int column,int table=1, int row=1)
        {
            JavaScriptExecutor.ExecuteMouseOver(string.Format(ClaimHistoryPageObjects.TableRowFieldCssTemplate, table,row,column), How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }

        public void MouseOverOnProviderCss(int table=1)
        {
            JavaScriptExecutor.ExecuteMouseOver(string.Format(ClaimHistoryPageObjects.ProviderToolTipCssLocator, table), How.CssSelector);
            SiteDriver.WaitToLoadNew(300);
        }
        public string GetTooltipHeaderValue()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.ToolTipHeaderCssLocator, How.CssSelector).Text;
        }

        public string GetTooltipContentValue(int row)
        {
            return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.ToolTipContentCssTemplate, row), How.CssSelector).Text;
        }
        public string GetToolTipTableContentHeader(int column)
        {
            return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.ToolTipTableContentHeaderCssTemplate, column), How.CssSelector).Text;
        }

        public bool IsLoadingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.LoadingAjaxMessageCssLocator, How.CssSelector);
        }

        public string GetRenenueCodeValue()
        {
            return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.TableRowLinkFieldCssTemplate, 1, 1, 6), How.CssSelector).Text;
        }

        public string GetProcedureCodeValue()
        {
            return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.TableRowLinkFieldCssTemplate, 1, 1, 7), How.CssSelector).Text;
        }
        public string GetFlagCodeValue()
        {
            var element = SiteDriver.FindElement(
                string.Format(ClaimHistoryPageObjects.TableRowLinkFieldCssTemplate, 1, 1, 16),How.CssSelector);
            return element.Text;
        }

        public NewPopupCodePage ClickOnRevenueCodeandSwitch(string title)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClaimHistoryPageObjects.TableRowLinkFieldCssTemplate, 1, 1, 6), How.CssSelector);
           return SwitchToPopupCodePage(title);
        }
        public NewPopupCodePage ClickOnRevenueCodeByRevCodeAndSwitch(string title, string revCode)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClaimHistoryPageObjects.RevCodeXpathTemplateByRevCode, revCode), How.XPath);
            return SwitchToPopupCodePage(title);
        }
        public NewPopupCodePage ClickOnProcedureCodeandSwitch(string title)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(ClaimHistoryPageObjects.TableRowLinkFieldCssTemplate, 1, 1, 7), How.CssSelector);
            return SwitchToPopupCodePage(title);
        }

        public FlagPopupPage ClickOnFlagandSwitch(string title)
        {
            JavaScriptExecutor.ExecuteClick(ClaimHistoryPageObjects.FlagLinkOnToolTipHeaderCssLocator, How.CssSelector);
            var popupCode = Navigator.Navigate<FlagPopupPageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(title));
                FlagPopupPageObjects.AssignPageTitle = title;

                Console.WriteLine("Switch To " + title + " Page");
            });
            return new FlagPopupPage(Navigator, popupCode, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public NewPopupCodePage SwitchToPopupCodePage(string title)
        {
            var popupCode = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle( title));
                NewPopupCodePage.AssignPageTitle(title);
               
                Console.WriteLine("Switch To "+title+" Page");
            });
            return new NewPopupCodePage(Navigator, popupCode, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetDataPointsBackGroundColor(int row = 0) //CAR-206
        {
            return SiteDriver.FindElement(string.Format(ClaimHistoryPageObjects.PatientClaimHistoryDentalDataPointsRowXPathTemplate, row), How.XPath).
                GetCssValue("background-color");


        }

        public bool IsBackgroundColorDiff() //CAR-206
        {
            var dentalDataPointBackgroundColor = GetDataPointsBackGroundColor(1);
            var medicalDataPointBackgroundColor = GetDataPointsBackGroundColor(2);
            return dentalDataPointBackgroundColor != medicalDataPointBackgroundColor;
        }

        public List<string> GetDentalDos() //CAR-206
        {
            return JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.DentalDosCssSelector, How.CssSelector, "Text");
        }

      
        public bool IsDOSSorted() //CAR-206
        {
            return GetDentalDos().IsInDescendingOrder();
        }

        public List<string> GetDentalDataPoints() //CAR-206
        {
            var dataPoints = JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.PatientClaimHistoryDentalDataPointsCssSelector, How.CssSelector, "Text");
            return dataPoints.Skip(1).Take(dataPoints.Count - 1).ToList();

        }

        public List<string> GetPreAuthHistoryTableLabel()
        {
            var label = JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.PatientClaimHistoryPreAuthLabelCssSelector,
                How.CssSelector, "Text");
            return label.ToList();
        }

        public List<string> GetDentalDataPointsValue()
        {
            
            var dataPointsValue = JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.DentalDataPointValues, How.XPath, "Text");
            var a = dataPointsValue.Skip(1).Take(16).ToList();
            a.RemoveAt(8);
            return a;

        }

        public List<string> GetDentalDataValuesFromPatHistoryPage()
        {

            var dataPointsValue = JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.DentalDataPointValues, How.XPath, "Text");

            var a = dataPointsValue.Skip(4).Take(3).ToList();

            return a;

        }

        public string MouseOverAndGetToolTipString(int row)
        {
            SiteDriver.MouseOver(String.Format(ClaimHistoryPageObjects.DentalDataPointsPointerXPathTemplate, row), How.XPath);
            return SiteDriver.FindElement(ClaimHistoryPageObjects.ProcDescTooltipCssSelector, How.CssSelector).Text;
        }

        public string GetPatientPreAuthHistoryTableValueByRowAndColumn(int row, int column)
        {
            return SiteDriver.FindElement(Format(ClaimHistoryPageObjects.PatientPreAuthHistoryRowAndColumnTemplate,row,column), How.XPath).Text;
        }

        public string GetDataPointLabel(string label)
        {
            return SiteDriver
                .FindElement(
                    String.Format(ClaimActionPageObjects.DataPointsLabelForFlaggedLineCssSelectorTemplate, label),
                    How.CssSelector).Text;
        }

        public string GetDentalDataPointvalue(string label)
        {
            return SiteDriver
                .FindElement(
                    string.Format(ClaimActionPageObjects.DataPointsLabelForFlaggedLineCssSelectorTemplate, label),
                    How.CssSelector).Text;
        }

        public string GetValueInGridByCol(int col)
        {
            return SiteDriver.FindElement(String.Format(ClaimHistoryPageObjects.DentalDataPointsPointerXPathTemplate, col), How.XPath).Text;
        }
        public List<string> GetDataBaseValueFromDb(string claseq)
        {

            var dataPointValues = Executor.GetCompleteTable(string.Format(ClaimSqlScriptObjects.GetDataPointValueFromDb, claseq)).ToList();
            List<string> listDataPoints = new List<string>();
            for (int i = 0; i < dataPointValues[0].Table.Columns.Count; i++)
            {
                listDataPoints.Add(dataPointValues[0][i].ToString().TrimStart());
            }
            //var x = dataPointValues[0]["DOS"].ToString();

            return listDataPoints;
        }

        public string GetProcInClaimHistory()
        {
            return JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.DentalDataPointValues, How.XPath, "Text").Take(8).ToString();
        }

        public string GetProcDesc(string procCode, string code_type)
        {
            var description = Executor.GetSingleStringValue(string.Format(ClaimSqlScriptObjects.GetProcDescFromDb, procCode, code_type)).ToString();
            return description;
        }

        //public bool IsDentalIconPresent()
        //{
        //    return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.PatClaimHistoryDentalIconCssSelector, How.CssSelector);
        //}

        public bool IsDentalChartPresent(string classname)
        {
            return SiteDriver.IsElementPresent(String.Format(ClaimHistoryPageObjects.DentalChartCssSelectorTemplate,classname), How.CssSelector);
        }

        public bool IsDentalChartSelectorPresent(int col, string message)
        {
            return SiteDriver.IsElementPresent(
                string.Format(ClaimHistoryPageObjects.DentalChartSelectorButtonTemplate, col), How.CssSelector);
        }

        public void ClickDentalIcon()
        {
            SiteDriver.FindElement(ClaimHistoryPageObjects.PatClaimHistoryDentalIconCssSelector, How.CssSelector).Click();
            WaitForWorkingAjaxMessageOnly();


        }
        

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitForCondition(IsWorkingAjaxMessagePresent, 2000);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();

        }

        public void WaitForWorkingAjaxMessageOnly(int t=2000)
        {
           
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent(),t);

        }

        public bool IsProviderHistoryTabPresent()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.ProviderHistoryTabCssSelector, How.CssSelector)
                .Displayed;
        }
        //public void ClickOnChartSelector(int col)
        //{
        //    SiteDriver.FindElement(
        //        string.Format(ClaimHistoryPageObjects.DentalChartSelectorButtonTemplate, col), How.CssSelector).Click();
        //}

        public void ClickOnChartSelector(string category)
        {
            string chartselector = UppercaseFirst(category);
            SiteDriver.FindElement(
                string.Format(ClaimHistoryPageObjects.DentalChartSelectorButtonTemplate, chartselector),How.XPath).Click();
        }

        string UppercaseFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public bool IsChartSelectorSelected(string category)
        {
            return SiteDriver.FindElement(
                    string.Format(ClaimHistoryPageObjects.DentalChartSelectorButtonTemplate, UppercaseFirst(category)),
                    How.XPath).GetAttribute("class").Contains("selected");
        }

        public void ClickDentalQuadrant(string category, string quadrant)
        {
            SiteDriver.FindElement(
                string.Format(ClaimHistoryPageObjects.QuadrantSelectionCssSelectorTemplate, category, quadrant),
                How.CssSelector).Click();
        }

        public void ClickIndividualTooth(string category, string quadrant, int toothnumber)
        {
           JavaScriptExecutor.ExecuteClick(string.Format(ClaimHistoryPageObjects.IndividualURToothSelectorTemplate,category, quadrant, toothnumber), How.CssSelector);
        }

        public bool IsToothNumberSelected(string category, string quadrant, int toothnumber )
        {
             
            return SiteDriver.FindElement(
                string.Format(ClaimHistoryPageObjects.ToothNumberSelectorCssSelector, category, quadrant, toothnumber),
                How.CssSelector).GetAttribute("class").Contains("selected");
        }

        public bool IsSelectedToothNumberCorrect(string toothNumber)
        {
            return SiteDriver.FindElement(
                       Format(ClaimHistoryPageObjects.ToothNumberSelectorCssSelector),
                       How.CssSelector).Text == toothNumber;
        }

        public bool IsDentalQuadrantSelected(string quadrant)
        {

            return SiteDriver.FindElementsCount(string.Format(
                           ClaimHistoryPageObjects.QuadrantClassSelectionCssSelector, quadrant),
                       How.CssSelector) > 0;


        }

        public string GetSelectedQuadrant()
        {
            return JavaScriptExecutor.FindElement(ClaimHistoryPageObjects.SelectedQuadrantJQuerySelector).GetAttribute("id");
        }

        public bool IsDentalDataPresent()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.DentalDataTableCssSelector, How.CssSelector);
        }

        public bool IsNoDataFoundPresent()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.EmptyHistoryDetailCssSelector, How.CssSelector);
        }

        public List<string> GetSelectedDentalNumber(string category, string quadrant)
        {
           return JavaScriptExecutor
                .FindElements(string.Format(ClaimHistoryPageObjects.ToothNumberTextCssSelector,category, quadrant),
                    How.CssSelector,"Attribute:innerHTML");
           
        }

        public void ClickResetButton()
        {
            SiteDriver.FindElement(ClaimHistoryPageObjects.DentalResetButtonCssSelector,How.CssSelector).Click();
        }


        public string GetQuadrantName()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.SelectedQuadrantCssSelector, How.CssSelector)
                .GetAttribute("id");
        }

        public string GetEmptyHistoryText()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.EmptyHistoryDetailCssSelector, How.CssSelector).Text;
        }

        public bool IsDataAvailableinProviderHistoryTab()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.HistoryTableCssSelector, How.CssSelector);
        }
        #region PreAuth

        public bool IsPreAuthIconPresentInHeaderLevel()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.PreAuthId, How.Id);
        }

        public bool IsRedBadgePresentOverPreAuthIcon()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.RedBadgeCssSelector,How.CssSelector);
        }

        public int GetPreAuthCountOnRedBadge()
        {
            return Convert.ToInt32(SiteDriver.FindElement(ClaimHistoryPageObjects.RedBadgeCssSelector, How.CssSelector).Text);
        }


        public ClaimHistoryPage ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory()
        {

            SiteDriver.FindElement(ClaimHistoryPageObjects.PreAuthId, How.Id).Click();
            Console.WriteLine("Clicked on Pre Auth");
            ClaimHistoryPageObjects.AssignPageTitle = PageTitleEnum.PatientPreAuthHistory.GetStringValue();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.PatientPreAuthHistory.GetStringValue()));
            SiteDriver.WaitForPageToLoad();
            //SiteDriver.WaitForCondition(() =>
            //    SiteDriver.IsElementPresent(ClaimHistoryPageObjects.HistoryTableCssSelector, How.CssSelector));
            return new ClaimHistoryPage(Navigator, new ClaimHistoryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickOnClaimHistoryIconByName(string iconName, string pageHeader)
        {
            SiteDriver.FindElement(Format(ClaimHistoryPageObjects.ClaimHistoryIconByName, iconName), How.CssSelector).Click();
            Console.WriteLine($"Clicked on {iconName} icon");

            SiteDriver.WaitForCondition(() => GetPopupPageTitle() == pageHeader);
        }


        public PreAuthActionPage ClickOnAuthSeqLinkAndNavigateToPreAuthActionPopup()
        {

            JavaScriptExecutor.ExecuteClick(ClaimHistoryPageObjects.AuthSeqLink, How.XPath);
            Console.WriteLine("Clicked on Pre Auth");
            ClaimHistoryPageObjects.AssignPageTitle = PageTitleEnum.PatientPreAuthHistory.GetStringValue();
            SiteDriver.WaitForCondition(() =>
                SiteDriver.SwitchWindowByUrl(PageUrlEnum.PreAuthAction.GetStringValue()));
            WaitForWorkingAjaxMessage();


            return new PreAuthActionPage(Navigator, new PreAuthActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public string GetAuthSeq()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.AuthSeqLink, How.XPath).Text;
        }
        #endregion



            public bool IsPatientDxHistoryTabDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.PatientDxTabCssSelector, How.CssSelector)
                .Displayed;
        }

        public bool IsDentalHistoryTabDisplayed()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.DentalTabCssSelector, How.CssSelector)
                .Displayed;
        }

        public bool IsDentalHistoryTabPresent()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.DentalTabCssSelector, How.CssSelector);
        }

        public bool IsProviderHistoryTabSelected()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.ProviderHistoryTabCssSelector, How.CssSelector)
                .GetAttribute("class").Contains("selected");
        }

        public bool IsProviderHistorySelected()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.ProviderHistoryTextLabelId, How.CssSelector)
                .GetAttribute("class").Contains("selected");
        }

        public bool IsPatientDxHistoryTabSelected()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.PatientDxTabCssSelector, How.CssSelector)
                .GetAttribute("class").Contains("selected");
        }

        public bool IsDentalHistoryTabSelected()
        {
            return JavaScriptExecutor.FindElement(ClaimHistoryPageObjects.DentalTabCssSelector)
                .GetAttribute("class").Contains("selected");
        }

        public void ClickProviderHistoryTab()
        {
            SiteDriver.FindElement(ClaimHistoryPageObjects.ProviderHistoryTabCssSelector, How.CssSelector)
                .Click();
            WaitForWorkingAjaxMessage();
        }
        public void ClickPatientDxTab()
        {
            SiteDriver.FindElement(ClaimHistoryPageObjects.PatientDxTabCssSelector, How.CssSelector).Click();
            WaitForWorkingAjaxMessage();
        }

        public void ClickDentalHistoryTab()
        {
            SiteDriver.FindElement(ClaimHistoryPageObjects.DentalTabCssSelector, How.CssSelector).Click();
            WaitForStaticTime(5000);
        }

        public void ResizeWindow(int width, int height)
        {
            SiteDriver.WebDriver.Manage().Window.Size = new Size(width, height);
            WaitForStaticTime(2000);
        }

        public string GetPatientClaimHistoryData(int table, int row, int col)
        {
           return JavaScriptExecutor.FindElement(Format(ClaimHistoryPageObjects.TableRowFieldCssTemplate, table, row,
               col)).Text;
        }

        //public void ClickTinHistoryTab()
        //{
        //    SiteDriver.FindElement(ClaimHistoryPageObjects.tin, How.CssSelector).Click();

        //}

        public bool IsProviderHistoryFilterPresent()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.ProviderHistoryTextLabelId, How.CssSelector).Displayed;
        }

        public bool IsTinHistoryFilterPresent()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.TinHistoryTextLabelId, How.Id).Displayed;
        }

        public bool IsSameDayFilterPresent()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.SameDayTextLabelCssSelector, How.CssSelector).Displayed;
        }

        public bool IsSixtyDaysFilterPresent()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.SixtyDaysTextLabelCssSelector, How.CssSelector).Displayed;
        }

        public bool IsTwelveMonthsFilterPresent()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.TwelveMonthsTextLabelCssSelector, How.CssSelector).Displayed;
        }

        public bool IsAllHistoryFilterPresent()
        {
            return SiteDriver
                .FindElement(ClaimHistoryPageObjects.AllHistoryTextLabelCssSelector, How.CssSelector).Displayed;
        }

        public string GetPatientDentalHistoryData(string row, string col)
        {
            return SiteDriver
                .FindElement(string.Format(ClaimHistoryPageObjects.DentalHistoryDataCssSelector, row, col),
                    How.CssSelector).Text;
        }
        //public void WaitForWorkingAjaxMessage()
        //{
        //    SiteDriver.WaitForCondition(IsWorkingAjaxMessagePresent, 2000);
        //    SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
        //    SiteDriver.WaitForPageToLoad();

        //}

        public void CloseAnyPopupIfExist()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
                }
            }
        }

        public int GetIndexOfSelectedTooth(string category, string quadrant)
        {
            //var list = SiteDriver.FindElement(ClaimHistoryPageObjects.ToothListByQuadrant,
            //    How.CssSelector).GetAttribute("class");
            var list = SiteDriver.FindElementAndGetAttributeByAttributeName(string.Format(ClaimHistoryPageObjects.ToothListByQuadrant,category,quadrant),
                How.CssSelector,"class");
            int index = 0;
          ;
            for(int i = 0; i< list.Count ; i++)
           
            {
                if (list[i].Contains("selected"))
                {
                    int counter = list.IndexOf(list[i]);
                    index=counter;
                }

                
            }

            return index;
            //if (list.Contains("tooth_group selected"))
            //{
            //    var counter = Convert.ToInt32(list.ElementAt(list.IndexOf("tooth_group selected")));
            //    index = counter;
            //}

            // return index;
        }

        


        public void ClickOnHistoryTabs(string tab)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(ClaimHistoryPageObjects.HistoryTabsByNameCssLocator, tab));
            WaitForWorkingAjaxMessageOnly(5000);
        }

        public bool IsHistoryTabSelectedByTabName(string tab)
        {
            return JavaScriptExecutor.FindElement(string.Format(ClaimHistoryPageObjects.HistoryTabsByNameCssLocator, tab))
                .GetAttribute("class").Contains("selected");
        }

        public List<string> GetHistoryOptionList()
        {
            return JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.HistoryOptionListCssLocator, How.CssSelector,
                "Text");
        }

        public List<string> GetPatientHistoryHeaderDetails(List<string> patientHeader)
        {
            List<string> patientHeaderList = new List<string>();
            for (int i = 0; i < patientHeader.Count; i++)
            {
                patientHeaderList.Add(SiteDriver.FindElement(
                        string.Format(ClaimHistoryPageObjects.PatientHistoryHeaderXpathTemplate, patientHeader[i]), How.XPath)
                    .Text);
            }

            return patientHeaderList;
        }

        public string GetEmptyMessageText()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.EmptymessageCssSelector, How.CssSelector)
                .Text;
        }

        public PreAuthActionPage SwitchBackToPreAuthActionPage()
        {

            var newPreAuthAction = Navigator.Navigate<PreAuthActionPageObjects>(() =>

            {
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.PreAuthorization.GetStringValue());
                StringFormatter.PrintMessage("Switch back to Pre Auth Action Page.");
            });
            return new PreAuthActionPage(Navigator, newPreAuthAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public int GetPatientPreAuthHistoryRowCount()
        {
            return JavaScriptExecutor.FindElements(PreAuthActionPageObjects.PatientPreAuthHistoryRowCssSelector, How.CssSelector, "Text").Count();
        }

        public List<string> GetPatientPreAuthHistoryRowValue()
        {
            return JavaScriptExecutor.FindElements(PreAuthActionPageObjects.PatientPreAuthHistoryRowCssSelector,
                How.CssSelector, "Text");
        }
        public List<string> GetPatientPreAuthHistorySummary()
        {
            return JavaScriptExecutor.FindElements(PreAuthActionPageObjects.PatientPreAuthHistorySummaryCssSelector,
                How.CssSelector, "Text");
        }


        public List<string> GetPatientPreAuthHistoryLineDetails(int row = 1)
        {
            int i;
            var details = "";
            List<string> linedetails = new List<string>();
            for ( i = 1; i < 4; i++)
            {
                details = SiteDriver.FindElement(Format(PreAuthActionPageObjects.PatientPreAuthHistoryLineDosSpecialtyCssSelector, row, i),
                        How.CssSelector).Text;
                linedetails.Add(details);
            }
            for (i = 4; i < 12; i++)
            {         
                if (i == 5)
                {
                    details = SiteDriver.FindElement(Format(PreAuthActionPageObjects.PatientPreAuthHistoryProcCssSelector, row),
                        How.CssSelector).Text;
                }
                else
                {
                    details = SiteDriver.FindElement(Format(PreAuthActionPageObjects.PatientPreAuthHistoryScenarioCurrencyCssSelector, row, i),
                        How.CssSelector).Text;
                }           
                linedetails.Add(details);
            }
            linedetails.Add(SiteDriver.FindElement(Format(PreAuthActionPageObjects.PatientPreAuthHistoryFlagCssSelector, row),
                How.CssSelector).Text);
            return linedetails;
        }

        public NewPopupCodePage ClickProcCode(string codeType, string procode, int row = 1)
        {
            string windowName = procode;
            NewPopupCodePage.AssignPageTitle(Format("{0} - {1}", codeType, windowName));
            var popCodePage = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver.FindElement(Format(PreAuthActionPageObjects.PatientPreAuthHistoryProcCssSelector, row),
                    How.CssSelector).Click();
                SiteDriver.SwitchWindowByTitle(windowName);
            });
            return new NewPopupCodePage(Navigator, popCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public FlagPopupPage ClickFlag(string flag, int row = 1)
        {
            string windowName = flag;
            FlagPopupPage.AssignPageTitle(Format("Flag Information - {0}",  windowName));
            var flagpopCodePage = Navigator.Navigate<FlagPopupPageObjects>(() =>
            {
                SiteDriver.FindElement(Format(PreAuthActionPageObjects.PatientPreAuthHistoryFlagCssSelector, row),
                    How.CssSelector).Click();
                SiteDriver.FindElement(PreAuthActionPageObjects.PateintPreAuthHistoryFlagPopUpXPathTemplate, How.XPath).Click();
                SiteDriver.SwitchWindowByTitle(windowName);
            });
            return new FlagPopupPage(Navigator, flagpopCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetDxCodeValue()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.DxCodeInputCssSelector, How.CssSelector).GetAttribute("value");
        }

        public void SetDxCodeValue(string value)
        {
            var element = SiteDriver.FindElement(ClaimHistoryPageObjects.DxCodeInputCssSelector, How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(value);
            Console.WriteLine("Dx Code set to " + value);
        }

        public string GetProcCodeValue()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.ProcCodeInputCssSelector, How.CssSelector).GetAttribute("value");
        }

        public void SetProcCodeValue(string value)
        {
            var element = SiteDriver.FindElement(ClaimHistoryPageObjects.ProcCodeInputCssSelector, How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(value);
            Console.WriteLine("Proc Code set to " + value);
        }

        public string GetDOSValue()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.DOSInputCssSelector, How.CssSelector).GetAttribute("value");
        }

        public bool IsViewEmpty()
        {
            return SiteDriver.FindElement(ClaimHistoryPageObjects.TablePresentCssSelector, How.CssSelector).GetAttribute("style").Equals("display: none;");
        }

        public void SetDOSValue(string date)
        {
            var element = SiteDriver.FindElement(ClaimHistoryPageObjects.DOSInputCssSelector, How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(date);
            Console.WriteLine("DOS set to " + date);
        }

        public string GetDxCodeInputFieldPlaceHolder()
        {
            return JavaScriptExecutor.FindElement(ClaimHistoryPageObjects.DxCodeInputCssSelector)
                .GetAttribute("placeholder");
        }

        public string GetProcCodeInputFieldPlaceHolder()
        {
            return JavaScriptExecutor.FindElement(ClaimHistoryPageObjects.ProcCodeInputCssSelector)
                .GetAttribute("placeholder");
        }

        public void ClickOnClearButton()
        {
            SiteDriver.FindElement(ClaimHistoryPageObjects.ClearButtonCssSelector, How.CssSelector).Click();
        }

        public void ClickOnFilterButton()
        {
            SiteDriver.FindElement(ClaimHistoryPageObjects.FilterButtonCssSelector, How.CssSelector).Click();
        }

        public bool IsHeaderRowPresent()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.TableHeaderCssSelector,How.CssSelector);
        }

        public bool IsFooterRowPresent()
        {
            return SiteDriver.IsElementPresent(ClaimHistoryPageObjects.TableFooterCssSelector,How.CssSelector);
        }

        public string GetDOSInputFieldPlaceHolder()
        {
            return JavaScriptExecutor.FindElement(ClaimHistoryPageObjects.DOSInputCssSelector)
                .GetAttribute("placeholder");
        }

        public bool IsDxCodeInputFieldPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ClaimHistoryPageObjects.DxCodeInputCssSelector);
        }

        public bool IsProcCodeInputFieldPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ClaimHistoryPageObjects.ProcCodeInputCssSelector);
        }

        public bool IsDOSInputFieldPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ClaimHistoryPageObjects.DOSInputCssSelector);
        }

        public bool IsClearButtonPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ClaimHistoryPageObjects.ClearButtonCssSelector);
        }

        public bool IsFilterButtonPresent()
        {
            return JavaScriptExecutor.IsElementPresent(ClaimHistoryPageObjects.FilterButtonCssSelector);
        }

        public PreAuthActionPage ClickPreAuthSeq()
        {
            var preAuthActionPopCodePage = Navigator.Navigate<PreAuthActionPageObjects>(() =>
            {
                SiteDriver.FindElement(PreAuthActionPageObjects.PatientPreAuthHistoryAuthSeqCssSelector, How.CssSelector).Click();
            });
            return new PreAuthActionPage(Navigator, preAuthActionPopCodePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsHugeDataMessageSectionPresent() =>
            SiteDriver.IsElementPresent(ClaimHistoryPageObjects.HugeDataLoadErrorMessageCssSelector, How.CssSelector);

        public string GetHugeDataLoadMessage() => SiteDriver
            .FindElement(ClaimHistoryPageObjects.HugeDataLoadErrorMessageCssSelector, How.CssSelector).Text;

        public void CloseHugeDataLoadSection() => SiteDriver
            .FindElement(ClaimHistoryPageObjects.CloseHugeDataLoadMessageSectionCssSelector, How.CssSelector).Click();

        public List<string> GetDisplayedClaimSeqListFromClaimHistory() =>
            JavaScriptExecutor.FindElements(ClaimHistoryPageObjects.ClaimHistoryClaimSequenceCssSelector);

        public string GetDosByClaimSeqAndRowFromClaimHistory(string claSeq, int row = 1) => SiteDriver
            .FindElement(Format(ClaimHistoryPageObjects.ClaimHistoryDosXPathTemplate, claSeq, row), How.XPath).Text;

        public List<string> GetClaimSeqListForHugeDataLoadFromDb(string patseq, string prvseq)
        {
            var result = Executor.GetTableSingleColumn(Format(ClaimSqlScriptObjects.GetClaimsForHugeDataLoad, patseq, prvseq));
            result.TrimExcess();
            return result;
        }

        public bool IsColumnNamePresentByRow(string columnName, int row) =>
            JavaScriptExecutor.IsElementPresent(Format(ClaimHistoryPageObjects.ColumnNameByRowXPathTemplate, columnName, row));

        public bool IsVerticalScrollBarPresentInPatientToothHistoryTab()
        {
            
            const string select = ClaimHistoryPageObjects.PatientPreAuthHistoryDiv;
            Console.WriteLine("Scroll Height: " + GetScrollHeight(select));
            Console.WriteLine("Client Height:" + GetClientHeight(select));
            return GetScrollHeight(select) > GetClientHeight(select);
        }

        public int GetScrollHeight(string select)
        {
            return JavaScriptExecutor.ScrollHeight(select);
        }

        public int GetClientHeight(string select)
        {
            return JavaScriptExecutor.ClientHeight(select);
        }

        public string MouseOverPatientPreAuthTableAndGetToolTipString(int row, int column)
        {
            SiteDriver.MouseOver(String.Format(ClaimHistoryPageObjects.PatientPreAuthHistoryRowAndColumnTemplate, row, column), How.XPath);
            WaitForStaticTime(300);
            var text= SiteDriver.FindElement(ClaimHistoryPageObjects.PatientPreAuthHistoryTooltipCssSelector, How.CssSelector).Text;
            return text;
        }
        #endregion


    }
}
