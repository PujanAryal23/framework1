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
    public class ProviderClaimHistoryPageObjects : NewPageBase
    {
        #region PRIVATE FIELDS

        #region CONSTANTS

        public const string ProviderSequenceId = "ctl00_MainContentPlaceHolder_PrvLabel";
        public const string AllButtonId = "ctl00_MainContentPlaceHolder_AllHistoryRbtn";
        public const string TwelveMonthButtonId = "ctl00_MainContentPlaceHolder_YearHistoryRbtn";
        public const string DownloadId = "ctl00_MainContentPlaceHolder_HistoryAndExportToExcelBtn";
        public const string GridHeaderId = "ctl00_MainContentPlaceHolder_ResultsGrid_ctl00_Header";
        public const string LoadingImageId = "ctl00_MainContentPlaceHolder_RadAjaxLoadingPanel1";
        public const string ClaimSequenceXPathTemplate =
            "//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr[td[2]/a[contains(text(),'{0}')]]";

        public const string DxCodeXPathTemplate = "//tr[@id='{0}']/td[{1}]";//26 for dxCode 4
        public const string DxCodeVersionXPathTemplate = "//tr[@id='{0}']/td[{1}]";
        public const string PageNumberXPathTemplate = "//div[@class='rgWrap rgNumPart']/a/span[text()='{0}']";
        public const string AltClaimNoXPathTemplate = "//tr[td/a[text()='{0}']]/td[3]";
        public const string DxCodeRangeId = "ctl00_MainContentPlaceHolder_DiagnosisCodeRangeControl_RangeText";
        public const string SearchButtonId = "ctl00_MainContentPlaceHolder_SearchBtn";

        //in case of extra white space or tabs, use normalize-space() to check for text using xpath.
        public const string ResultGridColumnXpathTemplateByRowAndValue =
            "(//table[@id='ctl00_MainContentPlaceHolder_ResultsGrid_ctl00']/tbody/tr)[{0}]//td/a[normalize-space()='{1}']"; //{0} = row, {1} = value of column


        public const string ToolTipHeaderValueCssTemplate = "#info_tip div.tip_header span";
        public const string ToolTipContentTemplate = "#info_tip div.tip_content li:nth-of-type({0})";
        public const string NavPopUpClass = "SubNavPopup";

        public const string PageHeaderCssTemplate = ".PopupPageTitle";
        #endregion

        #endregion

        #region PAGEOBJECTS PROPERTIES

        //[FindsBy(How = How.Id, Using = ProviderSequenceId)]
        //public TextLabel ProviderSequence;

        //[FindsBy(How = How.Id, Using = AllButtonId)]
        //public RadioButton AllButton;

        //[FindsBy(How = How.Id, Using = DownloadId)]
        //public ImageButton DownLoadButton;

        //[FindsBy(How = How.Id, Using = TwelveMonthButtonId)]
        //public ImageButton TwelveButton;

        //[FindsBy(How = How.CssSelector, Using = PageHeaderCssTemplate)]
        //public TextLabel PageHeader;
        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ClaimHistoryPopup.GetStringValue(); }
        }

        public string OriginalWindowHandle
        {
            get { return SiteDriver.CurrentWindowHandle; }
        }
       
        #endregion

        #region CONSTRUCTOR

        public ProviderClaimHistoryPageObjects()
            : base(PageUrlEnum.ClaimHistory.GetStringValue())
        {
        }


        #endregion
    }
}
