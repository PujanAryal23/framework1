using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class BillSearchPageObjects : NewDefaultPageObjects
    {
        #region public/PUBLIC FIELDS

        #region CONSTANTS

        public const string IsRequestInProgress = "($telerik.radControls[0]._manager == null) ? false : $telerik.radControls[0]._manager._isRequestInProgress";

        public const string AllClaims = "All Claims";
        public const string AllClearedClaims = "All Cleared Claims";
        public const string ClaimReviewList = "Claim Review List";
        public const string ClientClearedClaims = "Client Cleared Claims";
        public const string FlaggedClaims = "Flagged Claims";
        public const string HciClearedClaims = "HCI Cleared Claims";
        public const string PendedClaims = "Pended Claims";
        public const string RulesEngineClearedClaims = "Rules Engine Cleared Claims";
        public const string SystemClearedClaims = "System Cleared Claims";
        public const string UnreviewedClaims = "Unreviewed Claims";


        public const string ALL = "All";
        public const string CLEAR = "Clear";
        public const string DEFAULT = "Select one or more";

        #endregion

        #region ID

        public const string SearchComboDropDownId = "ctl00_MainContentPlaceHolder_SearchCombo_DropDown";
        public const string PageSizeComboInputArrowId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00_ctl03_ctl01_PageSizeComboBox_Arrow";
        public const string ListPageSizeItemId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00_ctl03_ctl01_PageSizeComboBox_DropDown";

        #region GRID HEADER

        public const string ProviderSequenceId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_rghcMenu_i8_i17_chkctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00PrvSeq";
        public const string ProviderNumberId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_rghcMenu_i8_i19_chkctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00ProvNum";
        public const string PatientNameId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_rghcMenu_i8_i24_chkctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00PatName";
        public const string PatientSequenceId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_rghcMenu_i8_i27_chkctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00PatSeq";
        public const string PatientNumberId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_rghcMenu_i8_i28_chkctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00PatNum";
        public const string ReviewGroupId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_rghcMenu_i8_i29_chkctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00ReviewGroup";

        #endregion

        public const string SearchComboId = "ctl00_MainContentPlaceHolder_SearchCombo_Input";
        public const string BatchIdComboLocatorId = "ctl00_MainContentPlaceHolder_BatchCombo_Input";
        public const string ClaimStatusTextFieldId = "ctl00_MainContentPlaceHolder_StatusCombo_Input";
        public const string ProductTextFieldId = "ctl00_MainContentPlaceHolder_ProductCombo_Input";
        public const string PlanMultiSelectFieldId = "ctl00_MainContentPlaceHolder_PlanGroup";
        public const string PlanButtonId = "ctl00_MainContentPlaceHolder_PlanCombo_multiselect";
        public const string LineOfBusinessTextFieldId = "ctl00_MainContentPlaceHolder_LineOfBusinessCombo_Input";
        public const string ClaimSeqTextFieldId = "ctl00_MainContentPlaceHolder_ClaSeqTxt";
        public const string ClaimSeqSubTextFieldId = "ctl00_MainContentPlaceHolder_ClaSubTxt";
        public const string ClaimNoTextFieldId = "ctl00_MainContentPlaceHolder_ClaimNoTxt";
        public const string PatientSequenceTextFieldId = "ctl00_MainContentPlaceHolder_PatSeqTxt";
        public const string PatientNoTextFieldId = "ctl00_MainContentPlaceHolder_PatNumTxt";
        public const string PatientLastNameTextFieldId = "ctl00_MainContentPlaceHolder_PatientLastNameTxt";
        public const string PatientFullNameTextFieldId = "ctl00_MainContentPlaceHolder_PatientFullNameTxt";
        public const string ProviderSequenceTextFieldId = "ctl00_MainContentPlaceHolder_PrvSeqTxt";
        public const string AssignedToTextFieldId = "ctl00_MainContentPlaceHolder_AssignedToCombo_Input";
        public const string ProviderLastNameTextFieldId = "ctl00_MainContentPlaceHolder_ProviderLastNameTxt";
        public const string ProviderFirstNameTextFieldId = "ctl00_MainContentPlaceHolder_ProviderFirstNameTxt";
        public const string ProviderFullNameTextFieldId = "ctl00_MainContentPlaceHolder_ProviderFullNameTxt";
        public const string ProviderNoTextFieldId = "ctl00_MainContentPlaceHolder_ProviderNumberTxt";
        public const string TblResultGridHeaderId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_GridHeader";
        public const string TblResultGridRowId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00";
        public const string SearchButtonId = "ctl00_MainContentPlaceHolder_SearchBtn";
        public const string ClearLinkId = "ctl00_MainContentPlaceHolder_LinkButton2";
        public const string ReviewGroupTextFieldId = "ctl00_MainContentPlaceHolder_ReviewGroupCombo_Input";

        public const string BatchDateBeginInputId = "ctl00_MainContentPlaceHolder_BeginBatchDateDt_dateInput";
        public const string BatchDateEndInputId = "ctl00_MainContentPlaceHolder_EndBatchDateDt_dateInput";
        public const string TblGridHeaderId = "ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00_Header";
        public const string RadAjaxLoadingPanel = "ctl00_MainContentPlaceHolder_RadAjaxLoadingPanel1";

        #endregion

        #region XPATH

        public const string RadToolTipXPath = "//div[@class='RadToolTip RadToolTip_Nucleus rtVisibleCallout ']";
        public const string ColumnXPathTemplate = "//li//div//span[label[contains(text(), '{0}')]]//input";
        public const string FirstRowWithAddIconEnabledXPath = ".//td/a/span[@class='add_icon']";
        public const string UnlockIconRowXPath = ".//table[@id='ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00']/tbody/tr[not(td/span[@class='table_icon disabled_lock'])]";

        public const string FirstUnlockClaimSeqXPath = ".//table[@id='ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00']/tbody/tr[not(td/span[@class='table_icon disabled_lock'])]/td/a[contains(@href,'ClaimManagement.aspx')]";
        public const string FirstLockClaimSeqXPath = ".//tr[td/span[@class='table_icon disabled_lock']]/td/a[contains(@href,'ClaimManagement.aspx')]";

        #endregion

        #region CSSSELECTORS

        public const string SearchLabelCss = "div.search_group > label.search_label";
        public const string SearchLabelClaimNoCss = "div.search_group > span.search_label";
        public const string AddIconEnabledCss = "span.add_icon";

        #endregion

        #region CLASSNAME

        public const string NoRecordsFoundClassName = "NoRecordsFound";
        public const string NoRecordsBlankGridClassName = "rgNoRecords";

        #endregion

        #region TEMPLATE

        public const string ClaimSearchResultGrids = ".//table[@id='ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00']//tbody//tr[{0}]//td[1]/span[@class='table_icon disabled_lock']";
        public const string ClaimSearchGridsRowXPath = ".//table[@id='ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00']//tbody//tr";
        public const string ResultGridDataTemplate = ".//table[@id='ctl00_MainContentPlaceHolder_ClaimSearchResultGrid_ctl00']/tbody/tr[{0}]/td[{1}]";
        public const string GridClaimSequenceXPathTemplate = "//tr[@id='{0}']/td/a[@class='grid_claim_key']";

        #endregion

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ClaimSearch.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public BillSearchPageObjects()
            : base(PageUrlEnum.ClaimSearch.GetStringValue())
        {
        }

        #endregion
    }
}
