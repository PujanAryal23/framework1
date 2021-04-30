using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Common.Constants;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service.Support.Common
{
    public class StandardGridFunctionalityService
    {
        private readonly StandardGridFunctionalityPage _standardGridFunctionalityPage;

        public StandardGridFunctionalityService()
        {
            _standardGridFunctionalityPage = new StandardGridFunctionalityPage();
            SiteDriver.InitializePageElement(_standardGridFunctionalityPage);
        }

        #region PUBLIC METHODS

        /// <summary>
        /// 	Perform Right Click and Reset Column Reordering
        /// </summary> 
        public void ResetColumns()
        {
            if ( string.Compare(BrowserConstants.Chrome, EnvironmentManager.Instance.Browser,
                    StringComparison.OrdinalIgnoreCase) == 0)
            {
                     JavaScriptExecutor.ExecuteContextClick(StandardGridFunctionalityPage.GridHeaderXPathChrome, How.XPath);
                     JavaScriptExecutor.ExecuteClick(StandardGridFunctionalityPage.ResetMenuXPath, How.XPath);
            }
            else if (string.Compare(BrowserConstants.Iexplorer, EnvironmentManager.Instance.Browser,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                SiteDriver.RightClick(StandardGridFunctionalityPage.GridHeaderXPathIE, How.XPath);
                _standardGridFunctionalityPage.ResetMenuOption.Click();
            }
        }

        /// <summary>
        /// Add multiple Columns To Grid and Save
        /// </summary> 
        public void AddColumnsToGridAndSave(List<string> columnToAdd)
        {
            if (string.Compare(BrowserConstants.Chrome, EnvironmentManager.Instance.Browser,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                JavaScriptExecutor.ExecuteContextClick(StandardGridFunctionalityPage.GridHeaderXPathChrome, How.XPath);
                WaitForChrome();
                JavaScriptExecutor.ExecuteMouseOver(StandardGridFunctionalityPage.GridColumnsXPath, How.XPath);
                WaitForChrome();
            }
            else if (string.Compare(BrowserConstants.Iexplorer, EnvironmentManager.Instance.Browser,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                SiteDriver.RightClick(StandardGridFunctionalityPage.GridHeaderXPathIE, How.XPath);
                WaitForIexplorer();
                JavaScriptExecutor.ExecuteMouseOver(StandardGridFunctionalityPage.GridColumnsXPath, How.XPath);
                WaitForIexplorer();
            }
            foreach (string column in columnToAdd)
            {
                JavaScriptExecutor.ExecuteClick(column, How.Id);
            }
            SaveGridCustomization();
        }

        /// <summary>
        /// Add multiple columns To Grid and Save
        /// </summary>
        /// <param name="columnXPathTemplate"></param>
        /// <param name="columnToAdd"></param>
        public void AddColumnsToGridAndSaveWithColumnsName(string columnXPathTemplate, List<string> columnToAdd)
        {
            if (string.Compare(BrowserConstants.Chrome, EnvironmentManager.Instance.Browser,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                JavaScriptExecutor.ExecuteContextClick(StandardGridFunctionalityPage.GridHeaderXPathChrome, How.XPath);
                SiteDriver.RightClick(StandardGridFunctionalityPage.GridHeaderXPathChrome, How.XPath);
                WaitForChrome();
                JavaScriptExecutor.ExecuteMouseOver(StandardGridFunctionalityPage.GridColumnsXPath, How.XPath);
                WaitForChrome();
            }

            else if (string.Compare(BrowserConstants.Iexplorer, EnvironmentManager.Instance.Browser,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                SiteDriver.RightClick(StandardGridFunctionalityPage.GridHeaderXPathIE, How.XPath);
                WaitForIexplorer();
                JavaScriptExecutor.ExecuteMouseOver(StandardGridFunctionalityPage.GridColumnsXPath, How.XPath);
                WaitForIexplorer();
            }
            foreach (string column in columnToAdd)
            {
                JavaScriptExecutor.ExecuteClick(string.Format(columnXPathTemplate, column), How.XPath);
            }
            SaveGridCustomization();
        }

        /// <summary>
        /// Perform Right Click and Save Grid Customizations
        /// </summary> 
        public void SaveGridCustomization()
        {
            if (string.Compare(BrowserConstants.Chrome, EnvironmentManager.Instance.Browser,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                SiteDriver.RightClick(StandardGridFunctionalityPage.GridHeaderXPathChrome, How.XPath);
            }
            else if (string.Compare(BrowserConstants.Iexplorer, EnvironmentManager.Instance.Browser,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                SiteDriver.RightClick(StandardGridFunctionalityPage.GridHeaderXPathIE, How.XPath);
            }

            JavaScriptExecutor.ExecuteClick(StandardGridFunctionalityPage.SaveMenuXPath,How.XPath);
        }
       
        #endregion

        #region PRIVATE METHODS
        
        private void WaitForChrome(int milliSecondsTimeout = 000)
        {
            if (string.Compare(BrowserConstants.Chrome, EnvironmentManager.Instance.Browser, StringComparison.OrdinalIgnoreCase) == 0)
                SiteDriver.WaitToLoadNew(milliSecondsTimeout);
        }

        private void WaitForIexplorer(int milliSecondsTimeout = 000)
        {
            if (string.Compare(BrowserConstants.Iexplorer, EnvironmentManager.Instance.Browser, StringComparison.OrdinalIgnoreCase) == 0)
                SiteDriver.WaitToLoadNew(milliSecondsTimeout);
        }
        
        #endregion
    }
}
