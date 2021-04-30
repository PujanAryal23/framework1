using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Product
{
    public class BatchStatisticsReportPageObjects : DefaultPageObjects
    {
        #region PRIVATE FIELDS

        public const string SpanTblHeaderListXPath = "//span[@id='Label2']//table[{0}]//table//th";

        public const string SpanTblRowTitleListXPath = "//span[@id='Label2']//table[{0}]//table//tr//td[1]";
        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get
            {
                return string.Format(PageTitleEnum.BatchStatisticsReport.GetStringValue(), StartLegacy.Product.GetStringValue());
            }
        }

        #endregion

        #region CONSTRUCTOR

        public BatchStatisticsReportPageObjects()
            : base(ProductPageUrlEnum.BatchStatisticsReport.GetStringValue())
        {

        }
        #endregion
    }
}
