using UIAutomation.Framework.Utils;

namespace Legacy.Service.Support.Enum
{
    public enum MedicalPreAuthPageUrlEnum
    {
        [StringValue("MedicalPreAuth/PreAuthSearch.aspx")]
        PreAuthorizationSearch = 0,

        [StringValue("{0}PreAuth/PreAuth.aspx")]
        PreAuth = 1,

        [StringValue("MedicalPreAuth/ListPage.aspx")]
        MedicalListPage = 2,

        [StringValue("MedicalPreAuth/SearchLogic.aspx")]
        LogicRequests = 3,

        [StringValue("Product/SendEmail.aspx?emailEvent=logic")]
        NotifyClient = 4,

        [StringValue("MedicalPreAuth/calendar.html")]
        Calendar = 5,
    }
}