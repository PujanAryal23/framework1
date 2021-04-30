using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Provider
{
    public class ProviderBillHistoryPageObjects : PageBase
    {
        #region PRIVATE FIELDS

        #region CONSTANTS

        private const string ProviderSequenceId = "ctl00_MainContentPlaceHolder_PrvLabel";
        private const string AllButtonId = "ctl00_MainContentPlaceHolder_AllHistoryRbtn";
        private const string DownloadId = "ctl00_MainContentPlaceHolder_HistoryAndExportToExcelBtn";
        public const string GridHeaderId = "ctl00_MainContentPlaceHolder_ResultsGrid_ctl00_Header";
        public const string LoadingImageId = "ctl00_MainContentPlaceHolder_RadAjaxLoadingPanel1";
        public const string ClaimSequenceXPathTemplate =
            "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr[td[2]/a[contains(text(),'{0}')]]";

        public const string DxCode4XPathTemplate = "//tr[@id='{0}']/td[26]";
        public const string DxCode4VersionXPathTemplate = "//tr[@id='{0}']/td[27]";
        public const string ExtraClaimNoColumnXPath =
            "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00_Header']/thead/tr/th[43]";



        #endregion

        #endregion

        #region PAGEOBJECTS PROPERTIES

        //[FindsBy(How = How.Id, Using = ProviderSequenceId)]
        //public TextLabel ProviderSequence;

        //[FindsBy(How = How.Id, Using = AllButtonId)]
        //public RadioButton AllButton;

        //[FindsBy(How = How.Id, Using = DownloadId)]
        //public ImageButton DownLoadButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderBillHistory.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }

        #endregion

        #region CONSTRUCTOR

        public ProviderBillHistoryPageObjects()
            : base(PageUrlEnum.ClaimHistory.GetStringValue())
        {
        }

        #endregion
    }
}
