using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    public enum CompanyEnum
    {
        [StringValue("Cotiviti")]
        companyName=0
    }

    public enum PageHeaderEnum
    {
        /// <summary>
        /// QA Claim Search
        /// </summary>
        [StringValue("QA Claim Search")]
        QaClaimSearch = 0,

        /// <summary>
        /// Provider Action
        /// </summary>
        [StringValue("Provider Action")]
        ProviderAction = 1,

        /// <summary>
        /// Claim Search
        /// </summary>
        [StringValue("Claim Search")]
        ClaimSearch = 2,

        /// <summary>
        /// Quick Launch
        /// </summary>
        [StringValue("Nucleus Home")]
        QuickLaunch = 3,

        /// <summary>
        /// Dashboard
        /// </summary>
        [StringValue("Dashboard")]
        Dashboard = 4,

        

        /// <summary>
        /// Dashboard
        /// </summary>
        [StringValue("Appeal Rationale Manager")]
        AppealRationaleManager = 5,

        /// <summary>
        /// Claim Action
        /// </summary>
        [StringValue("Claim Action")]
        ClaimAction = 6,

        /// <summary>
        /// Provider Search
        /// </summary>
        [StringValue("Provider Search")]
        ProviderSearch = 7,

        /// <summary>
        /// QA Claim Search
        /// </summary>
        [StringValue("QA Appeal Search")]
        QaAppealSearch = 8,


        /// <summary>
        /// QA Claim Search
        /// </summary>
        [StringValue("Appeal Creator")]
        AppealCreator = 9,

        /// <summary>
        /// User Profile Search
        /// </summary>
        [StringValue("User Profile Search")]
        UserProfileSearch = 10,

        /// <summary>
        /// Current page type is retro
        /// </summary>

        [StringValue("retro")]
        RetroPage = 11,

        /// <summary>
        /// Current page type is Ember
        /// </summary>
        [StringValue("Ember")]
        Ember = 12,

        /// <summary>
        /// Repository
        /// </summary>
        [StringValue("Repository")]
        Repository = 13,

        /// <summary>
        /// Reference manager
        /// </summary>
        [StringValue("Reference Manager")]
        ReferenceManager = 14,

        /// <summary>
        /// Invoice
        /// </summary>
        [StringValue("Invoice Search")]
        InvoiceSearch = 15,

        /// <summary>
        /// Batch
        /// </summary>
        [StringValue("Batch Search")]
        BatchSearch = 16,

        /// <summary>
        /// Batch Summary
        /// </summary>
        [StringValue("Batch Summary")]
        BatchSummary = 17,

        /// <summary>
        /// Batch Summary
        /// </summary>
        [StringValue("Appeal Action")]
        AppealAction = 18,

        /// <summary>
        /// Suspect Providers
        /// </summary>
        [StringValue("Suspect Providers")]
        SuspectProviders = 19,

        /// <summary>
        /// Microstrategy
        /// </summary>
        [StringValue("Microstrategy")]
        Microstrategy = 20,

        /// <summary>
        /// Batch Summary
        /// </summary>
        [StringValue("Appeal Summary")]
        AppealSummary = 21,

        /// <summary>
        /// Appeal Search
        /// </summary>
        [StringValue("Appeal Search")]
        AppealSearch = 22,

        /// <summary>
        /// Microstrategy Maintenance
        /// </summary>
        [StringValue("MicroStrategy")]
        MicrostrategyMaintenance = 23,

        /// <summary>
        /// Pre-Authorization
        /// </summary>
        [StringValue("Pre-Auth Creator")]
        PreAuthCreator = 24,

        /// <summary>
        /// Appeal Manager
        /// </summary>
        [StringValue("Appeal Manager")]
        AppealManager = 26,

        /// <summary>
        /// Appeal Category Manager
        /// </summary>
        [StringValue("Appeal Category Manager")]
        AppealCategoryManager = 27,

        /// <summary>
        /// Report Center
        /// </summary>
        [StringValue("Report Center")]
        ReportCenter = 28,

        /// <summary>
        /// QA Manager
        /// </summary>
        [StringValue("Analyst Manager")]
        QAManager = 29,

        /// <summary>
        /// Client Search
        /// </summary>
        [StringValue("Client Search")]
        ClientSearch = 30,

        /// <summary>
        /// My Profile
        /// </summary>
        [StringValue("My Profile")]
        MyProfile = 31,

        /// <summary>
        /// My Profile
        /// </summary>
        [StringValue("My Profile")]
        CoreMyProfile = 90,

        /// <summary>
        /// Maintenance Banners
        /// </summary>
        [StringValue("Maintenance Notices")]
        MaintenanceBanners = 32,
      

        /// <summary>
        /// Logic Search
        /// </summary>
        [StringValue("Logic Search")]
        LogicSearch = 25,

        /// <summary>
        /// Pre-Authorization Search
        /// </summary>
        [StringValue("Pre-Auth Search")]
        PreAuthSearch = 34,

        [StringValue("Help Center")]
        HelpCenter = 35,

        /// <summary>
        /// Pre-Auth Action
        /// </summary>
        [StringValue("Pre-Auth Action")]
        PreAuthAction = 36,

        /// <summary>
        /// Dashboard
        /// </summary>
        [StringValue("Dashboard - Appeals Detail")]
        DashboardAppealsDetail = 37,

        /// <summary>
        /// Dashboard
        /// </summary>
        [StringValue("Dashboard - Claims Detail")]
        DashboardClaimsDetail = 38,

        /// <summary>
        /// Dashboard
        /// </summary>
        [StringValue("Dashboard - Logic Requests Detail")]
        DashboardLogicRequestsDetail = 39,

        /// <summary>
        /// Dashboard
        /// </summary>
        [StringValue("Dashboard - FFP Claims Detail")]
        DashboardFFPClaimsDetail = 40,

        /// <summary>
        /// Dashboard
        /// </summary>
        [StringValue("Appeal Letter")]
        AppealLetter = 41,

        [StringValue("Client Profile")]
        ClientProfile = 42,

        /// <summary>
        /// Provider Scorecard
        /// </summary>
        [StringValue("Provider Scorecard")]
        ProviderScoreCard = 43,

        /// <summary>
        /// Provider Profile
        /// </summary>
        [StringValue("Provider Profile")]
        ProviderProfile = 44,

        /// <summary>
        /// Claim History
        /// </summary>
        [StringValue("Claim History")]
        ClaimHistory = 45,

        /// <summary>
        /// Pre-Auth History
        /// </summary>
        [StringValue("Pre-Auth Processing History")]
        PreAuthProcessingHistory = 46,

        /// <summary>
        /// Provider Claim History
        /// </summary>
        [StringValue("Provider Claim Search")]
        ProviderClaimSearch = 47,

        /// <summary>
        /// Patient Pre-Auth History
        /// </summary>
        [StringValue("Patient Pre-Auth History")]
        PatientPreAuthHistory = 48,

        /// <summary>
        /// Edit Settings Manager
        /// </summary>
        [StringValue("Edit Settings Manager")]
        EditSettingsManager = 49,

        /// <summary>
        /// Role Manager
        /// </summary>
        [StringValue("Role Manager")]
        RoleManager = 50,

        /// <summary>
        ///User Settings
        /// </summary>
        [StringValue("User Settings")]
        UserSettings = 51,

        /// <summary>
        /// New
        /// </summary>
        [StringValue("New User Account")]
        CreateUser = 52
    }
}
