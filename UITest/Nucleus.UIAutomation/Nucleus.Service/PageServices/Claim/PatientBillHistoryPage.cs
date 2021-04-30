using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;


namespace Nucleus.Service.PageServices.Claim
{
    public class PatientBillHistoryPage : BasePageService
    {
        #region PRIVATE FIELDS

        private PatientBillHistoryPageObjects _patientBillHistoryPage;

        #endregion

        #region CONSTRUCTOR

        public PatientBillHistoryPage(INavigator navigator, PatientBillHistoryPageObjects patientBillHistoryPage)
            : base(navigator, patientBillHistoryPage)
        {
            _patientBillHistoryPage = (PatientBillHistoryPageObjects)PageObject;
        }
        #endregion

        #region PUBLIC METHODS

        

       

        #endregion
    }
}
