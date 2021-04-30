using System.Data;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageObjects.Default;

namespace Nucleus.Service.PageObjects.QuickLaunch
{
    public class QuickLaunchPageObjects : NewDefaultPageObjects
    {

        #region PUBLIC FIELDS


        public const string GoToLinkXPathTemplate = "//span[contains(text(),'{0}')]";

        public const string AlertsListId = "qlAlertsList";
        public const string PasswordChangeAlertModalId = "nucleus_modal_content";
        public const string PasswordChangeAlertModalCloseButtonId = "nucleus_modal_close";
        public const string DashboardIconCssSelector = "a.dashboard.tooltip";
        public const string NewClaimSearchTileCssSelector = "a#ctl00_MainContentPlaceHolder_lnkClaimSearch";
        public const string TopNavigationTabListCssSelector = "ul#main_nav >li[class*=top_nav]>header";
        public const string HelpIconCssSelector = "a.help_center.tooltip";

        public const string SwitchClientIconCssSelector = "a.switch_client.tooltip";
        public const string CustomizeLinkCssSelector = "a.ModifySettings";
        public const string DashboardQuickLaunchSettingsXpath = "//li[@id='qlSettingsDashboard']";
        public const string ClaimSearchQuickLaunchSettingsXpath = "//li[@id='qlSettingsClaimSearch']";
        public const string FlaggedClaimsQuickLaunchSettingsXpath = "//li[@id='qlSettingsFlaggedClaims']";
        public const string PendedClaimsQuickLaunchSettingsXpath = "//li[@id='qlSettingsPendedClaims']";
        public const string UnreviewedClaimsQuickLaunchSettingsXpath = "//li[@id='qlSettingsUnReviewedClaims']";
        public const string UnreviewedFFPClaimsQuickLaunchSettingsXpath = "//li[@id='qlSettingsUnReviewedFFPClaims']";
        public const string CloseCustomizeButtonXpath = "//a[@id='qlSettingsClose']";
        public const string HeaderMenuXpathLocatorTemplate = "//div[@id='master_navigation']//li[contains(@class,'top_nav')][{0}]/header";
        public const string PendedWorkListCssSelector = "li#qlPendedWorklist>a";

        #region QuickLaunchTileIds

        
        public const string UnreviewedWorkListQuickLaunchTileXpath = "//li[@id='qlUnreviewedWorklist']";
        public const string CMSDatabaseQuickLaunchTileXpath = "//li[@id='qlCMSCoverageDatabase']";
        public const string CotivitiQuickLaunchTileXpath = "//li[@id='qlCotiviti']";
        public const string CotivitiBlogQuickLaunchTileXpath = "//li[@id='qlCotivitiBlog']";
        public const string KnowledgeBankQuickLaunchTileXpath = "//li[@id='qlKnowledgeBank']";
        public const string QuestDxQuickLaunchTileXpath = "//li[@id='qlQuestDiagnostic']";
        public const string EnclarityQuickLaunchTileXpath = "//li[@id='qlEnclarity']";
        public const string FDAQuickLaunchTileXpath = "//li[@id='qlFDA']";
        public const string LabCorpQuickLaunchTileXpath = "//li[@id='qlLabCorp']";
        public const string OIGExclusionsQuickLaunchTileXpath = "//li[@id='qlOIGExclusions']";
        public const string HealthGradesQuickLaunchTileXpath = "//li[@id='qlHealthGrades']";
        public const string UCompareQuickLaunchTileXpath = "//li[@id='qlUcomparehealth']";
        public const string CodeCorrectQuickLaunchTileXpath = "//li[@id='qlCodeCorrect']";
        public const string ClaimSearchQuickLaunchTileXpath = "//li[@id='qlClaimSearch']";
        public const string LogicSearchQuickLaunchTileXpath = "//li[@id='qlLogicSearch']";
        public const string InvoiceSearchQuickLaunchTileXpath = "//li[@id='qlInvoiceSearch']";
        public const string CotivitiFlaggedProvidersQuickLaunchTileXpath = "//li[@id='qlInternalFlaggedProviders']";
        public const string ClientFlaggedProvidersQuickLaunchTileXpath = "//li[@id='qlClientFlaggedProviders']";
        public const string DashboardQuickLaunchTileXpath = "//li[@id='qlDashboard']";
        public const string ProviderQuickLaunchTileXpath = "//li[@id='qlProviderSearch']";
        public const string AppealCreatorQuickLaunchTileXpath = "//li[@id='qlAppealCreator']";
        public const string AppealSearchQuickLaunchTileXpath = "//li[@id='qlAppealSearch']";
        public const string MyProfileQuickLaunchTileXpath = "//li[@id='qlMyProfile']";
        public const string ReportCenterQuickLaunchTileXpath = "//li[@id='qlReportsCenter']";
        public const string SuspectProviderQuickLaunchTileXpath = "//li[@id='qlSuspectProvider']";
        public const string PendedWorkListQuickLaunchTileXpath = "//li[@id='qlPendedWorklist']";
        public const string HelpCenterQuickLaunchTileXpath = "//li[@id='qlHelpCenter']";




        #endregion

        #endregion

        

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.QuickLaunch.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public QuickLaunchPageObjects()
            : base(PageUrlEnum.QuickLaunch.GetStringValue())
        {
        }
        #endregion
    }
}
