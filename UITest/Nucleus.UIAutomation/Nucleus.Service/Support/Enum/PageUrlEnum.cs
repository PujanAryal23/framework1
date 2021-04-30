using System;
using System.Text.RegularExpressions;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    static class Client
    {
        public const string DefaultClient = "SMTST";
        public const string PEHP = "PEHP";
    }
    public enum PageUrlEnum
    {
        #region MVCPAGE
        [StringValue("popup/medicalcodedescription")]
        MvcUrl = 106,
        #endregion

        #region LOGIN

        [StringValue("default.aspx")]
        Login = 0,

        #endregion

        #region QUICKLAUNCH

        [StringValue("app/#")]
        QuickLaunch = 1,

        #endregion

        #region CLAIM

        [StringValue("clients/" + Client.DefaultClient + "/OriginalClaimData")]
        OrignalClaimData = 20,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/claims?claimSearchType=findClaims")]
        NewClaimSearch = 59,

        [StringValue("Retro/Pages/SearchClaims.aspx")]
        ClaimSearch = 2,


        [StringValue("app/#/clients/" + Client.DefaultClient + "/logics")]
        LogicSearch = 94,



        [StringValue("app/#/clients/" + Client.PEHP + "/claims")]
        BillActionPopup = 104,

        [StringValue("Retro/Pages/LogicRequest/LogicManagement.aspx")]
        LogicManagement = 6,

        [StringValue("Retro/Pages/ToolTips/MedicalCodeDescriptionPopup.aspx")]
        PopupCode = 7,

        [StringValue("popup/medicalcodedescription")]
        NewPopupCode = 68,

        [StringValue("popup/editdescription")]
        FlagPopupCode = 69,
        [StringValue("clients/" + Client.DefaultClient + "/claimhistory")]
        ClaimPatientHistory = 100,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/claims")]
        ClaimAction = 101,

        [StringValue("retro/Pages/ToolTips/MedicalCodeDescriptionPopup.aspx")]
        TriggerCodePopup = 102,

        [StringValue("app/#/clients/")]
        BillAction = 103,


        [StringValue("clients/" + Client.DefaultClient + "/ClaimProcessingHistory")]
        ClaimProcessingHistory = 125,

        [StringValue("clients/" + Client.DefaultClient + "/ClaimInvoice")]
        InvoiceData = 108,

        [StringValue("api/clients/{0}/ClaimsWorkLists?product=CV&claimStatus=1&qa=true&quantity={1}")]
        PCIQAWorkListResponse = 107,

       
        #endregion

        #region BATCH

        [StringValue("app/#/clients/" + Client.DefaultClient + "/batches")]
        BatchSearch = 90,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/batches")]
        BatchSummary = 92,

        #endregion

        #region APPEAL

        [StringValue("retro/Pages/Appeal/AppealHistory.aspx")]
        AppealHistory = 55,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/claims")]
        AppealCreator = 35,


        [StringValue("Retro/Pages/FaxCoverSheet.aspx")]
        FaxCoverSheet = 37,

        [StringValue("Retro/Pages/UploadDocument.aspx")]
        DocumentUploader = 39,

        [StringValue("Retro/Notes/Appeal/")]
        AppealNotes = 40,

        [StringValue("Retro/Pages/Appeal/AppealEmail.aspx")]
        AppealEmail = 41,

        [StringValue("clients/" + Client.DefaultClient + "/AppealLetter")]
        AppealLetter = 42,

        [StringValue("app/#/appeal_category_assignments")]
        AppealCategoryManager = 71,

        [StringValue("/app/#/clients/SMTST/appeal_rationales")]
        AppealRationaleManager = 79,


        [StringValue("clients/SMTST/AppealProcessingHistory")]
        AppealProcessingHistory = 84,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/appeals")]
        AppealSummary = 52,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/appeals")]
        AppealSearch = 53,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/appeals")]
        AppealAction = 54,

        [StringValue("app/#/appeal_manager")]
        AppealManager = 58,

        #endregion

        #region PROVIDER


        [StringValue("Retro/Pages/ProviderAction.aspx")]
        ProviderAction = 46,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/providers/action_providers")]
        NewProviderAction = 51,

        [StringValue("Retro/Notes/Provider/")]
        ProviderNotes = 47,

        [StringValue("Retro/Pages/ProviderProfile.aspx")]
        ProviderProfile = 48,

        [StringValue("clients/" + Client.DefaultClient + "/providerScore/")]
        ProviderScoreCard = 49,

        [StringValue("Retro/Pages/ProviderClaimHistory.aspx")]
        ClaimHistory = 50,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/providers")]
        ProviderSearch = 80,

        [StringValue("clients/" + Client.DefaultClient + "/providerScore/")]
        NewProviderScoreCard = 85,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/suspect_providers")]
        SuspectProviders = 91,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/providers")]
        ProviderClaimSearch = 122,

        #endregion

        #region REPORTCENTER

        [StringValue("Retro/Pages/Reports/Reports.aspx")]
        ReportCenter = 60,

        #endregion

        #region SETTINGS


        [StringValue("Retro/Pages/ProfileManager.aspx")]
        ProfileManager = 62,

        [StringValue("retro/Pages/Administration/CreateNewUser.aspx")]
        OldUserProfileSearch = 63,

        [StringValue("Retro/Pages/ManageClientProfile.aspx")]
        ClientProfileManager = 64,

        [StringValue("app/#/maintenance_notices")]
        MaintenanceNotices = 66,

        [StringValue("app/#/microstrategy")]
        MicrostrategyMaintenance = 67,

        [StringValue("app/#/SearchClients")]
        ClientSearch = 123,

        [StringValue("app/#/users")]
        UserProfileSearch = 121,

        [StringValue("app/#/edits/" + Client.DefaultClient)]
        EditSettingsManager = 124,

        [StringValue("app/#/my_profile")]
        MyProfileSettings = 126,

        #endregion

        #region HELPCENTER

        [StringValue("app/#/help_center")]
        HelpCenter = 65,

        #endregion

        #region INVOICE

        [StringValue("Retro/Pages/Invoice/SearchInvoice.aspx")]
        InvoiceSearch = 72,
        [StringValue("Retro/Pages/Invoice/InvoiceDetail.aspx")]
        InvoiceDetail = 73,
        [StringValue("Retro/Pages/Invoice/RemitContactInformation.aspx")]
        RemitContact = 500,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/invoices")]
        NewInvoiceSearch = 89,

        #endregion

        #region PATIENT
        [StringValue("Retro/Pages/SearchPatients.aspx")]
        PatientSearch = 82,

        #endregion

        #region app

        [StringValue("app/#/clients/" + Client.DefaultClient + "/providers/")]
        FraudOps = 301,

        #endregion

        #region Dashboard

        [StringValue("app/#/app_dashboard")]
        Dashboard = 74,

        [StringValue("app/#/claims_summary_details")]
        ClaimsDetail = 75,

        [StringValue("app/#/appeals_summary_details")]
        AppealsDetail = 76,

        [StringValue("app/#/operations_dashboard/logics_summary_details?product=CV")]
        LogicRequestsDetail = 77,

        [StringValue("mstr")]
        Microstrategy = 88,

        [StringValue("Mstr/Home/MstrReport")]
        MicrostrategyHome = 93,

        [StringValue("app/#/operations_dashboard/cob_appeals_summary_details")]
        COBAppealsDetail = 127,
        [StringValue("app/#/operations_dashboard/cob_claims_summary_details?product=COB")]
        COBClaimsDetail = 127,


        #endregion

        #region QAMANAGER
        [StringValue("app/#/clients/" + Client.DefaultClient + "/qa_manager")]
        QaManager = 70,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/qa_claim_search")]
        QaClaimSearch = 78,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/qa_appeal_search")]
        QaAppealSearch = 81,
        #endregion

        #region Repository
        [StringValue("app/#/repository/")]
        Repository = 86,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/reference_manager")]
        ReferenceManager = 87,

        #endregion

        #region PreAuthorization
        [StringValue("app/#/clients/" + Client.DefaultClient + "/pre_auth_creator")]
        PreAuthorization = 95,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/pre_auths")]
        PreAuthSearch = 96,

        [StringValue("app/#/clients/" + Client.DefaultClient + "/pre_auths")]
        PreAuthAction = 97,

        [StringValue("clients/" + Client.DefaultClient + "/PreAuthHistory")]
        PreAuthProcessingHistory = 98,
        #endregion

        #region OTHER PAGE

        [StringValue("www.cotiviti.com")]
        Cotiviti = 109,

        [StringValue("www.cms.gov")]
        Cms = 110,

        [StringValue("blog.cotiviti.com")]
        CotivitiBlog = 111,

        [StringValue("resources.cotiviti.com")]
        CotivitiResources = 112,

        [StringValue("education.questdiagnostics.com")]
        EducationQuesdiagnostics = 113,

        [StringValue("www.enclarity.com")]
        Enclarity = 114,

        [StringValue("www.accessdata.fda.gov")]
        AccessdataFDA = 115,

        [StringValue("www.labcorp.com")]
        Labcorp = 116,

        [StringValue("exclusions.oig.hhs.gov")]
        ExclusionsOIG = 117,

        [StringValue("www.healthgrades.com")]
        Healthgrades = 118,

        [StringValue("www.ucomparehealthcare.com")]
        Ucomparehealthcare = 119,

        [StringValue("www.codecorrect.com")]
        Codecorrect = 120,



        #endregion
    }
}