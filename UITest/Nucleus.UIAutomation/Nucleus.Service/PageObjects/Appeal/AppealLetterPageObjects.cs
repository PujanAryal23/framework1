using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Common;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealLetterPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string PageHeaderCssLocator = "div#popupcontainer span.title";
        public const string ReviewDateControlId ="ctl00_MainContentPlaceHolder_reviewDateControl";
        public const string DownloadPdfClass = "downloadPdfLink";
        public const string IroPhraseId = "ctl00_MainContentPlaceHolder_iroPhrase";
        public const string IroCareOfId = "ctl00_MainContentPlaceHolder_iroCareOf";
        public const string IroDeptNameId = "ctl00_MainContentPlaceHolder_iroDeptName";
        public const string IroStreetId = "ctl00_MainContentPlaceHolder_iroStreet";
        public const string IroSuiteId = "ctl00_MainContentPlaceHolder_iroSuite";
        public const string IroCityId = "ctl00_MainContentPlaceHolder_iroCity";
        public const string IroStateId = "ctl00_MainContentPlaceHolder_iroState";
        public const string IroZipId = "ctl00_MainContentPlaceHolder_iroZip";
        public const string IroPhoneId = "ctl00_MainContentPlaceHolder_iroPhone";
        public const string IroTollFreeId = "ctl00_MainContentPlaceHolder_iroTollFree";
        public const string IroFaxId = "ctl00_MainContentPlaceHolder_iroFax";
        public const string IroEmailId = "ctl00_MainContentPlaceHolder_iroEmail";
        public const string IroWebSite1Id = "ctl00_MainContentPlaceHolder_iroWebSite1";
        public const string IroWebSite2Id = "ctl00_MainContentPlaceHolder_iroWebSite2";

        public const string AppealLetterClosingId = "appeal_letter_closing";
        public const string ReviewDisclaimerId = "review_disclaimer";
        public const string FooterCssSelector = "ul.appeal_letter_footer_row li";
        public const string AppealLetterFullDetailClassName = "appeal_letter_wrap";

        public const string AppealLetterClaimLinesByLineNumberXPathTemplate =
            "//(b[contains(text(),'Claim Sequence')]/../ul/li)[{0}]";

        public const string AppealClaimDetailsSectionCssSelector = "div.appeal-claim-details";
        public const string LineNumbersListFromAppealLetterByAppealClaimDetailsNumber = "(//div[contains(@class,'appeal-claim-details')])[{0}]/ul/li";
        public const string NotesListFromAppealLetterByAppealClaimDetailsNumber = "(//div[contains(@class,'appeal-claim-details')])[{0}]/div[contains(@class,'line_info')]/p";
        public const string ClaimNoCSSSelector = "#appealResultsSection > div > span > a";
        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealLetter.GetStringValue(); }
        }
        
        #endregion

        #region CONSTRUCTOR

        public AppealLetterPageObjects()
            : base(PageUrlEnum.AppealLetter.GetStringValue())
        {
        }

        #endregion
    }
}
