using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum PageTitleEnum
    {
        #region LOGIN

        [StringValue("Nucleus: Login")]

        Login = 0,

        #endregion

        #region CLAIM

        [StringValue("Claim Search | Nucleus", "Claim Search")]
        ClaimSearch = 1,

        [StringValue("Nucleus")]
        LogicSearch = 84,




        [StringValue("Nucleus")]
        ClaimAction = 100,

        [StringValue("Nucleus")]
        ClinicalOps = 101,


        #endregion

        [StringValue("Nucleus")]
        NewClaimSearch = 60,
        [StringValue("Patient Bill History")]
        PatientBillHistory = 5,
        [StringValue("Patient Claim History")]
        ExtendedPageClaimHistory = 6,
        [StringValue("Patient Pre-Auth History")]
        PatientPreAuthHistory = 7,
        [StringValue("Claim History")]
        ClaimHistoryPopup = 8,

        [StringValue("Patient Search | Nucleus")]
        PatientSearch = 10,
        [StringValue("Appeal History | Nucleus")]
        AppealHistory = 13,
        [StringValue("Notes | Nucleus")]
        AppealNotes = 14,
        [StringValue("Report Center | Nucleus")]
        ReportCenter = 17,
        [StringValue("Profile Manager | Nucleus")]
        ProfileManager = 20,
        [StringValue("Create New User")]
        CreateNewUser = 21,
        [StringValue("Nucleus")]
        MaintenanceNotices = 50,
        [StringValue("Nucleus")]
        QuickLaunch = 22,
        [StringValue("Nucleus")]
        HelpCenter = 23,
        [StringValue("Provider Scorecard | Nucleus")]
        ProviderScoreCard = 24,
        [StringValue("Provider Profile")]
        ProviderProfilePage = 25,
        [StringValue("Original Claim Data")]
        OriginalClaimDataPage = 26,
        [StringValue("Patient Diagnosis History")]
        PatientDiagnosisHistoryPage = 27,
        [StringValue("Processing History")]
        ClaimProcessingHistoryPage = 28,
        [StringValue("Claim Invoice")]
        ClaimInvoicePage = 29,
        [StringValue("Upload Documents")]
        DocumentUploader = 30,
        [StringValue("Notes | Nucleus")]
        NotesPage = 31,
        [StringValue("Fax Cover Sheet")]
        FaxCoverSheetPage = 32,
        [StringValue("Appeal Email")]
        AppealEmail = 33,
        [StringValue("Appeal Letter | Nucleus")]
        AppealLetter = 34,
        [StringValue("Nucleus")]
        AppealCreator = 35,
        [StringValue("Logic Management")]
        LogicManagementPage = 36,
        [StringValue("Nucleus")]
        BillAction = 37,

        [StringValue("Nucleus")]
        AppealCategoryManager = 59,
        [StringValue("Flag Manager")]
        FlagManager = 39,
        [StringValue("Patient Bill History")]
        ExtendedPageBillHistory = 40,
        [StringValue("Nucleus")]
        BillAction2 = 41,
        [StringValue("Bill Search | Nucleus")]
        BillSearch = 42,
        [StringValue("Bill History")]
        ProviderBillHistory = 43,
        [StringValue("Client Profile Management | Nucleus")]
        ClientProfileManager = 44,
        [StringValue("Invoice Detail | Nucleus")]
        InvoiceDetail = 45,
        [StringValue("Remit/Contact")]
        RemitContact = 302,
        [StringValue("Notes | Nucleus")]
        ClaimNotes = 46,
        [StringValue("Dashboard")]
        Dashboard = 47,
        [StringValue("Dashboard - Claims Detail")]
        DashboardClaimsDetail = 55,
        [StringValue("Dashboard - Appeals Detail")]
        DashboardAppealsDetail = 56,
        [StringValue("Dashboard - Logic Requests Detail")]
        DashboardLogicRequestDetail = 57,
        [StringValue("Nucleus")]
        ProviderAction = 48,
        [StringValue("Notes | Nucleus")]
        ProviderNotes = 49,

        [StringValue("Appeal Audit")]
        AppealProcessingHistory = 51,

        [StringValue("Nucleus", "Appeal Summary")]
        AppealSummary = 52,

        [StringValue("Nucleus")]
        AppealSearch = 53,

        [StringValue("Nucleus", "Appeal Action")]
        AppealAction = 54,

        [StringValue("Nucleus")]
        AppealManager = 58,

        [StringValue("Dashboard - FFP Claims Detail")]
        FFPDashboardClaimsDetail = 61,

        [StringValue("Nucleus")]
        QAManager = 70,

        [StringValue("Nucleus")]
        QAClaimSearch = 71,

        [StringValue("Nucleus")]
        AppealRationaleManager = 79,

        [StringValue("Nucleus")]
        ProviderSearch = 77,

        [StringValue("Nucleus")]
        SuspectProviders,

        [StringValue("Nucleus")]
        QAAppealSearch = 80,

        [StringValue("Nucleus")]
        Repository = 81,

        [StringValue("Nucleus")]
        InvoiceSearch = 82,

        [StringValue("Nucleus: MicroStrategy")]
        Microstrategy = 83,

        [StringValue("Nucleus")]
        Microstrategymaintainance = 85,

        [StringValue("Claim Invoice")]
        InvoiceData = 105,

        [StringValue("Nucleus")]
        ClientSearch = 106,

        [StringValue("Nucleus")]
        UserProfileSearch = 107,

        [StringValue("Nucleus")]
        EditSettingsManager = 110,

        [StringValue("Patient Tooth History")]
        PatientToothHistory = 111,

        [StringValue("Nucleus")]
        MyProfile = 112,

        [StringValue("Dashboard - COB Appeals Detail")]
        COBAppealsDetail = 113,

        [StringValue("Dashboard - Claims Detail")]
        COBClaimsDetail = 115,





        [StringValue("Nucleus: Login")]
        Logout = 114,


        #region FraudAlliance

        [StringValue("Nucleus")]
        FraudOps = 301,

        #endregion

        #region PreAuthorization
        [StringValue("Nucleus")]
        PreAuthorization = 86,

        [StringValue("Pre-Auth Processing History | Nucleus")]
        PreAuthProcessingHistory = 108,

        [StringValue("Nucleus")]
        ProviderClaimSearch = 109,

        #endregion

    }
}
