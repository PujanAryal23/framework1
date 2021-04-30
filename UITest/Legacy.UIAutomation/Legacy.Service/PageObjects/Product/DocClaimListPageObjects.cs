using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Product
{
    public class DocClaimListPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES/FIELDS

        public static string DocClaimListPageTitle;

        public const string ClaimSeqXpath = ".//a[contains(text(),'{0}')]";
       

        #endregion

        #region PAGEOBJECTS PROPERTIES

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return DocClaimListPageTitle; }
        }

        #endregion

        #region CONSTRUCTOR


        public DocClaimListPageObjects()
            : base(ProductPageUrlEnum.DocClaimList.GetStringValue())
        {
            
        }
        
        #endregion
    }
}
