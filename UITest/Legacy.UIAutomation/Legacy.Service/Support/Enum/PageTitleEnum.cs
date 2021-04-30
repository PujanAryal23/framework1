
using UIAutomation.Framework.Utils;

namespace Legacy.Service.Support.Enum
{
    public enum PageTitleEnum
    {
        [StringValue("{0} Batch List")]
        BatchList = 0,
        [StringValue("DentalClaim Insight")]
        DentalClaimInsight = 1,
        [StringValue("PhysicianClaim Insight")]
        PhysicianClaimInsight = 2,
        [StringValue("FacilityClaim Insight")]
        FacilityClaimInsight = 3,
        [StringValue("Welcome Page")]
        WelcomePage = 4,
        [StringValue("Processing History")]
        ClaimHistory = 5,
        [StringValue("{0} Daily Claims Batches")]
        FlaggedClaims = 6,
        [StringValue("{0} Daily Claims Batches")]
        BatchStatisticsReport = 7,
        [StringValue("{0} Claim Summary")]
        ClaimSummary = 8,
        [StringValue("HCI Code Description")]
        CodeDesc = 9,
        [StringValue("View Rule Rationale")]
        ViewRationale = 10,
        [StringValue("Provider Appeal")]
        ProviderAppeal = 11,
        [StringValue("Patient History")]
        PatientHistory = 12,
        [StringValue("Document Archive")]
        DocumentUpload = 13,
        [StringValue("{0} Logic Requests")]
        LogicRequests = 14,
        [StringValue("{0} Medical Pre-Authorizations")]
        PreAuthorizationSearch = 15,
        [StringValue("Original Data")]
        OriginalData = 16,
        [StringValue("Send Email")]
        NotifyClient = 17,
        [StringValue("Calendar")]
        Calendar = 18,
        [StringValue("Search {0}")]
        SearchProduct = 19,
        [StringValue("{0} Unreviewed Claims")]
        SearchUnreviewed = 20,
        [StringValue("{0} Modified Edits")]
        ModifiedEdits = 21,
        [StringValue("Search {0} Pended Claims")]
        SearchPended = 22,
        [StringValue("{0} Pre-Authorizations")]
        PreAuthorizations = 23,
        [StringValue("Documents {0} Refund Providers")]
        DocClaimList = 24,
        
        #region PRE AUTHORIZATIONS
        [StringValue("Internal Unreviewed {0}Pre-Authorizations")]
        Unreviewed = 30,

        [StringValue("Pended Pre-Authorizations")]
        MedicalPended = 31,

        [StringValue("Document Review")]
        DocumentReview = 32,

        [StringValue("Consult Required")]
        ConsultRequired = 33,

        [StringValue("Closed {0} Pre-Authorizations")]
        Closed = 34,

        [StringValue("Consult Complete")]
        ConsultComplete = 35,

        [StringValue("Internal Consult Required")]
        HciConsultRequired = 36,

        [StringValue("Internal Consult Complete")]
        HciConsultComplete = 37,
        #endregion


        [StringValue("Verscend Login")]
        Login = 40,
        [StringValue("Rev Code Description")]
        RevDesc= 41,

        #region FRAUD FINDER PRO

        [StringValue("Fraud Finder Pro")]
        FraudFinderPro = 50,

        #endregion

        [StringValue("Search")]
        Search = 42,
        [StringValue("Negotiation Statistics")]
        Negotiation = 43,
        [StringValue("Invoicing")]
        Invoicing = 44,
        [StringValue("Change Password")]
        ChangePassword = 45,
        [StringValue("Reports & Events")]
        Reports = 46,
        [StringValue("Appeal Tracking System")]
        Ats = 47,
        [StringValue("Search Rule Rationale")]
        Rationale = 48,
        
    }
}