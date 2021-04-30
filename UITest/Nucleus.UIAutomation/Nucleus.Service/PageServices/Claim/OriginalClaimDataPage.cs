using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Claim;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Claim
{
    public class OriginalClaimDataPage : NewBasePageService
    {
          #region PRIVATE FIELDS

        private OriginalClaimDataPageObjects _originalClaimDataPage;


        #endregion

        #region PUBLIC FIELDS

        public bool IsColumnPresentByColumnName(string colName)=>                   
                SiteDriver.IsElementPresent(
                    string.Format(OriginalClaimDataPageObjects.TableColumnHeaderXPathTemplate, colName), How.XPath);
        

        public bool IsPopupTitleDisplayed()=>        
             SiteDriver.IsElementPresent(OriginalClaimDataPageObjects.PopupTitleCssLocator, How.CssSelector);
        

        public string GetPageHeader()=>       
             SiteDriver.FindElement(OriginalClaimDataPageObjects.PopupTitleCssLocator, How.CssSelector).Text;
        

        public string GetClaimSequenceInHeader()=>
             SiteDriver.FindElement(OriginalClaimDataPageObjects.ClaimSequenceCssLocator,How.CssSelector).Text.Split().LastOrDefault();
        

        public List<string> GetColumnNames()=>
            JavaScriptExecutor.FindElements(OriginalClaimDataPageObjects.ColumnNamesCssLocator, How.CssSelector, "Text").Skip(1).Take(116).ToList();
       

        public (List<List<string>>,List<string>) GetOriginalClaimDataFromDatabase(string claSeq)
        {
            var list = Executor.GetCompleteTable(String.Format(ClaimSqlScriptObjects.GetOriginalClaimDataFromDatabase,
                claSeq.Split('-')[0], claSeq.Split('-')[1]));
            var originalClaimData = list.Select(row => row.ItemArray.Select(x => Regex.Replace(x.ToString(), " {2,}", " ")).ToList()).ToList();
            var columnNames =
                list.FirstOrDefault().Table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            return (originalClaimData,columnNames);
        }

        public List<string> GetOriginalClaimDataValuesByRow(int row)=>
            JavaScriptExecutor.FindElements(String.Format(OriginalClaimDataPageObjects.OriginalClamDataXpath, row), How.XPath, "Text").Skip(1).Take(116).ToList();
        






        #endregion

    #region CONSTRUCTOR

    public OriginalClaimDataPage(INewNavigator navigator, OriginalClaimDataPageObjects claimHistoryPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, claimHistoryPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _originalClaimDataPage = (OriginalClaimDataPageObjects)PageObject;
        }
        #endregion

        #region PUBLIC METHODS

      
       

        
        #endregion
    }
}
