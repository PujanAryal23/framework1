using Nucleus.Service.PageObjects.Appeal;

using System.Collections.Generic;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Common;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Utils;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using static System.String;

namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealRationaleManagerPage : NewDefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private readonly AppealRationaleManagerPageObjects _appealRationaleManagerPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly GridViewSection _gridViewSection;
        private readonly SideWindow _sideWindow;
        private readonly CalenderPage _calendarpage;
        private readonly Mouseover Mouseover;
        private readonly FileUploadPage _fileUploadPage;

        #endregion

        #region CONSTRUCTOR

        public AppealRationaleManagerPage(INewNavigator navigator, AppealRationaleManagerPageObjects appealRationaleManager, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealRationaleManager, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealRationaleManagerPage = (AppealRationaleManagerPageObjects)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _calendarpage = new CalenderPage(SiteDriver);
            Mouseover = new Mouseover(SiteDriver, JavaScriptExecutor);
            _fileUploadPage = new FileUploadPage(SiteDriver, JavaScriptExecutor);

        }
        #endregion


        #region DB methods

        public List<string> GetFlagLists()
        {
            return
                Executor.GetTableSingleColumn(AppealSqlScriptObjects.FlagList);
        }

        public List<string> GetSourceLists()
        {
            return
                Executor.GetTableSingleColumn(AppealSqlScriptObjects.SourceList);
        }

        public List<string> GetModifierValues()
        {
            return Executor.GetTableSingleColumn(AppealSqlScriptObjects.ModifierList);


        }

        public List<string> GetTrigProcValues()
        {
            return Executor.GetTableSingleColumn(AppealSqlScriptObjects.ProcTrigList);

        }
        public List<string> GetActiveClientList()
        {
            return
                Executor.GetTableSingleColumn(AppealSqlScriptObjects.GetActiveClientList);
        }

        public int GetActiveRationaleCount()
        {
            return (int)Executor.GetSingleValue(AppealSqlScriptObjects.ActiveRationaleCount);
        }

        public List<string> GetAssignedClientList(string userId)
        {
            return
                Executor.GetTableSingleColumn(string.Format(AppealSqlScriptObjects.GetAssignedClientListForCentene, userId));
        }



        #endregion

        public FileUploadPage GetFileUploadPage
        {
            get { return _fileUploadPage; }
        }

        public bool IsDocumentIconPresentInAppealRationaleAuditHx() =>
            SiteDriver.IsElementPresent(AppealRationaleManagerPageObjects.DocumentsIconInAppealRationaleAuditHx, How.CssSelector);

        public void ClickDocumentIconInAppealRationaleAuditHistoryPage()
        {
            SiteDriver.FindElement(AppealRationaleManagerPageObjects.DocumentsIconInAppealRationaleAuditHx, How.CssSelector).Click();
            WaitForWorking();
        }

        public string GetUploadedFileNameFromDocumentsTab()
        {
            return SiteDriver.FindElement(AppealRationaleManagerPageObjects.UploadedFileNameXpath, How.XPath).Text;
        }

        public void ClickOnFileToOpenInNewTab()
        {
            SiteDriver.FindElement(AppealRationaleManagerPageObjects.UploadedFileNameXpath, How.XPath).Click();
        }

        public List<string> GetUploadedDocInfoFromDB(string clientcode, string editflg, string editsrc, string lowProc, string highProc)
        {
            List<string> docInfo = new List<string>();

            var dataRow = Executor.GetCompleteTable(Format(AppealSqlScriptObjects.GetUploadedDocInfoInAppealRationale, clientcode,
                editflg, editsrc, lowProc, highProc));

            var fileName = dataRow.ToList()[0].ItemArray[0].ToString();
            var uploadedDate = dataRow.ToList()[0].ItemArray[1].ToString();

            docInfo.Add(fileName);
            docInfo.Add(uploadedDate);

            return docInfo;
        }

        public string GetUploadedDateFromDocumentsTab()
        {
            return SiteDriver.FindElement("//label[contains(@title,\"Uploaded Date\")]/following-sibling::span", How.XPath).Text;
        }
            
        
        public int GetRationaleRowsCountFromPage()
        {
            return SiteDriver.FindElementsCount(AppealRationaleManagerPageObjects.RationaleRowsCssLocator, How.CssSelector)-1;
        }

        public void ClickOnLoadMore()
        {
            GetGridViewSection.ClickLoadMore();
            /*JavaScriptExecutor.ExecuteClick(AppealCategoryManagerPageObjects.LoadMoreCssLocator, How.CssSelector);*/
            WaitForWorkingAjaxMessage();
        }

        public string GetLoadMoreText()
        {
            return GetGridViewSection.GetLoadMoreText();
        }
        public bool IsPencilIconPresentInEachRationaleRecord()
        {
            return SiteDriver.FindElementsCount(AppealRationaleManagerPageObjects.EditIconCssLocator, How.CssSelector)
                .Equals(GetRationaleRowsCountFromPage());

        }
        public bool IsPencilIconPresentInRationaleRecord(int rowId)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealRationaleManagerPageObjects.EditRecordIconCssTemplate, rowId),
                How.CssSelector);
        }

        public bool IsDeactivateIconPresentInRationaleRecord(int rowId)
        {
            return SiteDriver.IsElementPresent(string.Format(AppealRationaleManagerPageObjects.DeactivateIconCssTemplate, rowId),
                How.CssSelector);
        }
        public void ToggleSideBarMenu()
        {
            JavaScriptExecutor.ExecuteClick(AppealRationaleManagerPageObjects.SearchIconCssLocator, How.CssSelector);

        }
        public void ClickOnAddButton()
        {
            JavaScriptExecutor.ExecuteClick(AppealRationaleManagerPageObjects.AddButtonCssLocator, How.CssSelector);
        }

        /// <summary>
        /// Col: 2 => Flag
        /// Col: 3 => Source
        /// Col: 4 => Modifier
        /// Col: 5 => Proc
        /// Col: 6 => Trig
        /// Col: 7 => Client
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public List<string> GetGridListValueByCol(int col = 2)
        {

            return JavaScriptExecutor.FindElements(string.Format(AppealRationaleManagerPageObjects.GetGridViewItembyColCss, col), "Text");
        }




        public void FillDuplicateData(string[] data)
        {
            GetSideWindow.SelectDropDownValue("Client", data[0]);
            GetSideWindow.SelectDropDownValue("Flag", data[1]);
            GetSideWindow.SelectDropDownValue("Source Value", data[2]);
            GetSideWindow.FillIFrame("Rationale", data[3], 20);
            GetSideWindow.FillInputBox("Rationale Source", data[4]);

        }

        public void FillFormData(string[] data, bool procCode = false, bool trigProc = false,string[] effectiveDos=null)
        {
            GetSideWindow.SelectDropDownValue("Client", data[0]);
            GetSideWindow.SelectDropDownValue("Flag", data[1]);
            GetSideWindow.SelectDropDownValue("Source Value", data[2]);
            
            if (procCode)
            {
                GetSideWindow.FillInputBox("Proc Code From", data[3]);
                GetSideWindow.FillInputBox("Proc Code To", data[4]);
            }
            else
            {
                GetSideWindow.GetInputBox("Proc Code From").ClearElementField();
                GetSideWindow.GetInputBox("Proc Code To").ClearElementField();
            }

            if (trigProc)
            {
                GetSideWindow.FillInputBox("Trig Proc From", data[5]);
                GetSideWindow.FillInputBox("Trig Proc To", data[6]);
            }
            else
            {
                GetSideWindow.GetInputBox("Trig Proc From").ClearElementField();
                GetSideWindow.GetInputBox("Trig Proc To").ClearElementField();
            }

            if (effectiveDos == null)
            {
                GetSideWindow.SetDateFieldFrom("Effective DOS",DateTime.Now.ToString("MM/d/yyyy"));
            }
            else
            {
                GetSideWindow.SetDateFieldFrom("Effective DOS",
                    Convert.ToDateTime(effectiveDos[0]).ToString("MM/d/yyyy"));
                if (effectiveDos.Length==2)
                    GetSideWindow.SetDateFieldTo("Effective DOS",
                        Convert.ToDateTime(effectiveDos[1]).ToString("MM/d/yyyy"));
            }

            GetSideWindow.FillIFrame("Rationale", data[7]);
            GetSideWindow.FillInputBox("Rationale Source", data[8]);

        }

        public void FillOverlappingData(string[] data, bool proc = true)
        {
            GetSideWindow.SelectDropDownValue("Client", data[0]);
            GetSideWindow.SelectDropDownValue("Flag", data[1]);
            GetSideWindow.SelectDropDownValue("Source Value", data[2]);
            GetSideWindow.FillIFrame("Rationale", data[3]);
            GetSideWindow.FillInputBox("Rationale Source", data[4]);
            if (proc)
            {
                GetSideWindow.FillInputBox("Proc Code From", data[5]);
                GetSideWindow.FillInputBox("Proc Code To", data[6]);
                return;
            }
            GetSideWindow.GetInputBox("Proc Code From").ClearElementField();
            GetSideWindow.GetInputBox("Proc Code To").ClearElementField();
            GetSideWindow.FillInputBox("Trig Proc From", data[5]);
            GetSideWindow.FillInputBox("Trig Proc To", data[6]);
        }






        public bool Check_Maxlength_by_Label(string label, string length)
        {
            var a = _sideWindow.GetInputBox(label)
                .GetAttribute("maxlength");
            return a.Contains(length);
        }



        public List<int>[] GetProcFromValueFromUI()
        {

            var verify = GetGridListValueByCol(5);
            var procFromList = new List<int>();
            var procToList = new List<int>();
            List<int>[] finalValue = new List<int>[2];
            foreach (var item in verify)
            {
                var temp = item.Split(':')[1];
                if (temp.Length > 0)
                {
                    procFromList.Add(int.Parse(temp.Split('\n')[1].Split('-')[0]));
                    procToList.Add(int.Parse(temp.Split('\n')[1].Split('-')[0]));

                }

            }
            finalValue[0] = procFromList;
            finalValue[1] = procToList;
            return finalValue;

        }

        public bool IsAppealRatioanleAuditWindowPresent()
        {
            return SiteDriver.IsElementPresent(AppealRationaleManagerPageObjects.AppealRationaleAuditSideWindowXpathSelector,
                How.XPath);
             
        }

        public string GetAppealAuditHistoryTitle()
        {
            return SiteDriver.FindElement(AppealRationaleManagerPageObjects.AppealRationaleAuditSideWindowTitle,
                How.CssSelector).Text;
        }



        public List<string> GetAuditHistoryListByCol(int col)
        {
            return JavaScriptExecutor.FindElements(string.Format(AppealRationaleManagerPageObjects.AppealRationaleAuditValueListByCol, 2),
                How.XPath, "Text");



        }

        public List<string> GetAppealSubMenuOptions(String menuName)
        {
            Mouseover.IsAppealSearch();
            SiteDriver.WaitToLoadNew(500);
            return JavaScriptExecutor.FindElements(string.Format(DefaultPageObjects.SubMenuCategoryListXPathTemplate, menuName), How.XPath, "Text");
        }

        public string GetAppealAuditHistoryActionByRowCol(int row, int col)
        {
           
            return SiteDriver.FindElement(
                string.Format(AppealRationaleManagerPageObjects.AuditHistoryTemplateFieldsByRowColXpathSelector, row, col), How.XPath).Text;



        }

       
        public string GetDeletedIconToolTipText(string[] newAppealRationaleData)
        {
            var check = string.Format(AppealRationaleManagerPageObjects.DeleteButtonofAppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate ,
                newAppealRationaleData[1], newAppealRationaleData[2], "", newAppealRationaleData[8] + "-" + newAppealRationaleData[9], "", newAppealRationaleData[0]
            );
            return SiteDriver.FindElement(
                check, How.XPath).GetAttribute("title");
           

           

        }
        
        public bool ShowAppealRationaleAudit(string[] newAppealRationaleData, bool refresh=false, bool emptyProc=false)

        {
            if (refresh)
                RefreshPage();
            if(GetGridViewSection.IsLoadMorePresent())
                GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
            ToggleSideBarMenu();
            var check = "";
            if (emptyProc)
                 check = string.Format(AppealRationaleManagerPageObjects.AppealRationaleAuditHistoryTemplate,
                    newAppealRationaleData[1], newAppealRationaleData[2], "", newAppealRationaleData[8] + newAppealRationaleData[9], "", newAppealRationaleData[0]
                );
            else
             check = string.Format(AppealRationaleManagerPageObjects.AppealRationaleAuditHistoryTemplate,
                newAppealRationaleData[1], newAppealRationaleData[2], "", newAppealRationaleData[8] +"-"+ newAppealRationaleData[9], "", newAppealRationaleData[0]
            );
            JavaScriptExecutor.ExecuteClick(check, How.XPath);
            WaitForWorkingAjaxMessage();
            ToggleSideBarMenu();
            return true;


        }
        public bool IsActiveAndInactiveCountNonZero()
        {
            return JavaScriptExecutor.FindElements(AppealRationaleManagerPageObjects.ActiveXpathSelector, How.XPath,
                       "Attribute:title").Count > 0 &&
                   JavaScriptExecutor.FindElements(AppealRationaleManagerPageObjects.InActiveXpathSelector, How.XPath,
                       "Attribute:title").Count > 0 ;

        }

        public void CreateAppealRationale(string[] newAppealRationaleData, string cagapproval = null , string effDos=null, string overrideValue=null, bool clickSave = true,bool uploadFile = false,string fileName="",bool formType = false)
        {
            ClickOnDeleteAppealRationale(newAppealRationaleData);
            GetSideWindow.SelectDropDownValue("Client", newAppealRationaleData[0]);
            GetSideWindow.SelectDropDownValue("Flag", newAppealRationaleData[1]);
            GetSideWindow.SelectDropDownValue("Source Value", newAppealRationaleData[2]);
            GetSideWindow.FillIFrame("Rationale", newAppealRationaleData[3]);
            GetSideWindow.FillIFrame("Pay Summary", newAppealRationaleData[4]);
            GetSideWindow.FillIFrame("Deny Summary", newAppealRationaleData[5]);
            GetSideWindow.FillIFrame("Adjust Summary", newAppealRationaleData[6]);
            GetSideWindow.FillIFrame("Helpful Appeal Hints", newAppealRationaleData[10]);
            GetSideWindow.FillInputBox("Rationale Source", newAppealRationaleData[7]);

            if (uploadFile)
            {
                GetFileUploadPage.AddFileForUpload(fileName);
            }

            // GetSideWindow.GetInputBox("Proc Code From").ClearElementField();
            // GetSideWindow.GetInputBox("Proc Code To").ClearElementField();
            GetSideWindow.GetInputBox("Trig Proc From").ClearElementField();
            GetSideWindow.GetInputBox("Trig Proc To").ClearElementField();
            GetSideWindow.GetInputBox("Modifier").ClearElementField();

            GetSideWindow.FillInputBox("Proc Code From", newAppealRationaleData[8]);
            GetSideWindow.FillInputBox("Proc Code To", newAppealRationaleData[9]);

            if (cagapproval != null)
            {
                var element = GetSideWindow.GetInputBox("CAG Approval");
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                GetCalendarPage.SetDate(cagapproval);

            }
            if(effDos==null)
                effDos = DateTime.Now.ToString("MM/d/yyyy");

            if (overrideValue != null)
                GetSideWindow.SelectDropDownValue("Override ",overrideValue);

            if (formType)
                GetSideWindow.SelectDropDownValue("Form Type ", newAppealRationaleData[11]);

            GetSideWindow.GetInputBox("Effective DOS").Click();
            GetCalendarPage.SetDate(Convert.ToDateTime(effDos).ToString("MM/d/yyyy"));

            if (clickSave)
            {
                GetSideWindow.Save();
                WaitForWorkingAjaxMessage();
            }
        }

        public void ScrollToSpecificRow(int row = 0)
        {
            row = SiteDriver.FindElementsCount(AppealRationaleManagerPageObjects.RationaleRowsCssLocator,
                How.CssSelector);
            if(row>0)
                JavaScriptExecutor.ExecuteToScrollToSpecificDiv(
                string.Format(AppealRationaleManagerPageObjects.RationaleRowsCssLocator + ":nth-of-type({0})", row));
        }

        public void ScrollToLastPosition()
        {
            JavaScriptExecutor.ExecuteToScrollToSpecificDiv(AppealRationaleManagerPageObjects.SaveButtonCSSSelector);
        }

        public void ClickOnEditAppealRationale(string[] createdAppealRationaleDetails)
        {
            
            var check = string.Format(AppealRationaleManagerPageObjects.EditButtonofAppealRationaleRowCategoryByFlagSourceModifierProcTrigClientTemplate,
              createdAppealRationaleDetails[1], createdAppealRationaleDetails[2], "", createdAppealRationaleDetails[8] + "-" + createdAppealRationaleDetails[9], "", createdAppealRationaleDetails[0]
              );
            JavaScriptExecutor.ExecuteClick(check, How.XPath);
            WaitForWorkingAjaxMessage();
        }
        public void ClickOnEditAppealRationale(string flag, string source, string modifier, string proc, string trig, string client)
        {
            var check = string.Format(AppealRationaleManagerPageObjects.EditButtonOfAppealRationaleRowbyFlagSourceModifierProcTrigClientCssTemplate,flag,source,modifier,proc,trig,client);
            JavaScriptExecutor.ClickJQuery(check);
            WaitForWorkingAjaxMessage();
        }

        public int CountOfAppealRationaleByRationaleData(string[] createdAppealRationaleDetails)
        {
            return SiteDriver.FindElementsCount(string.Format(
                AppealRationaleManagerPageObjects.DeleteButtonofAppealRationaleRowCategoryByFlagSourcProcClientTemplate,
                createdAppealRationaleDetails[1], createdAppealRationaleDetails[2],
                createdAppealRationaleDetails[8] + "-" + createdAppealRationaleDetails[9],
                createdAppealRationaleDetails[0]
            ), How.XPath);
        }
       
        public bool ClickOnDeleteAppealRationale(string[] createdAppealRationaleDetails,bool verification=false)
        {
            if(GetGridViewSection.IsLoadMorePresent())
                GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
            var check = string.Format(AppealRationaleManagerPageObjects.DeleteButtonofAppealRationaleRowCategoryByFlagSourcProcClientTemplate,
                createdAppealRationaleDetails[1], createdAppealRationaleDetails[2],  createdAppealRationaleDetails[8] + "-" + createdAppealRationaleDetails[9],  createdAppealRationaleDetails[0]
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

        public string GetFormTypeForAppealRationale(string[] createdAppealRationaleDetails)
        {
            var lastRow = GetGridViewSection.GetTotalInLoadMore();
            if (GetGridViewSection.IsLoadMorePresent())
                GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
            var check = Format(AppealRationaleManagerPageObjects.FormTypeValueofRationaleRowCategoryByFlagSourcProcClientTemplate,
                createdAppealRationaleDetails[1], createdAppealRationaleDetails[2], createdAppealRationaleDetails[8] + "-" + createdAppealRationaleDetails[9], createdAppealRationaleDetails[0]
            );
            ScrollToSpecificRow(lastRow);
            return SiteDriver.FindElement(check, How.XPath).Text;
        }

        public string GetValueInGridByColRow(int row = 1, int col = 2) //2:flag; 3:source
        {
            return SiteDriver.FindElement(
                string.Format(AppealRationaleManagerPageObjects.ValueInGridByRowColumnCssTemplate, row, col), How.CssSelector).Text;

        }
        public string GetValueInGridBylabelAndRow(string label, int row = 1) //2:flag; 3:source
        {
            return SiteDriver.FindElement(
                string.Format(AppealRationaleManagerPageObjects.ValueInGridByLabelXpathTemplate, label, row), How.XPath).Text;

        }

        public List<string> GetSourceListsAndSort()
        {
            var sourcelist = GetSourceLists();
            sourcelist.Sort();
            return sourcelist;
        }

        public void ClickFindButtonAndWait()
        {
            SiteDriver.WaitToLoadNew(300);
            _sideBarPanelSearch.ClickOnFindButton();
            SiteDriver.WaitToLoadNew(1000);
        }

        public void ClickPencilIconofRow(int row = 1)
        {
            SiteDriver.FindElementAndClickOnCheckBox(string.Format(AppealRationaleManagerPageObjects.EditRecordIconCssTemplate, row),
                How.CssSelector);
            SiteDriver.WaitToLoadNew(3000);
        }

        public bool IsEditWindowPresent()
        {
            return SiteDriver.IsElementPresent(AppealRationaleManagerPageObjects.EditWindowCSSLocator, How.CssSelector);
        }

        public void HardDeleteRationale(String[] createdAppeal)
        {
            var query = string.Format(AppealSqlScriptObjects.DeleteAppealRationale, createdAppeal[0],
                createdAppeal[8], createdAppeal[9],createdAppeal[3]);
            Executor.ExecuteQueryAsync(query);
          

        }

        public string GetEditSrcForClaSeq(string claseq, string flagSeq, string clasub = "0")
        {
            var query = string.Format(AppealSqlScriptObjects.GetEditSrcForClaSeq, claseq,  clasub, flagSeq);
            return Executor.GetSingleStringValue(query);


        }


        public List<String> GetSearchResultListByCol(int col, bool removeNull = true)
        {

            var list = JavaScriptExecutor.FindElements(
                  string.Format(AppealRationaleManagerPageObjects.RationaleSearchResultListCssTemplate, col), How.CssSelector, "Text");
            if (removeNull)
                list.RemoveAll(String.IsNullOrEmpty);
            return list;
        }

        public String GetSearchResultByColRow(int col, int row = 1)
        {
            return
                SiteDriver.FindElement(
                    string.Format(AppealRationaleManagerPageObjects.RationaleResultByRowColCssTemplate, row, col),
                    How.CssSelector).Text;
        }

        public bool IsListStringSortedInAscendingOrder(int col)
        {
            if (col==6)
                return GetSearchResultListByCol(col).Select(n => n.Split('-')[0]).ToList().IsInAscendingOrder();
            if (col == 8)
                return GetSearchResultListByCol(col).Select<string, string>(s => s == "All Centene" ? "CTN" : s).ToList()
                    .IsInAscendingOrder();
            return GetSearchResultListByCol(col).IsInAscendingOrder();
        }
       
        public bool IsListStringSortedInDescendingOrder(int col)
        {
            if (col == 6)
                return GetSearchResultListByCol(col).Select(n => n.Split('-')[0]).ToList().IsInDescendingOrder();
            if (col == 8)
                return GetSearchResultListByCol(col).Select<string, string>(s => s == "All Centene" ? "CTN" : s).ToList()
                    .IsInDescendingOrder();
            return GetSearchResultListByCol(col).IsInDescendingOrder();
        }

        public bool IsHelpfulAppealHintsTextBoxPresent() => SiteDriver.IsElementPresent(AppealRationaleManagerPageObjects.HelpfulAppealHintsTextBoxXpath, How.XPath);

        #region Windows

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public CalenderPage GetCalendarPage
        {
            get { return _calendarpage; }
        }


        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        #endregion

        public string GetAppealHelpfulHintsForARationaleFromDb(String[] newAppealRationaleData) =>
                Executor.GetSingleStringValue(string.Format(AppealSqlScriptObjects.GetAppealHelpfulHintsInfoInAppealRationale,newAppealRationaleData[0],newAppealRationaleData[1],newAppealRationaleData[2],newAppealRationaleData[8],newAppealRationaleData[9]));

        public string GetAppealFormTypeForARationaleFromDb(String[] newAppealRationaleData) =>
            Executor.GetSingleStringValue(string.Format(AppealSqlScriptObjects.GetFormTypeInfoInAppealRationale, newAppealRationaleData[0], newAppealRationaleData[1], newAppealRationaleData[2], newAppealRationaleData[3], newAppealRationaleData[4]));

        public void UpdateAppealFormTypeForARationaleFromDb(String[] newAppealRationaleData, string formType)
        {
            string type = (formType == "All") ? "null" : $"'{formType}'";
            Executor.ExecuteQuery(string.Format(AppealSqlScriptObjects.UpdateFormTypeInfoInAppealRationale, newAppealRationaleData[0], newAppealRationaleData[1], newAppealRationaleData[2], newAppealRationaleData[3], newAppealRationaleData[4], type));

        }

        public string GetClaimFormTypeFromDb(string claimSequence) =>
            Executor.GetSingleStringValue(string.Format(AppealSqlScriptObjects.GetClaimFormTypeFromDb, claimSequence.Split('-').ToList()[0], claimSequence.Split('-').ToList()[1]));

        public void UpdateClaimFormTypeFromDb(string claimSequence,string formType) =>
            Executor.ExecuteQuery(string.Format(AppealSqlScriptObjects.UpdateClaimFormTypeFromDb, claimSequence.Split('-').ToList()[0], claimSequence.Split('-').ToList()[1], formType));

    }
}
