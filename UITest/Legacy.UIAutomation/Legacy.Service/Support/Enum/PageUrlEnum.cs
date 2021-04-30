using UIAutomation.Framework.Utils;

namespace Legacy.Service.Support.Enum
{
    public enum PageUrlEnum
    {
        #region RATIONALE

        [StringValue("Rationale/View_Rationale.aspx")]
        ViewRationale = 20,

        #endregion

        #region ATS

        [StringValue("ATS/ATSCreateCheck.aspx")]
        ProviderAppeal = 30,
        #endregion

        #region PRE-AUTHORIZATIONS

        [StringValue("{0}PreAuth/PreAuthSearch.aspx")]
        PreAuthorizationSearch = 40,

        [StringValue("{0}PreAuth/PreAuth.aspx")]
        PreAuth = 41,

        [StringValue("{0}PreAuth/ListPage.aspx")]
        ListPage = 42,

        [StringValue("{0}PreAuth/Closed.aspx")]
        Closed = 43,

        #endregion

    }
}
