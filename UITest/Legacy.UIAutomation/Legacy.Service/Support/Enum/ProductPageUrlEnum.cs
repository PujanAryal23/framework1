using UIAutomation.Framework.Utils;

namespace Legacy.Service.Support.Enum
{
    public enum ProductPageUrlEnum
    {
        [StringValue("Product/Batchmenu.aspx")]
        Batchmenu = 0,

        [StringValue("Product/BatchSummary.aspx")]
        BatchStatisticsReport = 1,

        [StringValue("Product/FlaggedClaims.aspx")]
        FlaggedClaims = 2,

        [StringValue("Product/ClaimHistory.aspx")]
        ClaimHistory = 3,

        [StringValue("Product/ClaSum.aspx")]
        ClaimSummary = 4,

        [StringValue("Product/CodeDesc.aspx")]
        CodeDesc = 5,

        [StringValue("Product/PatHist.aspx")]
        PatientHistory = 6,

        [StringValue("Product/DocumentUpLoad.aspx")]
        DocumentUpload = 7,

        [StringValue("Product/SearchLogic.aspx")]
        LogicRequests = 8,

        [StringValue("Product/Product.aspx")]
        Product = 9,

        [StringValue("Product/Orig.aspx")]
        OriginalData = 10,

        [StringValue("Product/SendEmail.aspx?emailEvent=logic")]
        NotifyClient = 11,

        [StringValue("Product/calendar.html")]
        Calendar = 12,

        [StringValue("Product/SearchProduct.aspx")]
        SearchProduct = 13,

        [StringValue("Product/SearchUnreviewed.aspx")]
        SearchUnreviewed = 14,

        [StringValue("Product/SearchModified.aspx")]
        ModifiedEdits = 15,

        [StringValue("Product/SearchPended.aspx")]
        SearchPended = 16,

        [StringValue("Product/RevDesc.aspx")]
        RevDesc = 17,

        [StringValue("Product/DocClaimList.aspx")]
        DocClaimList = 18,

        [StringValue("Product/SearchCleared.aspx")]
        SearchCleared = 19,

        [StringValue("Product/BatchMenu.aspx")]
        BatchMenu = 20,

        [StringValue("Product/ClaSum.aspx")]
        ClaimSummaryInfo = 21,

        [StringValue("Search.aspx")]
        Search = 22
    }
}