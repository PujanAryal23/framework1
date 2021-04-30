using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.References;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Reference;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using OpenQA.Selenium;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;



namespace Nucleus.Service.PageServices.Reference
{
    public class ReferenceManagerPage :NewDefaultPage
    {

        #region PRIVATE FIELDS

        private readonly ReferenceManagerPageObjects _referenceManagerPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly GridViewSection _gridViewSection;
        private readonly SideWindow _sideWindow;



        #endregion

        #region CONSTRUCTOR

        public ReferenceManagerPage(INewNavigator navigator, ReferenceManagerPageObjects referenceManagerPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
        : base(navigator, referenceManagerPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _referenceManagerPage = (ReferenceManagerPageObjects)PageObject;
            _commonSqlObjects = new CommonSQLObjects(Executor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);

        }

        public CommonSQLObjects GetCommonSql
        {
            get { return _commonSqlObjects; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }


        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }


        #endregion

        #region Database operation

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public int GetActiveReferenceCount()
        {
            return (int)Executor.GetSingleValue(ReferenceManagerSqlScriptsObjects.ActiveReferenceCount);
        }

        public List<string> GetFlagLists()
        {
            return
                Executor.GetTableSingleColumn(ReferenceManagerSqlScriptsObjects.FlagList);
        }

        public List<string> GetTrigProcValues()
        {
            return Executor.GetTableSingleColumn(ReferenceManagerSqlScriptsObjects.ProcTrigList);
        }

        public List<string> GetActiveClientList(string userSeq)
        {
            var list = Executor.GetTableSingleColumn(string.Format(ReferenceManagerSqlScriptsObjects.GetClientListAssignedToUser, userSeq));
            //list.Insert(0,"ALL Centene");
            return list;
        }

        public void DeleteClaimReferenceFromDB(string[] newClaimReference, bool isActive = true)
        {
            var active = isActive ? 'T' : 'F';
            Executor.ExecuteQuery(string.Format
               (ReferenceManagerSqlScriptsObjects.DeleteClaimReference, newClaimReference[1], newClaimReference[2], newClaimReference[3], newClaimReference[4], newClaimReference[5], active));
        }

        //public void DeleteClaimReferenceWithoutTriggerFromDB(string[] newClaimReference)
        //{
        //    Executor.ExecuteQuery(string.Format
        //        (ReferenceManagerSqlScriptsObjects.DeleteClaimReferenceWithoutTrigCoge, newClaimReference[1], newClaimReference[2], newClaimReference[3]));
        //}
        #endregion

        public List<string> SearchRowHavingTricProc()
        {
            var txt = SiteDriver.FindElement(ReferenceManagerPageObjects.SearchRowHavingTricProcXPath,
                How.XPath).Text.Replace("\r", string.Empty);
            var list = txt.Split('\n').ToList();
            return new List<string> { list[3], list[5], list[7], list[9] };
        }

        public bool IsReferenceManagerSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.ReferenceManager), How.XPath);
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public bool IsAddIconPresent()
        {
            return SiteDriver.IsElementPresent((ReferenceManagerPageObjects.AddButtonCssSelector), How.CssSelector);
        }

        public void ClickOnAddIcon()
        {
            SiteDriver.FindElement(ReferenceManagerPageObjects.AddButtonCssSelector, How.CssSelector).Click();
        }

        public List<string> GetListByLabelInSearchGrid(string label)
        {
            var catchSelector = string.Format(ReferenceManagerPageObjects.SearchByLabelInSearchGridXPath, label);
            var catchList = JavaScriptExecutor.FindElements(string.Format(ReferenceManagerPageObjects.SearchByLabelInSearchGridXPath, label), How.XPath, "Text");
            return catchList;
        }

        public void AddDataforClaimReference(string[] newClaimReferenceData, bool isEditClaimReference = false, bool waitToLoad = true)
        {
            if (!isEditClaimReference)
            {
                ClickOnAddIcon();
                GetSideWindow.SelectDropDownValue("Client", newClaimReferenceData[0]);
                GetSideWindow.FillTextAreaBox("Review Guideline", newClaimReferenceData[6]);
                GetSideWindow.SelectDropDownValue("Flag", newClaimReferenceData[1]);
                GetSideWindow.FillInputBox("Proc Code From", newClaimReferenceData[2]);
                GetSideWindow.FillInputBox("Proc Code To", newClaimReferenceData[3]);
                GetSideWindow.FillInputBox("Trig Proc From", newClaimReferenceData[4]);
                GetSideWindow.FillInputBox("Trig Proc To", newClaimReferenceData[5]);

            }
            else
            {
                GetSideWindow.SelectDropDownValue("Flag", newClaimReferenceData[1]);
                GetSideWindow.FillTextAreaBox("Review Guideline", newClaimReferenceData[6]);
                GetSideWindow.FillInputBox("Proc Code From", newClaimReferenceData[2], isEditReference: true);
                GetSideWindow.FillInputBox("Proc Code To", newClaimReferenceData[3], isEditReference: true);
                GetSideWindow.FillInputBox("Trig Proc From", newClaimReferenceData[4], isEditReference: true);
                GetSideWindow.FillInputBox("Trig Proc To", newClaimReferenceData[5], isEditReference: true);

            }
            GetSideWindow.Save();
            if (waitToLoad)
                WaitForWorkingAjaxMessage();
        }

        public bool ClickOnDeleteReferenceManager(string[] createdReferenceManagerDetails, bool verification = false)
        {
            var check = string.Format(ReferenceManagerPageObjects.DeleteButtonofReferenceManagerByFlagProcTrigClientTemplate,
               createdReferenceManagerDetails[1], createdReferenceManagerDetails[2] + "-" + createdReferenceManagerDetails[3], createdReferenceManagerDetails[4] + "-" + createdReferenceManagerDetails[5], createdReferenceManagerDetails[0]
                );
            try
            {
                JavaScriptExecutor.ExecuteClick(check, How.XPath);
                WaitForWorkingAjaxMessage();
                WaitToLoadPageErrorPopupModal();
                if (verification) return true;
                ClickOkCancelOnConfirmationModal(true);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool IsDeleteButtonPresentForReferenceRecord(string[] createdReferenceManagerDetails)
        {
            WaitForWorkingAjaxMessage();
            var check = string.Format(ReferenceManagerPageObjects.DeleteButtonofReferenceManagerByFlagProcTrigClientTemplate,
               createdReferenceManagerDetails[1], createdReferenceManagerDetails[2] + "-" + createdReferenceManagerDetails[3], createdReferenceManagerDetails[4] + "-" + createdReferenceManagerDetails[5], createdReferenceManagerDetails[0]
               );
            return SiteDriver.IsElementPresent(check, How.XPath);
        }

        public string GetDeletedIconToolTipText(string[] newReferenceManagerDetails)
        {
            var check = string.Format(ReferenceManagerPageObjects.DeleteButtonofReferenceManagerByFlagProcTrigClientTemplate,
                newReferenceManagerDetails[1], newReferenceManagerDetails[2] + "-" + newReferenceManagerDetails[3], newReferenceManagerDetails[4] + "-" + newReferenceManagerDetails[5], newReferenceManagerDetails[0]
            );
            return SiteDriver.FindElement(
                check, How.XPath).GetAttribute("title");
        }

        public bool ShowAppealRationaleAudit(string[] newAppealRationaleData, bool refresh = false, bool emptyProc = false)
        {
            if (refresh)
            {
                RefreshPage();
            }
            ToggleSideBarMenu();
            var check = "";
            if (emptyProc)
                check = string.Format(ReferenceManagerPageObjects.ReferenceRationaleAuditHistoryTemplate,
                  newAppealRationaleData[1], newAppealRationaleData[2] + "-" + newAppealRationaleData[3], newAppealRationaleData[4] + "-" + newAppealRationaleData[5], newAppealRationaleData[0]
              );
            else
                check = string.Format(ReferenceManagerPageObjects.ReferenceRationaleAuditHistoryTemplate,
                  newAppealRationaleData[1], newAppealRationaleData[2] + "-" + newAppealRationaleData[3], newAppealRationaleData[4] + "-" + newAppealRationaleData[5], newAppealRationaleData[0]
             );
            JavaScriptExecutor.ExecuteClick(check, How.XPath);
            WaitForWorkingAjaxMessage();
            ToggleSideBarMenu();
            return true;
        }

        public bool IsTextBoxDisabled(string label)
        {
            return !SiteDriver.FindElement(string.Format(ReferenceManagerPageObjects.EditFieldInputBoxXpathTemplatebyLabel, label), How.XPath).Enabled;
        }

        public bool IsReviewGuidelineDisabled()
        {
            return !SiteDriver.FindElement(ReferenceManagerPageObjects.EditFieldReviewGuidelineXpathTemplate, How.XPath).Enabled;
        }

        public string GetReferenceManagerAuditHistoryActionByRowCol(int row, int col)
        {
            return SiteDriver.FindElement(
                string.Format(ReferenceManagerPageObjects.AuditHistoryTemplateFieldsByRowColXpathSelector, row, col), How.XPath).Text;
        }

        public bool IsReferenceManagerAuditWindowPresent()
        {
            return SiteDriver.IsElementPresent(ReferenceManagerPageObjects.ReferenceManagerAuditSideWindowXpathSelector,
                How.XPath);
        }

        public bool ShowReferenceAudit(string[] ReferenceData, bool refresh = false, bool emptyTrig = false)
        {
            if (refresh)
            {
                RefreshPage();
                SiteDriver.WaitForCondition(IsReferenceManagerAuditWindowPresent);
            }
            var check = "";
            if (emptyTrig)
                check = string.Format(ReferenceManagerPageObjects.ReferenceRationaleAuditHistoryTemplate,
                    ReferenceData[0], ReferenceData[1] + "-" + ReferenceData[2], ReferenceData[3] + "" + ReferenceData[4], ReferenceData[5]);
            else
                check = string.Format(ReferenceManagerPageObjects.ReferenceRationaleAuditHistoryTemplate,
                    ReferenceData[0], ReferenceData[1] + "-" + ReferenceData[2], ReferenceData[3] + "-" + ReferenceData[4], ReferenceData[5]);
            WaitForWorkingAjaxMessage();
            if (GetSideBarPanelSearch.IsSideBarPanelOpen())
                GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
            if (IsReferenceManagerAuditWindowPresent()) return true;
            JavaScriptExecutor.ExecuteClick(check, How.XPath);
            SiteDriver.WaitForCondition(IsReferenceManagerAuditWindowPresent);
            return true;
        }

        public void ClickOnEditClaimReferenceIcon(string[] claimReferenceData)
        {
            var check = string.Format(ReferenceManagerPageObjects.EditIconReferenceManagerByFlagProcTrigClientTemplate,
                claimReferenceData[1], claimReferenceData[2] + "-" + claimReferenceData[3], claimReferenceData[4] + "-" + claimReferenceData[5],
                claimReferenceData[0]);
            JavaScriptExecutor.ExecuteClick(check, How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public void ClickOnEditIconByRow(int row)
        {
            var check = string.Format(ReferenceManagerPageObjects.EditIconByRow, row);
            JavaScriptExecutor.ExecuteClick(check, How.XPath);
            WaitForWorkingAjaxMessage();
        }

        public void ToggleSideBarMenu()
        {
            JavaScriptExecutor.ExecuteClick(ReferenceManagerPageObjects.SearchIconCssLocator, How.CssSelector);
        }

        public void ClickOkButtonOnEditConfirmWindow()
        {
            JavaScriptExecutor.ExecuteClick(ReferenceManagerPageObjects.ClickOkButtonOnEditConfirmWindow, How.XPath);
        }

        public List<string> GetAuditHistoryListByCol(int col)
        {
            return JavaScriptExecutor.FindElements(string.Format(ReferenceManagerPageObjects.ReferenceManagereAuditValueListByCol, 2),
                How.XPath, "Attribute:title");
        }

        public void ClickCancelButtonOnEditConfirmWindow()
        {
            JavaScriptExecutor.ExecuteClick(ReferenceManagerPageObjects.ClickCancelButtonOnEditConfirmWindow, How.XPath);
        }

    }
}