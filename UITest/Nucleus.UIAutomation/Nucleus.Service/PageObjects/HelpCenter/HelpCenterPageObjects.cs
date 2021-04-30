
using System.Drawing;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Common;

namespace Nucleus.Service.PageObjects.HelpCenter
{
    public class HelpCenterPageObjects : NewDefaultPageObjects
    {
        #region PUBLIC PROPERTIES

        public const string HelpButtonId = "ctl00_HelpBtn";
        public const string FormsTabId = "ctl00_MainContentPlaceHolder_FormsTab";
        public const string SystemTabId = "ctl00_MainContentPlaceHolder_SystemTab";
        public const string ReleaseTabId = "ctl00_MainContentPlaceHolder_ReleaseTab";
        public const string FraudPreventionQuickStartGuideLinkId = "ctl00_MainContentPlaceHolder_GuidesListView_ctrl0_GuideLink";
        public const string FraudPreventionOnlineHelpLinkId = "ctl00_MainContentPlaceHolder_GuidesListView_ctrl1_GuideLink";
        public const string ClaimsEditingQuickStartGuideLinkId = "ctl00_MainContentPlaceHolder_GuidesListView_ctrl2_GuideLink";
        public const string ClaimsEditingOnlineHelpLinkId = "ctl00_MainContentPlaceHolder_GuidesListView_ctrl3_GuideLink";
        
        public const string NewUserRequestFormLinkId = "ctl00_MainContentPlaceHolder_FormsView_ctrl0_FormsLink";
        public const string ClientCustomizationFormLinkId = "ctl00_MainContentPlaceHolder_FormsView_ctrl2_FormsLink";
        public const string UserTerminationRequestFormLinkId = "ctl00_MainContentPlaceHolder_FormsView_ctrl1_FormsLink";
        public const string UserReportRequestFormLinkId = "ctl00_MainContentPlaceHolder_FormsView_ctrl3_FormsLink";
        public const string FraudPreventionEobFlagDescLinkId = "ctl00_MainContentPlaceHolder_SystemInfoListView_ctrl0_SysInfoLink";
        public const string ClaimsEditingEobFlagDescLinkId = "ctl00_MainContentPlaceHolder_SystemInfoListView_ctrl1_SysInfoLink";
        public const string RulesEngineReleaseNotesLinkId = "ctl00_MainContentPlaceHolder_ReleaseNotesListView_ctrl0_ReleaseLink";
        public const string FormHeaderCssSelector = "div.info_box.forms>header";
        public const string SystemInformationHeaderCssSelector = "div.system_info>header";
        public const string HelpDeskHeaderCssSelector = "div.help_desk>header";
        public const string DownloadIconByFormNameCssTemplate = "div.form:has(span:contains({0}))>a,div.form:has(a:contains({0}))>a.download_button"; 
        public const string HelpDeskInfoSectionHeadersCssSelector = "div.help_desk li.info_section>header";
        public const string InfoSectionDescriptionXPathTemplate = "//header[text()='{0}']/following-sibling::p";
        public const string AdobeAcrobatReaderCssSelector = "div.forms span>a";
        public const string AdobeTextCssSelector = "div.forms span:has(a:contains(Adobe Acrobat Reader))";
        public const string CotivitiUrlCssSelector = "li:has(header:contains(Operating Hours))>p>a";
        public const string HolidaysListCssSelectorTemplate = "li:has(header:contains({0}))>p";

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.HelpCenter.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public HelpCenterPageObjects()
            : base(PageUrlEnum.HelpCenter.GetStringValue())
        {
        }

        #endregion
    }
}
