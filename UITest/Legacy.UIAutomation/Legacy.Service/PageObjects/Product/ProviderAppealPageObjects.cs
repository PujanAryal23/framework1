using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;

namespace Legacy.Service.PageObjects.Product
{
    public class ProviderAppealPageObjects : PageBase
    {
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderAppeal.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public ProviderAppealPageObjects()
            : base(PageUrlEnum.ProviderAppeal.GetStringValue())
        {

        }

        #endregion
    }
}
